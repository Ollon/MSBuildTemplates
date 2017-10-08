// -----------------------------------------------------------------------
// <copyright file="BuildProject.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;

namespace Ollon.VisualStudio.Extensibility.Model
{
    internal class BuildProject : IProject2
    {
        private const string CompileItemTypeName = "Compile";
        private readonly ProjectFileLoader _factory;
        private readonly Project _project;

        public BuildProject(string filePath, ProjectFileLoader factory)
        {
            _factory = factory;
            _project = factory.ProjectCollection.GetLoadedProjects(filePath).Any() ?
                factory.ProjectCollection.LoadedProjects.FirstOrDefault(p => p.ProjectFileLocation.File == filePath) :
                factory.ProjectCollection.LoadProject(filePath);
        }

        public string FilePath => _project.ProjectFileLocation.File;

        public string ProjectDirectory { get; }

        public string UniqueName { get; }

        public string ProjectGuidString { get; }

        public string ProjectFlavorGuid { get; }

        public Guid ProjectGuid { get; }

        public string ProjectTypeGuids { get; }

        public ProjectType Type { get; }

        public string Name { get; }

        public string RootNamespace { get; }

        public ProjectSystem ProjectSystem { get; }

        public string TargetFramework { get; }

        public void WriteTo(ProjectWriter writer)
        {
        }

        public Task<ProjectInfo> GetProjectInfoAsync(CancellationToken cancellationToken)
        {
            string outputFilePath = _project.GetPropertyValue("TargetPath");
            string assemblyName = _project.GetPropertyValue("AssemblyName");
            string projectDirectory = _project.GetPropertyValue("MSBuildProjectDirectory");
            return Task.FromResult(
                new ProjectInfo(
                    outputFilePath,
                    assemblyName,
                    projectDirectory,
                    GetItems("Compile"),
                    GetItems("ProjectReference"),
                    GetItems("Reference"),
                    GetItems("PackageReference")));
        }

        private IReadOnlyList<ProjectItemInfo> GetItems(string itemType)
        {
            List<ProjectItemInfo> list = new List<ProjectItemInfo>();
            foreach (Microsoft.Build.Evaluation.ProjectItem projectItem in _project.Items)
            {
                if (projectItem.ItemType.Equals(itemType, StringComparison.Ordinal))
                {
                    list.Add(
                        new ProjectItemInfo(
                            projectItem.ItemType,
                            projectItem.UnevaluatedInclude,
                            projectItem.Metadata.ToImmutableDictionary(m => m.Name, m => m.EvaluatedValue)));
                }
            }
            return list;
        }

        private string MakeRelative(string filePath)
        {
            string projectDirectoryPath = _project.GetPropertyValue("MSBuildProjectDirectory");
            return PathUtilities.MakeRelative(projectDirectoryPath, filePath);
        }

        private Microsoft.Build.Evaluation.ProjectItem GetItem(string itemType, string unevaluatedInclude)
        {
            Microsoft.Build.Evaluation.ProjectItem foundItem = null;
            foreach (Microsoft.Build.Evaluation.ProjectItem projectItem in _project.Items)
            {
                if (!projectItem.ItemType.Equals(itemType, StringComparison.Ordinal) ||
                    !projectItem.UnevaluatedInclude.Equals(unevaluatedInclude, StringComparison.Ordinal))
                {
                    continue;
                }
                foundItem = projectItem;
                break;
            }
            return foundItem;
        }

        private void RemoveItem(string itemType, string unevalueatedInclude)
        {
            Microsoft.Build.Evaluation.ProjectItem item = GetItem(itemType, unevalueatedInclude);
            _project.RemoveItem(item);
        }

        public void AddDocument(string filePath)
        {
            _project.AddItem(CompileItemTypeName, MakeRelative(filePath));
        }

        public void RemoveDocument(string documentFilePath)
        {
            string relativePath = MakeRelative(documentFilePath);
            Microsoft.Build.Evaluation.ProjectItem item = GetItem("Compile", relativePath);
            _project.RemoveItem(item);
        }

        public void AddAssemblyReference(string assemblyFilePath)
        {
            _project.AddItem(
                "Reference",
                Path.GetFileNameWithoutExtension(assemblyFilePath),
                ImmutableDictionary<string, string>.Empty.Add("HintPath", MakeRelative(assemblyFilePath)));
        }

        public void RemoveAssemblyReference(string assemblyFilePath)
        {
            RemoveItem("Reference", assemblyFilePath);
        }

        public void AddPackageReference(string packageName, string packageVersion, string privateAssets = null, string publicAssets = null)
        {
            Microsoft.Build.Evaluation.ProjectItem projectItem = _project.AddItem("PackageReference", packageName).First();
            ProjectMetadataElement packageVersionElement = projectItem.Xml.AddMetadata("Version", packageVersion);
            packageVersionElement.ExpressedAsAttribute = true;
        }

        public void RemovePackageReference(string packageName)
        {
            RemoveItem("PackageReference", packageName);
        }

        public void AddProjectReference(string projectName, string projectFilePath)
        {
            Microsoft.Build.Evaluation.ProjectItem reference = _project.AddItem("ProjectReference", PathUtilities.MakeRelative(FilePath, projectFilePath)).First();
            ProjectMetadataElement projectElement = reference.Xml.AddMetadata("Name", projectName);
        }

        public void RemoveProjectReference(string projectFilePath)
        {
            Microsoft.Build.Evaluation.ProjectItem reference = GetItem("ProjectReference", PathUtilities.MakeRelative(FilePath, projectFilePath));
            _project.RemoveItem(reference);
        }

        public void Save()
        {
            _project.Save();
        }

        public void Save(string filePath)
        {
            _project.Save(filePath);
        }
    }
}
