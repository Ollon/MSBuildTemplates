// -----------------------------------------------------------------------
// <copyright file="VsNewSolutionDialogService.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell.Interop;
using Ollon.VisualStudio.Extensibility.Dialogs;
using Ollon.VisualStudio.Extensibility.Services;

namespace Ollon.VisualStudio.Extensibility.Implementation.Services
{
    [Export(typeof(IVsNewSolutionDialogService))]
    internal class VsNewSolutionDialogService : IVsNewSolutionDialogService
    {
        public ISolutionOptions CreateSolutionViaDialog()
        {
            NewSolutionDialogInfrastructure infra = new NewSolutionDialogFactory().Create();

            bool? result = infra.View.ShowDialog();

            if (result == true)
            {
                return infra.Model;
            }

            return null;
        }
    }
}
