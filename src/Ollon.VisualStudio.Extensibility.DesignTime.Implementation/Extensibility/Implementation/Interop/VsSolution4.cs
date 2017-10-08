// -----------------------------------------------------------------------
// <copyright file="VsSolution4.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ollon.VisualStudio.Extensibility.Implementation.Interop
{
    [Export(typeof(IVsSolution4))]
    internal sealed class VsSolution4 : IVsSolution4
    {
        private readonly IVsSolution4 _solution;

        public int WriteUserOptsFile()
        {
            return _solution.WriteUserOptsFile();
        }

        public int IsBackgroundSolutionLoadEnabled(out bool pfIsEnabled)
        {
            return _solution.IsBackgroundSolutionLoadEnabled(out pfIsEnabled);
        }

        public int EnsureProjectsAreLoaded(uint cProjects, Guid[] guidProjects, uint grfFlags)
        {
            return _solution.EnsureProjectsAreLoaded(cProjects, guidProjects, grfFlags);
        }

        public int EnsureProjectIsLoaded(ref Guid guidProject, uint grfFlags)
        {
            return _solution.EnsureProjectIsLoaded(ref guidProject, grfFlags);
        }

        public int EnsureSolutionIsLoaded(uint grfFlags)
        {
            return _solution.EnsureSolutionIsLoaded(grfFlags);
        }

        public int ReloadProject(ref Guid guidProjectID)
        {
            return _solution.ReloadProject(ref guidProjectID);
        }

        public int UnloadProject(ref Guid guidProjectID, uint dwUnloadStatus)
        {
            return _solution.UnloadProject(ref guidProjectID, dwUnloadStatus);
        }

        public VsSolution4(IVsSolution4 solution)
        {
            _solution = solution;
        }

        [ImportingConstructor]
        private VsSolution4([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
            : this(serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution4)
        {
        }
    }
}
