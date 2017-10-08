// -----------------------------------------------------------------------
// <copyright file="MSBuildProjectLoader.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Build.Evaluation;
using Ollon.VisualStudio.Extensibility.Services;

namespace Ollon.VisualStudio.Extensibility.Implementation.Services
{
    [Export(typeof(IMSBuildProjectLoader))]
    public class MSBuildProjectLoader : IMSBuildProjectLoader
    {
        [ImportingConstructor]
        public MSBuildProjectLoader()
        {
            // 
        }

        public Project LoadProject(string filePath)
        {
            return GetOrCreateProject(filePath);
        }

        [Import]
        internal MSBuildProjectCollection ProjectCollection { get; private set; }

        private Project GetOrCreateProject(string filePath)
        {
            ICollection<Project> loadedProjects = ProjectCollection.GetLoadedProjects(filePath);
            return loadedProjects.Any() ? loadedProjects.First() : ProjectCollection.LoadProject(filePath);
        }
    }
}
