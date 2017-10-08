// -----------------------------------------------------------------------
// <copyright file="ProjectFile.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis.MSBuild;

namespace Ollon.VisualStudio.Extensibility.Model.ProjectFile
{
    internal class ProjectFile : IProjectFile
    {
        private const string CompileItemTypeName = "Compile";
        private ProjectCollection _projectCollection;
        private Project _project;
        public event ProjectChangedEventHandler ProjectChanged;

        public ProjectFile(ProjectSystem projectSystem)
        {
            switch (projectSystem)
            {
                case ProjectSystem.Legacy:
                    {
                        InitializeLegacyProjectXml();
                        GenerateAndAddLegacyConfigurationPropertyGroup();
                        break;
                    }
                case ProjectSystem.CPS:
                    {
                        InitializeCPSProjectXml();
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(projectSystem), projectSystem, null);
            }

        }


        public ProjectRootElement Xml => _project.Xml;

        private void InitializeLegacyProjectXml()
        {
            _projectCollection = new ProjectCollection();
            _project = new Project(new Dictionary<string, string>(), "12.0", _projectCollection, NewProjectFileOptions.IncludeXmlNamespace | NewProjectFileOptions.IncludeToolsVersion);
            ProjectRootElement root = _project.Xml;
            ProjectElement firstChild = root.FirstChild;
            ProjectElement lastChild = root.LastChild;
            ProjectImportElement commonImport =
                root.CreateImportElement("$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props");
            commonImport.Condition = "Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')";
            ProjectImportElement csharpImport =
                root.CreateImportElement("$(MSBuildToolsPath)\\Microsoft.CSharp.targets");
            root.InsertBeforeChild(commonImport, firstChild);
            root.InsertAfterChild(csharpImport, lastChild);
        }

        private void InitializeCPSProjectXml()
        {
            _project = new Project(NewProjectFileOptions.None);
            ProjectRootElement root = _project.Xml;
            ProjectElement firstChild = root.FirstChild;
            ProjectElement lastChild = root.LastChild;
            ProjectImportElement sdkPropsImportElement = root.CreateImportElement("Sdk.props");
            sdkPropsImportElement.Sdk = "Microsoft.NET.Sdk";
            ProjectImportElement sdkTargetsImportElement = root.CreateImportElement("Sdk.targets");
            sdkTargetsImportElement.Sdk = "Microsoft.NET.Sdk";
            root.InsertBeforeChild(sdkPropsImportElement, firstChild);
            root.InsertAfterChild(sdkTargetsImportElement, lastChild);
        }

        private void GenerateAndAddLegacyConfigurationPropertyGroup()
        {
            ProjectPropertyGroupElement group = Xml.AddPropertyGroup();
            group.AddProperty("ProjectGuidString", Guid.NewGuid().ToString("B").ToUpper());
            group.AddProperty("ProjectTypeGuids", VsProjectTypeGuids.LegacyProjectGuid);

        }

        public ProjectFile(string filePath)
        {
            MSBuildProjectLoader loader = new MSBuildProjectLoader();

            _project = loader.LoadProject(filePath);
        }

        public string FilePath { get; }

        public ProjectInfo GetProjectInfo()
        {
            return null;
        }

        public void AddDocument(string filePath)
        {
            OnProjectChanged(new ProjectChangedEventArgs(ProjectChangedKind.DocumentAdded));
        }

        public void RemoveDocument(string documentFilePath)
        {
        }

        public void AddAssemblyReference(string assemblyFilePath)
        {
        }

        public void RemoveAssemblyReference(string assemblyFilePath)
        {
        }

        public void AddPackageReference(string packageName, string packageVersion, string privateAssets = null, string publicAssets = null)
        {
        }

        public void RemovePackageReference(string packageName)
        {
        }

        public void AddProjectReference(string projectName, string projectFilePath)
        {
        }

        public void RemoveProjectReference(string projectFilePath)
        {
        }
        public void Save()
        {
            _project.Save();
        }

        public void Save(string filePath)
        {
            _project.Save(filePath);
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

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return _project.Xml.RawXml;
        }

        protected virtual void OnProjectChanged(ProjectChangedEventArgs e)
        {
            ProjectChanged?.Invoke(this, e);
        }
    }
}
