using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using Ollon.VisualStudio.Extensibility.MSBuild;
using Ollon.VisualStudio.Extensibility.TemplateWizards.Dialogs;

namespace Ollon.VisualStudio.Extensibility.TemplateWizards
{

    /// <summary>
    /// Classic implementation of an <see cref="T:Microsoft.VisualStudio.TemplateWizard.IWizard" /> for Item Templates at the Solution 
    /// Level in Visual Studio.
    /// </summary>
    public class SolutionItemsTemplateWizard : IWizard
    {
        public EnvDTE.DTE DTE { get; private set; }

        public EnvDTE.Solution Solution { get; private set; }
        public EnvDTE.Project CurrentProject { get; private set; }

        public IMSBuildOptions Options { get; private set; }


        /// <inheritdoc />
        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {

            if (automationObject is DTE dte)
            {
                DTE = dte;
                Solution = DTE.Solution;
            }

            SolutionItemInfrastructure infra = SolutionItemFactory.Create();

            infra.ViewModel.SolutionDirectory = replacementsDictionary[ReplacementNames.SolutionDirectory];
            infra.ViewModel.SolutionName = Path.GetFileNameWithoutExtension(DTE.Solution.FullName);

            bool? result = infra.View.ShowModal();

            if (result == true)
            {
                Options = infra.Model;
            }


        }

        /// <inheritdoc />
        public void ProjectFinishedGenerating(Project project)
        {
        }

        /// <inheritdoc />
        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            CurrentProject = projectItem.ContainingProject;
        }

        /// <inheritdoc />
        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }

        /// <inheritdoc />
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        /// <inheritdoc />
        public void RunFinished()
        {
            MSBuildScriptGenerator.GenerateDirectoryBuildScripts(Options);
        }



    }
}
