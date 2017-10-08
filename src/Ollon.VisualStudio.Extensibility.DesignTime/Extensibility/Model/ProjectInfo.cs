// -----------------------------------------------------------------------
// <copyright file="ProjectInfo.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Ollon.VisualStudio.Extensibility.Model
{
    public class ProjectInfo
    {
        public ProjectInfo(
            string outputPath,
            string assemblyName,
            string projectDirectory,
            IReadOnlyList<ProjectItemInfo> compileItems = null,
            IReadOnlyList<ProjectItemInfo> projectReferences = null,
            IReadOnlyList<ProjectItemInfo> assemblyReferences = null,
            IReadOnlyList<ProjectItemInfo> packageReferences = null)
        {
            OutputPath = outputPath;
            AssemblyName = assemblyName;
            ProjectDirectory = projectDirectory;
            CompileItems = compileItems ?? ImmutableArray<ProjectItemInfo>.Empty;
            ProjectReferences = projectReferences ?? ImmutableArray<ProjectItemInfo>.Empty;
            AssemblyReferences = assemblyReferences ?? ImmutableArray<ProjectItemInfo>.Empty;
            PackageReferences = packageReferences ?? ImmutableArray<ProjectItemInfo>.Empty;
        }

        public string OutputPath { get; }

        public string AssemblyName { get; }

        public string ProjectDirectory { get; }

        public IReadOnlyList<ProjectItemInfo> CompileItems { get; }

        public IReadOnlyList<ProjectItemInfo> ProjectReferences { get; }

        public IReadOnlyList<ProjectItemInfo> AssemblyReferences { get; }

        public IReadOnlyList<ProjectItemInfo> PackageReferences { get; }
    }
}
