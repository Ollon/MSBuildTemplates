// -----------------------------------------------------------------------
// <copyright file="NewSolutionCommand.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
using System.IO;
using EnvDTE;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Ollon.VisualStudio.Extensibility.Input;
using Ollon.VisualStudio.Extensibility.Services;

namespace Ollon.VisualStudio.Extensibility.DesignTime.Input.Extensibility.Input
{
    [Export(typeof(DynamicCommand))]
    public class NewSolutionCommand : DynamicCommand
    {
        [ImportingConstructor]
        public NewSolutionCommand([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
            : base(serviceProvider, MenuCommands.NewSolutionCommand)
        {

        }

        [Import]
        internal IDirectoryBuildPropsWriter DirectoryBuildPropsWriter { get; private set; }

        [Import]
        internal IVsRuleSetWriter RuleSetWriter { get; private set; }

        [Import]
        internal IVsNewSolutionDialogService DialogService { get; private set; }

        [Import]
        internal IVsSolution Solution { get; private set; }


        protected override void Execute()
        {

            ISolutionOptions options = DialogService.CreateSolutionViaDialog();

            if (options != null)
            {
                Directory.CreateDirectory(options.RepositoryDirectory);
                Directory.CreateDirectory(options.SolutionDirectory);

                DirectoryBuildPropsWriter.WriteDirectoryBuildProps(options);

                RuleSetWriter.WriteRuleSet(options);

                Solution.CreateSolution(options.SolutionDirectory, options.SolutionName, CreateSolutionFlags.Overwrite);

                Solution.SaveSolutionElement(SaveSolutionOptions.ForceSave, null, 0);

                Solution.OpenSolutionFile(OpenSolutionFlags.Silent, options.SolutionFilePath);

                Project project = Solution2.AddSolutionFolder("Build");
                project.ProjectItems.AddFromFile(options.DirectoryBuildPropsPath);

            }


        }


    }
}
