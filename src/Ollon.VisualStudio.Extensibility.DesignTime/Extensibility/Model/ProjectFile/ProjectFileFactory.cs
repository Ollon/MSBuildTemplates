// -----------------------------------------------------------------------
// <copyright file="ProjectFileFactory.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;

namespace Ollon.VisualStudio.Extensibility.Model.ProjectFile
{
    public static class ProjectFileFactory
    {
        private static readonly ProjectCollection _staticCollection;

        static ProjectFileFactory()
        {
            _staticCollection = new ProjectCollection(null, null, null, ToolsetDefinitionLocations.Default, 8, true);

            _staticCollection.ProjectAdded += OnProjectAdded;
        }

        private static void OnProjectAdded(object sender, ProjectCollection.ProjectAddedToProjectCollectionEventArgs e)
        {

        }


        public static ProjectRootElement CreateCPSClassLibrary(
            string assemblyName,
            string rootNamespace,
            string targetFramework,
            Guid projectGuid = default
            )
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("net");
            foreach (char c in targetFramework)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                }
            }

            if (projectGuid == default)
            {
                projectGuid = Guid.NewGuid();
            }
            ProjectRootElement root = ProjectRootElement.Create(NewProjectFileOptions.None);
            InitializeCPSClassLibraryConfigurationProperties(root, assemblyName, rootNamespace, "Library", sb.ToString());
            InitializeCPSProjectImports(root);
            return root;
        }

        public static ProjectRootElement CreateCPSConsoleProject(
            string assemblyName,
            string rootNamespace,
            string targetFramework = "net47",
            Guid projectGuid = default
        )
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("net");
            foreach (char c in targetFramework)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                }
            }

            if (projectGuid == default)
            {
                projectGuid = Guid.NewGuid();
            }
            ProjectRootElement root = ProjectRootElement.Create(NewProjectFileOptions.None);
            InitializeCPSClassLibraryConfigurationProperties(root, assemblyName, rootNamespace, "Exe", sb.ToString());
            InitializeCPSProjectImports(root);
            return root;
        }



        public static ProjectRootElement CreateLegacyVSIXProject(
            string assemblyName,
            string rootNamespace,
            string targetFramework = "v4.7",
            Guid projectGuid = default)
        {
            if (projectGuid == default)
            {
                projectGuid = Guid.NewGuid();
            }
            ProjectRootElement root = ProjectRootElement.Create(NewProjectFileOptions.IncludeAllOptions);
            InitializeLegacyClassLibraryConfigurationProperties(
                root, assemblyName, rootNamespace, "Library", targetFramework, projectGuid.ToString("B").ToUpper());
            InitializeLegacyProjectImports(root);
            InitializeLegacyExtensibilityConfigurationProperties(root);
            InitializeExtensibilityDebugParameters(root);
            InitializeVSIXProjectItems(root);
            return root;
        }

        public static ProjectRootElement CreateLegacyConsoleProject(
            string assemblyName,
            string rootNamespace,
            string targetFramework = "v4.7",
            Guid projectGuid = default)
        {
            if (projectGuid == default)
            {
                projectGuid = Guid.NewGuid();
            }
            ProjectRootElement root = ProjectRootElement.Create(NewProjectFileOptions.IncludeAllOptions);
            InitializeLegacyClassLibraryConfigurationProperties(
                root, assemblyName, rootNamespace, "Exe", targetFramework, projectGuid.ToString("B").ToUpper());
            InitializeLegacyProjectImports(root);
            return root;
        }
        public static ProjectRootElement CreateLegacyClassLibrary(
            string assemblyName,
            string rootNamespace,
            string targetFramework = "v4.7",
            Guid projectGuid = default)
        {
            if (projectGuid == default)
            {
                projectGuid = Guid.NewGuid();
            }
            ProjectRootElement root = ProjectRootElement.Create(NewProjectFileOptions.IncludeAllOptions);
            InitializeLegacyClassLibraryConfigurationProperties(
                root, assemblyName, rootNamespace, "Library", targetFramework, projectGuid.ToString("B").ToUpper());
            InitializeLegacyProjectImports(root);
            return root;
        }

        private static void InitializeLegacyProjectImports(ProjectRootElement root)
        {
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

        private static void InitializeCPSProjectImports(ProjectRootElement root)
        {
            ProjectElement firstChild = root.FirstChild;
            ProjectElement lastChild = root.LastChild;
            ProjectImportElement sdkProps =
                root.CreateImportElement("Sdk.props");
            sdkProps.Sdk = "Microsoft.NET.Sdk";
            ProjectImportElement sdkTargets =
                root.CreateImportElement("Sdk.targets");
            sdkTargets.Sdk = "Microsoft.NET.Sdk";
            root.InsertBeforeChild(sdkProps, firstChild);
            root.InsertAfterChild(sdkTargets, lastChild);
        }


        private static void InitializeLegacyExtensibilityConfigurationProperties(ProjectRootElement root)
        {
            ProjectElement firstChild = root.FirstChild;
            ProjectPropertyGroupElement group = root.CreatePropertyGroupElement();
            root.InsertBeforeChild(group, firstChild);
            group.Label = "Globals";
            group.AddProperty("EnableGlobbing", "false");
            group.AddProperty("ImportVSSDKTargets", "true");
            group.AddProperty("GeneratePkgDefFile", "false");
            group.AddProperty("DeployExtension", "true");
            group.AddProperty("DeployVSTemplates", "true");
            group.AddProperty("UseCodebase", "false");
            group.AddProperty("CreateVsixContainer", "true");
            group.AddProperty("IncludeAssemblyInVSIXContainer", "false");
            group.AddProperty("IncludeDebugSymbolsInVSIXContainer", "true");
            group.AddProperty("IncludeDebugSymbolsInLocalVSIXDeployment", "true");
            group.AddProperty("CopyBuildOutputToOutputDirectory", "false");
            group.AddProperty("CopyOutputSymbolsToOutputDirectory", "false");
            group.AddProperty("UseCommonOutputDirectory", "true");

        }

        private static void InitializeCPSClassLibraryConfigurationProperties(
            ProjectRootElement root, string assemblyName, string rootNamespace, string outputType, string targetFramework)
        {
            ProjectPropertyGroupElement group = root.AddPropertyGroup();
            group.Label = "Configuration";
            group.AddProperty("AssemblyName", assemblyName);
            group.AddProperty("RootNamespace", rootNamespace);
            group.AddProperty("OutputType", outputType);
            group.AddProperty("TargetFramework", targetFramework);
        }

        private static void InitializeLegacyVSIXConfigurationProperties(
            ProjectRootElement root, string assemblyName, string rootNamespace, string outputType, string targetFramework, string projectGuid)
        {
            ProjectPropertyGroupElement group = root.AddPropertyGroup();
            group.Label = "Configuration";
            group.AddProperty("ProjectGuid", projectGuid);
            group.AddProperty("ProjectTypeGuids", VsProjectTypeGuids.ExtensibilityFlavorProjectGuid);
            group.AddProperty("AssemblyName", assemblyName);
            group.AddProperty("RootNamespace", rootNamespace);
            group.AddProperty("OutputType", outputType);
            group.AddProperty("TargetFrameworkVersion", targetFramework);
        }
        private static void InitializeLegacyClassLibraryConfigurationProperties(
            ProjectRootElement root, string assemblyName, string rootNamespace, string outputType, string targetFramework, string projectGuid)
        {
            ProjectPropertyGroupElement group = root.AddPropertyGroup();
            group.Label = "Configuration";
            group.AddProperty("ProjectGuid", projectGuid);
            group.AddProperty("AssemblyName", assemblyName);
            group.AddProperty("RootNamespace", rootNamespace);
            group.AddProperty("OutputType", outputType);
            group.AddProperty("TargetFrameworkVersion", targetFramework);
        }

        private static void InitializeExtensibilityDebugParameters(ProjectRootElement root)
        {
            ProjectElement firstChild = root.FirstChild;
            ProjectPropertyGroupElement group = root.CreatePropertyGroupElement();
            root.InsertAfterChild(group, firstChild);
            group.AddProperty("StartAction", "Program");
            group.AddProperty("StartProgram", "$(DevEnvDir)\\devenv.exe");
            group.AddProperty("StartArguments", "/RootSuffix Exp");
        }

        private static void InitializeVSIXProjectItems(ProjectRootElement root)
        {
            ProjectElement lastChild = root.LastChild;
            ProjectItemGroupElement group = root.CreateItemGroupElement();
            root.InsertBeforeChild(group, lastChild);
            group.AddItem("None", "source.extension.vsixmanifest");
        }
    }
}
