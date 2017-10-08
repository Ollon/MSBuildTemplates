




using System;
using System.ComponentModel.Composition;
using System.IO;
using EnvDTE;
using Microsoft.Build.Evaluation;
using Ollon.VisualStudio.Extensibility.Services;
using Project = EnvDTE.Project;
using ProjectProperty = Microsoft.Build.Evaluation.ProjectProperty;

namespace Ollon.VisualStudio.Extensibility.Implementation.Services
{
    [Export(typeof(IMSBuildOutputService))]
    public class MSBuildOutputService : IMSBuildOutputService
    {
        [Import]
        internal DTE DTE { get; private set; }

        [Import]
        internal IMSBuildProjectLoader ProjectLoader { get; private set; }

        public void OpenOutput(string propertyName)
        {
            Project project = DTE.SelectedItems.Item(1).Project;

            DirectoryInfo outputPath = GetFolderPath(project.FullName, propertyName);

            if (!outputPath.Exists)
            {
                while (!outputPath.Exists)
                {
                    outputPath = outputPath.Parent;
                }
            }

            System.Diagnostics.Process.Start("explorer.exe", outputPath.FullName);
        }

        private DirectoryInfo GetFolderPath(string projectFileFullPath, string propertyName)
        {
            Microsoft.Build.Evaluation.Project msbuildProject = ProjectLoader.LoadProject(projectFileFullPath);
            ProjectProperty property = msbuildProject.GetProperty(propertyName);
            string expandString = msbuildProject.ExpandString(property.UnevaluatedValue);
            return new DirectoryInfo(expandString);
        }
    }
}
