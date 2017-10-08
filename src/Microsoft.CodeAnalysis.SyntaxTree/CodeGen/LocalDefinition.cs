
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CodeGen
{
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    internal sealed class LocalDefinition : Cci.ILocalDefinition
    {

        private readonly ILocalSymbol _symbolOpt;

        private readonly string _nameOpt;

        private readonly Cci.ITypeReference _type;

        private readonly LocalSlotConstraints _constraints;

        private readonly int _slot;

        private readonly LocalSlotDebugInfo _slotInfo;

        private readonly LocalVariableAttributes _pdbAttributes;

        private readonly ImmutableArray<TypedConstant> _dynamicTransformFlags;

        private readonly ImmutableArray<TypedConstant> _tupleElementNames;

        public LocalDefinition(
    ILocalSymbol symbolOpt,
    string nameOpt,
    Cci.ITypeReference type,
    int slot,
    SynthesizedLocalKind synthesizedKind,
    LocalDebugId id,
    LocalVariableAttributes pdbAttributes,
    LocalSlotConstraints constraints,
    ImmutableArray<TypedConstant> dynamicTransformFlags,
    ImmutableArray<TypedConstant> tupleElementNames)
        {
            _symbolOpt = symbolOpt;
            _nameOpt = nameOpt;
            _type = type;
            _slot = slot;
            _slotInfo = new LocalSlotDebugInfo(synthesizedKind, id);
            _pdbAttributes = pdbAttributes;
            _dynamicTransformFlags = dynamicTransformFlags.NullToEmpty();
            _tupleElementNames = tupleElementNames.NullToEmpty();
            _constraints = constraints;
        }

        internal string GetDebuggerDisplay()
            => $"{_slot}: {_nameOpt ?? "<unnamed>"} ({_type})";

        public ILocalSymbol SymbolOpt => _symbolOpt;

        public Location Location
        {
            get
            {
                ISymbol symbol = _symbolOpt as ISymbol;
                if (symbol != null)
                {
                    ImmutableArray<Location> locations = symbol.Locations;
                    if (!locations.IsDefaultOrEmpty)
                    {
                        return locations[0];
                    }
                }
                return Location.None;
            }
        }

        public int SlotIndex => _slot;

        public MetadataConstant CompileTimeValue
        {
            get { throw ExceptionUtilities.Unreachable; }
        }

        public ImmutableArray<Cci.ICustomModifier> CustomModifiers
            => ImmutableArray<Cci.ICustomModifier>.Empty;

        public bool IsConstant
        {
            get { throw ExceptionUtilities.Unreachable; }
        }

        public bool IsModified => false;

        public LocalSlotConstraints Constraints => _constraints;

        public bool IsPinned
            => (_constraints & LocalSlotConstraints.Pinned) != 0;

        public bool IsReference
            => (_constraints & LocalSlotConstraints.ByRef) != 0;

        public LocalVariableAttributes PdbAttributes => _pdbAttributes;

        public ImmutableArray<TypedConstant> DynamicTransformFlags => _dynamicTransformFlags;

        public ImmutableArray<TypedConstant> TupleElementNames => _tupleElementNames;

        public Cci.ITypeReference Type => _type;

        public string Name => _nameOpt;

        public byte[] Signature => null;

        public LocalSlotDebugInfo SlotInfo => _slotInfo;
    }
}
