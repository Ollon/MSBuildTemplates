// -----------------------------------------------------------------------
// <copyright file="VsSolution3.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ollon.VisualStudio.Extensibility.Implementation.Interop
{
    [Export(typeof(IVsSolution3))]
    internal sealed class VsSolution3 : IVsSolution3
    {
        private readonly IVsSolution3 _solution;

        public int CreateNewProjectViaDlgEx(string pszDlgTitle, string pszTemplateDir, string pszExpand, string pszSelect, string pszHelpTopic, uint cnpvdeFlags, IVsBrowseProjectLocation pBrowse)
        {
            return _solution.CreateNewProjectViaDlgEx(pszDlgTitle, pszTemplateDir, pszExpand, pszSelect, pszHelpTopic, cnpvdeFlags, pBrowse);
        }

        public int GetUniqueUINameOfProject(IVsHierarchy pHierarchy, out string pbstrUniqueName)
        {
            return _solution.GetUniqueUINameOfProject(pHierarchy, out pbstrUniqueName);
        }

        public int CheckForAndSaveDeferredSaveSolution(int fCloseSolution, string pszMessage, string pszTitle, uint grfFlags)
        {
            return _solution.CheckForAndSaveDeferredSaveSolution(fCloseSolution, pszMessage, pszTitle, grfFlags);
        }

        public int UpdateProjectFileLocationForUpgrade(string pszCurrentLocation, string pszUpgradedLocation)
        {
            return _solution.UpdateProjectFileLocationForUpgrade(pszCurrentLocation, pszUpgradedLocation);
        }

        public VsSolution3(IVsSolution3 solution)
        {
            _solution = solution;
        }

        [ImportingConstructor]
        private VsSolution3([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
            : this(serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution3)
        {
        }
    }
}
