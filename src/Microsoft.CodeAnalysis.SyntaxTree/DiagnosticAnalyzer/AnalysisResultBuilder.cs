﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.Diagnostics.Telemetry;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    /// <summary>
    /// Stores the results of analyzer execution:
    /// 1. Local and non-local diagnostics, per-analyzer.
    /// 2. Analyzer execution times, if requested.
    /// </summary>
    internal sealed class AnalysisResultBuilder
    {
        private readonly object _gate = new object();
        private readonly Dictionary<DiagnosticAnalyzer, TimeSpan> _analyzerExecutionTimeOpt;
        private readonly HashSet<DiagnosticAnalyzer> _completedAnalyzers;
        private readonly Dictionary<DiagnosticAnalyzer, AnalyzerActionCounts> _analyzerActionCounts;

        private Dictionary<SyntaxTree, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>> _localSemanticDiagnosticsOpt = null;
        private Dictionary<SyntaxTree, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>> _localSyntaxDiagnosticsOpt = null;
        private Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder> _nonLocalDiagnosticsOpt = null;
        
        internal AnalysisResultBuilder(bool logAnalyzerExecutionTime, ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            _analyzerExecutionTimeOpt = logAnalyzerExecutionTime ? CreateAnalyzerExecutionTimeMap(analyzers) : null;
            _completedAnalyzers = new HashSet<DiagnosticAnalyzer>();
            _analyzerActionCounts = new Dictionary<DiagnosticAnalyzer, AnalyzerActionCounts>(analyzers.Length);
        }

        private static Dictionary<DiagnosticAnalyzer, TimeSpan> CreateAnalyzerExecutionTimeMap(ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            Dictionary<DiagnosticAnalyzer, TimeSpan> map = new Dictionary<DiagnosticAnalyzer, TimeSpan>(analyzers.Length);
            foreach (DiagnosticAnalyzer analyzer in analyzers)
            {
                map[analyzer] = default(TimeSpan);
            }

            return map;
        }

        public TimeSpan GetAnalyzerExecutionTime(DiagnosticAnalyzer analyzer)
        {
            Debug.Assert(_analyzerExecutionTimeOpt != null);

            lock (_gate)
            {
                return _analyzerExecutionTimeOpt[analyzer];
            }
        }

        internal ImmutableArray<DiagnosticAnalyzer> GetPendingAnalyzers(ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            lock (_gate)
            {
                ArrayBuilder<DiagnosticAnalyzer> builder = null;
                foreach (DiagnosticAnalyzer analyzer in analyzers)
                {
                    if (!_completedAnalyzers.Contains(analyzer))
                    {
                        builder = builder ?? ArrayBuilder<DiagnosticAnalyzer>.GetInstance();
                        builder.Add(analyzer);
                    }
                }

                return builder != null ? builder.ToImmutableAndFree() : ImmutableArray<DiagnosticAnalyzer>.Empty;
            }
        }

        internal void StoreAnalysisResult(AnalysisScope analysisScope, AnalyzerDriver driver, Compilation compilation, Func<DiagnosticAnalyzer, AnalyzerActionCounts> getAnalyzerActionCounts, bool fullAnalysisResultForAnalyzersInScope)
        {
            Debug.Assert(!fullAnalysisResultForAnalyzersInScope || analysisScope.FilterTreeOpt == null, "Full analysis result cannot come from partial (tree) analysis.");

            foreach (DiagnosticAnalyzer analyzer in analysisScope.Analyzers)
            {
                // Dequeue reported analyzer diagnostics from the driver and store them in our maps.
                ImmutableArray<Diagnostic> syntaxDiagnostics = driver.DequeueLocalDiagnostics(analyzer, syntax: true, compilation: compilation);
                ImmutableArray<Diagnostic> semanticDiagnostics = driver.DequeueLocalDiagnostics(analyzer, syntax: false, compilation: compilation);
                ImmutableArray<Diagnostic> compilationDiagnostics = driver.DequeueNonLocalDiagnostics(analyzer, compilation);

                lock (_gate)
                {
                    if (_completedAnalyzers.Contains(analyzer))
                    {
                        // Already stored full analysis result for this analyzer.
                        continue;
                    }

                    if (syntaxDiagnostics.Length > 0 || semanticDiagnostics.Length > 0 || compilationDiagnostics.Length > 0 || fullAnalysisResultForAnalyzersInScope)
                    {
                        UpdateLocalDiagnostics_NoLock(analyzer, syntaxDiagnostics, fullAnalysisResultForAnalyzersInScope, ref _localSyntaxDiagnosticsOpt);
                        UpdateLocalDiagnostics_NoLock(analyzer, semanticDiagnostics, fullAnalysisResultForAnalyzersInScope, ref _localSemanticDiagnosticsOpt);
                        UpdateNonLocalDiagnostics_NoLock(analyzer, compilationDiagnostics, fullAnalysisResultForAnalyzersInScope);
                    }

                    if (_analyzerExecutionTimeOpt != null)
                    {
                        TimeSpan timeSpan = driver.ResetAnalyzerExecutionTime(analyzer);
                        _analyzerExecutionTimeOpt[analyzer] = fullAnalysisResultForAnalyzersInScope ?
                            timeSpan :
                            _analyzerExecutionTimeOpt[analyzer] + timeSpan;
                    }

                    if (!_analyzerActionCounts.ContainsKey(analyzer))
                    {
                        _analyzerActionCounts.Add(analyzer, getAnalyzerActionCounts(analyzer));
                    }

                    if (fullAnalysisResultForAnalyzersInScope)
                    {
                        _completedAnalyzers.Add(analyzer);
                    }
                }
            }
        }

        private void UpdateLocalDiagnostics_NoLock(
            DiagnosticAnalyzer analyzer,
            ImmutableArray<Diagnostic> diagnostics,
            bool overwrite,
            ref Dictionary<SyntaxTree, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>> lazyLocalDiagnostics)
        {
            if (diagnostics.IsEmpty)
            {
                return;
            }

            lazyLocalDiagnostics = lazyLocalDiagnostics ?? new Dictionary<SyntaxTree, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>>();

            foreach (IGrouping<SyntaxTree, Diagnostic> diagsByTree in diagnostics.GroupBy(d => d.Location.SourceTree))
            {
                SyntaxTree tree = diagsByTree.Key;

                if (!lazyLocalDiagnostics.TryGetValue(tree, out Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder> allDiagnostics))
                {
                    allDiagnostics = new Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>();
                    lazyLocalDiagnostics[tree] = allDiagnostics;
                }

                if (!allDiagnostics.TryGetValue(analyzer, out ImmutableArray<Diagnostic>.Builder analyzerDiagnostics))
                {
                    analyzerDiagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
                    allDiagnostics[analyzer] = analyzerDiagnostics;
                }

                if (overwrite)
                {
                    analyzerDiagnostics.Clear();
                }

                analyzerDiagnostics.AddRange(diagsByTree);
            }
        }

        private void UpdateNonLocalDiagnostics_NoLock(DiagnosticAnalyzer analyzer, ImmutableArray<Diagnostic> diagnostics, bool overwrite)
        {
            if (diagnostics.IsEmpty)
            {
                return;
            }

            _nonLocalDiagnosticsOpt = _nonLocalDiagnosticsOpt ?? new Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>();

            if (!_nonLocalDiagnosticsOpt.TryGetValue(analyzer, out ImmutableArray<Diagnostic>.Builder currentDiagnostics))
            {
                currentDiagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
                _nonLocalDiagnosticsOpt[analyzer] = currentDiagnostics;
            }

            if (overwrite)
            {
                currentDiagnostics.Clear();
            }

            currentDiagnostics.AddRange(diagnostics);
        }

        internal ImmutableArray<Diagnostic> GetDiagnostics(AnalysisScope analysisScope, bool getLocalDiagnostics, bool getNonLocalDiagnostics)
        {
            lock (_gate)
            {
                return GetDiagnostics_NoLock(analysisScope, getLocalDiagnostics, getNonLocalDiagnostics);
            }
        }

        private ImmutableArray<Diagnostic> GetDiagnostics_NoLock(AnalysisScope analysisScope, bool getLocalDiagnostics, bool getNonLocalDiagnostics)
        {
            Debug.Assert(getLocalDiagnostics || getNonLocalDiagnostics);

            ImmutableArray<Diagnostic>.Builder builder = ImmutableArray.CreateBuilder<Diagnostic>();
            if (getLocalDiagnostics)
            {
                if (!analysisScope.IsTreeAnalysis)
                {
                    AddAllLocalDiagnostics_NoLock(_localSyntaxDiagnosticsOpt, analysisScope, builder);
                    AddAllLocalDiagnostics_NoLock(_localSemanticDiagnosticsOpt, analysisScope, builder);
                }
                else if (analysisScope.IsSyntaxOnlyTreeAnalysis)
                {
                    AddLocalDiagnosticsForPartialAnalysis_NoLock(_localSyntaxDiagnosticsOpt, analysisScope, builder);
                }
                else
                {
                    AddLocalDiagnosticsForPartialAnalysis_NoLock(_localSemanticDiagnosticsOpt, analysisScope, builder);
                }
            }

            if (getNonLocalDiagnostics && _nonLocalDiagnosticsOpt != null)
            {
                AddDiagnostics_NoLock(_nonLocalDiagnosticsOpt, analysisScope, builder);
            }

            return builder.ToImmutableArray();
        }

        private static void AddAllLocalDiagnostics_NoLock(
            Dictionary<SyntaxTree, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>> localDiagnostics,
            AnalysisScope analysisScope,
            ImmutableArray<Diagnostic>.Builder builder)
        {
            if (localDiagnostics != null)
            {
                foreach (Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder> localDiagsByTree in localDiagnostics.Values)
                {
                    AddDiagnostics_NoLock(localDiagsByTree, analysisScope, builder);
                }
            }
        }

        private static void AddLocalDiagnosticsForPartialAnalysis_NoLock(
            Dictionary<SyntaxTree, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>> localDiagnostics,
            AnalysisScope analysisScope,
            ImmutableArray<Diagnostic>.Builder builder)
        {
            if (localDiagnostics != null && localDiagnostics.TryGetValue(analysisScope.FilterTreeOpt, out Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder> diagnosticsForTree))
            {
                AddDiagnostics_NoLock(diagnosticsForTree, analysisScope, builder);
            }
        }

        private static void AddDiagnostics_NoLock(
            Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder> diagnostics,
            AnalysisScope analysisScope,
            ImmutableArray<Diagnostic>.Builder builder)
        {
            Debug.Assert(diagnostics != null);

            foreach (DiagnosticAnalyzer analyzer in analysisScope.Analyzers)
            {
                if (diagnostics.TryGetValue(analyzer, out ImmutableArray<Diagnostic>.Builder diagnosticsByAnalyzer))
                {
                    builder.AddRange(diagnosticsByAnalyzer);
                }
            }
        }

        internal AnalysisResult ToAnalysisResult(ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ImmutableDictionary<SyntaxTree, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>> localSyntaxDiagnostics;
            ImmutableDictionary<SyntaxTree, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>> localSemanticDiagnostics;
            ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>> nonLocalDiagnostics;

            ImmutableHashSet<DiagnosticAnalyzer> analyzersSet = analyzers.ToImmutableHashSet();
            lock (_gate)
            {
                localSyntaxDiagnostics = GetImmutable(analyzersSet, _localSyntaxDiagnosticsOpt);
                localSemanticDiagnostics = GetImmutable(analyzersSet, _localSemanticDiagnosticsOpt);
                nonLocalDiagnostics = GetImmutable(analyzersSet, _nonLocalDiagnosticsOpt);
            }

            cancellationToken.ThrowIfCancellationRequested();
            ImmutableDictionary<DiagnosticAnalyzer, AnalyzerTelemetryInfo> analyzerTelemetryInfo = GetTelemetryInfo(analyzers, cancellationToken);
            return new AnalysisResult(analyzers, localSyntaxDiagnostics, localSemanticDiagnostics, nonLocalDiagnostics, analyzerTelemetryInfo);
        }

        private static ImmutableDictionary<SyntaxTree, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>> GetImmutable(
            ImmutableHashSet<DiagnosticAnalyzer> analyzers,
            Dictionary<SyntaxTree, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>> localDiagnosticsOpt)
        {
            if (localDiagnosticsOpt == null)
            {
                return ImmutableDictionary<SyntaxTree, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>>.Empty;
            }

            ImmutableDictionary<SyntaxTree, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>>.Builder builder = ImmutableDictionary.CreateBuilder<SyntaxTree, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>>();
            ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>.Builder perTreeBuilder = ImmutableDictionary.CreateBuilder<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>();

            foreach (KeyValuePair<SyntaxTree, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>> diagnosticsByTree in localDiagnosticsOpt)
            {
                SyntaxTree tree = diagnosticsByTree.Key;
                foreach (KeyValuePair<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder> diagnosticsByAnalyzer in diagnosticsByTree.Value)
                {
                    if (analyzers.Contains(diagnosticsByAnalyzer.Key))
                    {
                        perTreeBuilder.Add(diagnosticsByAnalyzer.Key, diagnosticsByAnalyzer.Value.ToImmutable());
                    }
                }

                builder.Add(tree, perTreeBuilder.ToImmutable());
                perTreeBuilder.Clear();
            }

            return builder.ToImmutable();
        }

        private static ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>> GetImmutable(
            ImmutableHashSet<DiagnosticAnalyzer> analyzers,
            Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder> nonLocalDiagnosticsOpt)
        {
            if (nonLocalDiagnosticsOpt == null)
            {
                return ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>.Empty;
            }

            ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>.Builder builder = ImmutableDictionary.CreateBuilder<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>();
            foreach (KeyValuePair<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder> diagnosticsByAnalyzer in nonLocalDiagnosticsOpt)
            {
                if (analyzers.Contains(diagnosticsByAnalyzer.Key))
                {
                    builder.Add(diagnosticsByAnalyzer.Key, diagnosticsByAnalyzer.Value.ToImmutable());
                }
            }

            return builder.ToImmutable();
        }

        private ImmutableDictionary<DiagnosticAnalyzer, AnalyzerTelemetryInfo> GetTelemetryInfo(
            ImmutableArray<DiagnosticAnalyzer> analyzers,
            CancellationToken cancellationToken)
        {
            ImmutableDictionary<DiagnosticAnalyzer, AnalyzerTelemetryInfo>.Builder builder = ImmutableDictionary.CreateBuilder<DiagnosticAnalyzer, AnalyzerTelemetryInfo>();

            lock (_gate)
            {
                foreach (DiagnosticAnalyzer analyzer in analyzers)
                {
                    AnalyzerActionCounts actionCounts = _analyzerActionCounts[analyzer];
                    TimeSpan executionTime = _analyzerExecutionTimeOpt != null ? _analyzerExecutionTimeOpt[analyzer] : default(TimeSpan);
                    AnalyzerTelemetryInfo telemetryInfo = new AnalyzerTelemetryInfo(actionCounts, executionTime);
                    builder.Add(analyzer, telemetryInfo);
                }
            }

            return builder.ToImmutable();
        }
    }
}
