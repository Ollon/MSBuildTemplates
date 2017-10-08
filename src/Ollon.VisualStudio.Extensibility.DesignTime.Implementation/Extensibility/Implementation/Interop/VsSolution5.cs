// -----------------------------------------------------------------------
// <copyright file="VsSolution5.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ollon.VisualStudio.Extensibility.Implementation.Interop
{
    [Export(typeof(IVsSolution5))]
    internal sealed class VsSolution5 : IVsSolution5
    {
        private readonly IVsSolution5 _solution;

        public void ResolveFaultedProjects(uint cHierarchies, IVsHierarchy[] rgHierarchies, IVsPropertyBag pProjectFaultResolutionContext, out uint pcResolved, out uint pcFailed)
        {
            _solution.ResolveFaultedProjects(cHierarchies, rgHierarchies, pProjectFaultResolutionContext, out pcResolved, out pcFailed);
        }

        public Guid GetGuidOfProjectFile(string pszProjectFile)
        {
            return _solution.GetGuidOfProjectFile(pszProjectFile);
        }

        public VsSolution5(IVsSolution5 solution)
        {
            _solution = solution;
        }

        [ImportingConstructor]
        private VsSolution5([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
            : this(serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution5)
        {
        }
    }
}
