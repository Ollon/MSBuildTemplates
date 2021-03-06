﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Emit
{
    internal abstract class DefinitionMap
    {
        protected struct MappedMethod
        {
            public MappedMethod(IMethodSymbolInternal previousMethodOpt, Func<SyntaxNode, SyntaxNode> syntaxMap)
            {
                this.PreviousMethod = previousMethodOpt;
                this.SyntaxMap = syntaxMap;
            }

            public readonly IMethodSymbolInternal PreviousMethod;
            public readonly Func<SyntaxNode, SyntaxNode> SyntaxMap;
        }

        protected readonly PEModule module;
        protected readonly IReadOnlyDictionary<IMethodSymbol, MappedMethod> mappedMethods;

        protected DefinitionMap(PEModule module, IEnumerable<SemanticEdit> edits)
        {
            Debug.Assert(module != null);
            Debug.Assert(edits != null);

            this.module = module;
            this.mappedMethods = GetMappedMethods(edits);
        }

        private static IReadOnlyDictionary<IMethodSymbol, MappedMethod> GetMappedMethods(IEnumerable<SemanticEdit> edits)
        {
            Dictionary<IMethodSymbol, MappedMethod> mappedMethods = new Dictionary<IMethodSymbol, MappedMethod>();
            foreach (SemanticEdit edit in edits)
            {
                // We should always "preserve locals" of iterator and async methods since the state machine 
                // might be active without MoveNext method being on stack. We don't enforce this requirement here,
                // since a method may be incorrectly marked by Iterator/AsyncStateMachine attribute by the user, 
                // in which case we can't reliably figure out that it's an error in semantic edit set. 

                // We should also "preserve locals" of any updated method containing lambdas. The goal is to 
                // treat lambdas the same as method declarations. Lambdas declared in a method body that escape 
                // the method (are assigned to a field, added to an event, e.g.) might be invoked after the method 
                // is updated and when it no longer contains active statements. If we didn't map the lambdas of 
                // the updated body to the original lambdas we would run the out-of-date lambda bodies, 
                // which would not happen if the lambdas were named methods.

                // TODO (bug https://github.com/dotnet/roslyn/issues/2504)
                // Note that in some cases an Insert might also need to map syntax. For example, a new constructor added 
                // to a class that has field/property initializers with lambdas. These lambdas get "copied" into the constructor
                // (assuming it doesn't have "this" constructor initializer) and thus their generated names need to be preserved. 

                if (edit.Kind == SemanticEditKind.Update && edit.PreserveLocalVariables)
                {
                    IMethodSymbol method = edit.NewSymbol as IMethodSymbol;
                    if (method != null)
                    {
                        mappedMethods.Add(method, new MappedMethod((IMethodSymbolInternal)edit.OldSymbol, edit.SyntaxMap));
                    }
                }
            }

            return mappedMethods;
        }

        internal abstract Cci.IDefinition MapDefinition(Cci.IDefinition definition);

        internal bool DefinitionExists(Cci.IDefinition definition)
        {
            return MapDefinition(definition) != null;
        }

        internal abstract bool TryGetTypeHandle(Cci.ITypeDefinition def, out TypeDefinitionHandle handle);
        internal abstract bool TryGetEventHandle(Cci.IEventDefinition def, out EventDefinitionHandle handle);
        internal abstract bool TryGetFieldHandle(Cci.IFieldDefinition def, out FieldDefinitionHandle handle);
        internal abstract bool TryGetMethodHandle(Cci.IMethodDefinition def, out MethodDefinitionHandle handle);
        internal abstract bool TryGetPropertyHandle(Cci.IPropertyDefinition def, out PropertyDefinitionHandle handle);
        internal abstract CommonMessageProvider MessageProvider { get; }
    }

    internal abstract class DefinitionMap<TSymbolMatcher> : DefinitionMap
        where TSymbolMatcher : SymbolMatcher
    {
        protected readonly TSymbolMatcher mapToMetadata;
        protected readonly TSymbolMatcher mapToPrevious;

        protected DefinitionMap(PEModule module, IEnumerable<SemanticEdit> edits, TSymbolMatcher mapToMetadata, TSymbolMatcher mapToPrevious)
            : base(module, edits)
        {
            Debug.Assert(mapToMetadata != null);

            this.mapToMetadata = mapToMetadata;
            this.mapToPrevious = mapToPrevious ?? mapToMetadata;
        }

        internal sealed override Cci.IDefinition MapDefinition(Cci.IDefinition definition)
        {
            return this.mapToPrevious.MapDefinition(definition) ??
                   (this.mapToMetadata != this.mapToPrevious ? this.mapToMetadata.MapDefinition(definition) : null);
        }

        private bool TryGetMethodHandle(EmitBaseline baseline, Cci.IMethodDefinition def, out MethodDefinitionHandle handle)
        {
            if (this.TryGetMethodHandle(def, out handle))
            {
                return true;
            }

            def = (Cci.IMethodDefinition)this.mapToPrevious.MapDefinition(def);
            if (def != null)
            {
                if (baseline.MethodsAdded.TryGetValue(def, out int methodIndex))
                {
                    handle = MetadataTokens.MethodDefinitionHandle(methodIndex);
                    return true;
                }
            }

            handle = default(MethodDefinitionHandle);
            return false;
        }

        protected static IReadOnlyDictionary<SyntaxNode, int> CreateDeclaratorToSyntaxOrdinalMap(ImmutableArray<SyntaxNode> declarators)
        {
            Dictionary<SyntaxNode, int> declaratorToIndex = new Dictionary<SyntaxNode, int>();
            for (int i = 0; i < declarators.Length; i++)
            {
                declaratorToIndex.Add(declarators[i], i);
            }

            return declaratorToIndex;
        }

        protected abstract void GetStateMachineFieldMapFromMetadata(
            ITypeSymbol stateMachineType,
            ImmutableArray<LocalSlotDebugInfo> localSlotDebugInfo,
            out IReadOnlyDictionary<EncHoistedLocalInfo, int> hoistedLocalMap,
            out IReadOnlyDictionary<Cci.ITypeReference, int> awaiterMap,
            out int awaiterSlotCount);

        protected abstract ImmutableArray<EncLocalInfo> TryGetLocalSlotMapFromMetadata(MethodDefinitionHandle handle, EditAndContinueMethodDebugInformation debugInfo);
        protected abstract ITypeSymbol TryGetStateMachineType(EntityHandle methodHandle);

        internal VariableSlotAllocator TryCreateVariableSlotAllocator(EmitBaseline baseline, Compilation compilation, IMethodSymbolInternal method, IMethodSymbol topLevelMethod, DiagnosticBag diagnostics)
        {
            // Top-level methods are always included in the semantic edit list. Lambda methods are not.
            if (!this.mappedMethods.TryGetValue(topLevelMethod, out MappedMethod mappedMethod))
            {
                return null;
            }

            // TODO (bug https://github.com/dotnet/roslyn/issues/2504):
            // Handle cases when the previous method doesn't exist.

            if (!this.TryGetMethodHandle(baseline, (Cci.IMethodDefinition)method, out MethodDefinitionHandle previousHandle))
            {
                // Unrecognized method. Must have been added in the current compilation.
                return null;
            }

            ImmutableArray<EncLocalInfo> previousLocals;
            IReadOnlyDictionary<EncHoistedLocalInfo, int> hoistedLocalMap = null;
            IReadOnlyDictionary<Cci.ITypeReference, int> awaiterMap = null;
            IReadOnlyDictionary<int, KeyValuePair<DebugId, int>> lambdaMap = null;
            IReadOnlyDictionary<int, DebugId> closureMap = null;

            int hoistedLocalSlotCount = 0;
            int awaiterSlotCount = 0;
            string stateMachineTypeNameOpt = null;
            TSymbolMatcher symbolMap;

            int methodIndex = MetadataTokens.GetRowNumber(previousHandle);
            DebugId methodId;

            // Check if method has changed previously. If so, we already have a map.
            if (baseline.AddedOrChangedMethods.TryGetValue(methodIndex, out AddedOrChangedMethodInfo addedOrChangedMethod))
            {
                methodId = addedOrChangedMethod.MethodId;

                MakeLambdaAndClosureMaps(addedOrChangedMethod.LambdaDebugInfo, addedOrChangedMethod.ClosureDebugInfo, out lambdaMap, out closureMap);

                if (addedOrChangedMethod.StateMachineTypeNameOpt != null)
                {
                    // method is async/iterator kickoff method
                    GetStateMachineFieldMapFromPreviousCompilation(
                        addedOrChangedMethod.StateMachineHoistedLocalSlotsOpt,
                        addedOrChangedMethod.StateMachineAwaiterSlotsOpt,
                        out hoistedLocalMap,
                        out awaiterMap);

                    hoistedLocalSlotCount = addedOrChangedMethod.StateMachineHoistedLocalSlotsOpt.Length;
                    awaiterSlotCount = addedOrChangedMethod.StateMachineAwaiterSlotsOpt.Length;

                    // Kickoff method has no interesting locals on its own. 
                    // We use the EnC method debug information for hoisted locals.
                    previousLocals = ImmutableArray<EncLocalInfo>.Empty;

                    stateMachineTypeNameOpt = addedOrChangedMethod.StateMachineTypeNameOpt;
                }
                else
                {
                    previousLocals = addedOrChangedMethod.Locals;
                }

                // All types that AddedOrChangedMethodInfo refers to have been mapped to the previous generation.
                // Therefore we don't need to fall back to metadata if we don't find the type reference, like we do in DefinitionMap.MapReference.
                symbolMap = mapToPrevious;
            }
            else
            {
                // Method has not changed since initial generation. Generate a map
                // using the local names provided with the initial metadata.
                EditAndContinueMethodDebugInformation debugInfo;
                try
                {
                    debugInfo = baseline.DebugInformationProvider(previousHandle);
                }
                catch (InvalidDataException)
                {
                    // TODO: localize message & use better error code (https://github.com/dotnet/roslyn/issues/11512): 
                    diagnostics.Add(MessageProvider.CreateDiagnostic(
                        MessageProvider.ERR_ModuleEmitFailure,
                        method.Locations.First(),
                        $"Unable to read debug information of method '{MessageProvider.GetErrorDisplayString(method)}' (token 0x{MetadataTokens.GetToken(previousHandle):X8}) " +
                        $"from assembly '{MessageProvider.GetErrorDisplayString(method.ContainingAssembly)}'"));

                    return null;
                }

                methodId = new DebugId(debugInfo.MethodOrdinal, 0);

                if (!debugInfo.Lambdas.IsDefaultOrEmpty)
                {
                    MakeLambdaAndClosureMaps(debugInfo.Lambdas, debugInfo.Closures, out lambdaMap, out closureMap);
                }

                ITypeSymbol stateMachineType = TryGetStateMachineType(previousHandle);
                if (stateMachineType != null)
                {
                    // method is async/iterator kickoff method
                    ImmutableArray<LocalSlotDebugInfo> localSlotDebugInfo = debugInfo.LocalSlots.NullToEmpty();
                    GetStateMachineFieldMapFromMetadata(stateMachineType, localSlotDebugInfo, out hoistedLocalMap, out awaiterMap, out awaiterSlotCount);
                    hoistedLocalSlotCount = localSlotDebugInfo.Length;

                    // Kickoff method has no interesting locals on its own. 
                    // We use the EnC method debug information for hoisted locals.
                    previousLocals = ImmutableArray<EncLocalInfo>.Empty;

                    stateMachineTypeNameOpt = stateMachineType.Name;
                }
                else
                {
                    // If the current method is async/iterator then either the previous method wasn't declared as async/iterator and it's updated to be one,
                    // or it was but is not marked by the corresponding state machine attribute because it was missing in the compilation. 
                    // In the later case we need to report an error since we don't known how to map to the previous state machine.

                    // The IDE already checked that the attribute type is present in the base compilation, but didn't validate that it is well-formed.
                    // We don't have the base compilation to directly query for the attribute, only the source compilation. 
                    // But since constructor signatures can't be updated during EnC we can just check the current compilation.

                    if (method.IsAsync)
                    {
                        if (compilation.CommonGetWellKnownTypeMember(WellKnownMember.System_Runtime_CompilerServices_AsyncStateMachineAttribute__ctor) == null)
                        {
                            ReportMissingStateMachineAttribute(diagnostics, method, AttributeDescription.AsyncStateMachineAttribute.FullName);
                            return null;
                        }
                    }
                    else if (method.IsIterator)
                    {
                        if (compilation.CommonGetWellKnownTypeMember(WellKnownMember.System_Runtime_CompilerServices_IteratorStateMachineAttribute__ctor) == null)
                        {
                            ReportMissingStateMachineAttribute(diagnostics, method, AttributeDescription.IteratorStateMachineAttribute.FullName);
                            return null;
                        }
                    }

                    previousLocals = TryGetLocalSlotMapFromMetadata(previousHandle, debugInfo);
                    if (previousLocals.IsDefault)
                    {
                        // TODO: Report error that metadata is not supported.
                        return null;
                    }
                }

                symbolMap = mapToMetadata;
            }

            return new EncVariableSlotAllocator(
                symbolMap,
                mappedMethod.SyntaxMap,
                mappedMethod.PreviousMethod,
                methodId,
                previousLocals,
                lambdaMap,
                closureMap,
                stateMachineTypeNameOpt,
                hoistedLocalSlotCount,
                hoistedLocalMap,
                awaiterSlotCount,
                awaiterMap,
                GetLambdaSyntaxFacts());
        }

        protected abstract LambdaSyntaxFacts GetLambdaSyntaxFacts();

        private void ReportMissingStateMachineAttribute(DiagnosticBag diagnostics, IMethodSymbolInternal method, string stateMachineAttributeFullName)
        {
            diagnostics.Add(MessageProvider.CreateDiagnostic(
                MessageProvider.ERR_EncUpdateFailedMissingAttribute,
                method.Locations.First(), 
                MessageProvider.GetErrorDisplayString(method),
                stateMachineAttributeFullName));
        }

        private static void MakeLambdaAndClosureMaps(
            ImmutableArray<LambdaDebugInfo> lambdaDebugInfo,
            ImmutableArray<ClosureDebugInfo> closureDebugInfo,
            out IReadOnlyDictionary<int, KeyValuePair<DebugId, int>> lambdaMap,
            out IReadOnlyDictionary<int, DebugId> closureMap)
        {
            Dictionary<int, KeyValuePair<DebugId, int>> lambdas = new Dictionary<int, KeyValuePair<DebugId, int>>(lambdaDebugInfo.Length);
            Dictionary<int, DebugId> closures = new Dictionary<int, DebugId>(closureDebugInfo.Length);

            for (int i = 0; i < lambdaDebugInfo.Length; i++)
            {
                LambdaDebugInfo lambdaInfo = lambdaDebugInfo[i];
                lambdas[lambdaInfo.SyntaxOffset] = KeyValuePair.Create(lambdaInfo.LambdaId, lambdaInfo.ClosureOrdinal);
            }

            for (int i = 0; i < closureDebugInfo.Length; i++)
            {
                ClosureDebugInfo closureInfo = closureDebugInfo[i];
                closures[closureInfo.SyntaxOffset] = closureInfo.ClosureId;
            }

            lambdaMap = lambdas;
            closureMap = closures;
        }

        private static void GetStateMachineFieldMapFromPreviousCompilation(
            ImmutableArray<EncHoistedLocalInfo> hoistedLocalSlots,
            ImmutableArray<Cci.ITypeReference> hoistedAwaiters,
            out IReadOnlyDictionary<EncHoistedLocalInfo, int> hoistedLocalMap,
            out IReadOnlyDictionary<Cci.ITypeReference, int> awaiterMap)
        {
            Dictionary<EncHoistedLocalInfo, int> hoistedLocals = new Dictionary<EncHoistedLocalInfo, int>();
            Dictionary<Cci.ITypeReference, int> awaiters = new Dictionary<Cci.ITypeReference, int>();

            for (int slotIndex = 0; slotIndex < hoistedLocalSlots.Length; slotIndex++)
            {
                EncHoistedLocalInfo slot = hoistedLocalSlots[slotIndex];
                if (slot.IsUnused)
                {
                    // Unused field.
                    continue;
                }

                hoistedLocals.Add(slot, slotIndex);
            }

            for (int slotIndex = 0; slotIndex < hoistedAwaiters.Length; slotIndex++)
            {
                Cci.ITypeReference slot = hoistedAwaiters[slotIndex];
                if (slot == null)
                {
                    // Unused awaiter.
                    continue;
                }

                awaiters.Add(slot, slotIndex);
            }

            hoistedLocalMap = hoistedLocals;
            awaiterMap = awaiters;
        }
    }
}
