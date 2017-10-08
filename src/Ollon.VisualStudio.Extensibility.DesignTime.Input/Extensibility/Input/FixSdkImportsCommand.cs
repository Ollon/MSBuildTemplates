// -----------------------------------------------------------------------
// <copyright file="FixSdkImportsCommand.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Properties;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Ollon.VisualStudio.Extensibility.Input;
using Ollon.VisualStudio.Extensibility.Services;

namespace Ollon.VisualStudio.Extensibility.DesignTime.Input.Extensibility.Input
{
    [Export(typeof(DynamicCommand))]
    public class FixSdkImportsCommand : DynamicCommand
    {
        [ImportingConstructor]
        public FixSdkImportsCommand(
          [Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider) 
            : base(serviceProvider, MenuCommands.FixSdkAttributeCommand)
        {
        }

        [Import]
        internal IVsMonitorSelectionService SelectionService { get; private set; }

        [Import]
        internal IProjectXmlService ProjectXmlService { get; private set; }

        protected override void Execute()
        {
            IVsHierarchy selection = ThreadHelper.JoinableTaskFactory.Run(() => SelectionService.GetCurrentSelectionAsync(new CancellationToken()));

            if (selection is IVsBrowseObjectContext browseObject)
            {
                BrowseObject = browseObject;

                using (ProjectWriteLockReleaser access = ThreadHelper.JoinableTaskFactory.Run(async () => await Locker.WriteLockAsync()))
                {
                    string filePath = BrowseObject.UnconfiguredProject.FullPath;

                    ProjectXmlService.SetExplicitSdkImportsIfNecessaryAsync(filePath);
                }
            }
        }

        private IVsBrowseObjectContext BrowseObject { get; set; }


        
        private IProjectLockService Locker
        {
            get
            {
                return BrowseObject.UnconfiguredProject.Services.ProjectService.Services.ProjectLockService;
            }
        }


    }
}
