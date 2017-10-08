using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Ollon.VisualStudio.Extensibility.Model
{
    public static class ProjectFactory
    {
        public static readonly Func<string> GenerateGuid = () => Guid.NewGuid().ToString("B").ToUpper();

        public static IProject CreateLegacyProject(
            string name,
            string extension,
            string rootNamespace,
            string targetFramework,
            string solutionDirectory,
            ProjectType type
            )
        {
            string projectFile = Path.Combine(solutionDirectory, $"{name}\\{name}{extension}");


            return new Project(
                name,
                rootNamespace,
                targetFramework,
                ProjectSystem.Legacy,
                type,
                projectFile,
                VsProjectTypeGuids.ExtensibilityFlavorProjectGuid,
                VsProjectTypeGuids.LegacyProjectGuid,
                GenerateGuid());
        }

        public static IProject CreateCPSProject(
            string name,
            string extension,
            string rootNamespace,
            string targetFramework,
            string solutionDirectory,
            ProjectType type
        )
        {
            string projectFile = Path.Combine(solutionDirectory, $"{name}\\{name}{extension}");


            return new Project(
                name,
                rootNamespace,
                targetFramework,
                ProjectSystem.CPS,
                type,
                projectFile,
                VsProjectTypeGuids.NETStandardProjectGuid);
        }

        private static string GenerateProjectContent(
            string projectGuid,
            string name,
            string rootNamespace,
            string targetFramework,
            string projectTypeGuids,
            ProjectSystem projectSystem,
            ProjectType type)
        {
            StringBuilder sb = new StringBuilder();

            switch (projectSystem)
            {
                case ProjectSystem.CPS:

                    string target = "net" + targetFramework.Substring(1).Replace(".", string.Empty);

                    switch (type)
                    {
                        case ProjectType.ClassLibrary:
                            sb.AppendLine(@"<Project>");
                            sb.AppendLine(@"");
                            sb.AppendLine(@"  <PropertyGroup Label=""Globals"">");
                            sb.AppendLine(@"    ");
                            sb.AppendLine(@"  </PropertyGroup>");
                            sb.AppendLine(@"  ");
                            sb.AppendLine(@"  <Import Project=""Sdk.props"" Sdk=""Microsoft.NET.Sdk""/>");
                            sb.AppendLine(@"");
                            sb.AppendLine(@"  <PropertyGroup>");
                            sb.AppendLine(@"    <OutputType>Library</OutputType>");
                            sb.AppendLine($@"    <TargetFramework>{target}</TargetFramework>");
                            sb.AppendLine($@"    <RootNamespace>{rootNamespace}</RootNamespace>");
                            sb.AppendLine($@"    <AssemblyName>{name}</AssemblyName>");
                            sb.AppendLine(@"  </PropertyGroup>");
                            sb.AppendLine(@"");
                            sb.AppendLine(@"  ");
                            sb.AppendLine(@"  <Import Project=""Sdk.targets"" Sdk=""Microsoft.NET.Sdk""/>");
                            sb.AppendLine(@"</Project>");
                            break;
                        case ProjectType.VSIXProject:
                            sb.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
                            sb.AppendLine(@"<Project>");
                            sb.AppendLine(@"");
                            sb.AppendLine(@"  <PropertyGroup Label=""Globals"">");
                            sb.AppendLine(@"    <EnableGlobbing>false</EnableGlobbing>");
                            sb.AppendLine(@"    <ImportVSSDKTargets>true</ImportVSSDKTargets>");
                            sb.AppendLine(@"    <GeneratePkgDefFile>false</GeneratePkgDefFile>");
                            sb.AppendLine(@"    <DeployExtension>true</DeployExtension>");
                            sb.AppendLine(@"    <DeployVSTemplates>true</DeployVSTemplates>");
                            sb.AppendLine(@"    <UseCodebase>false</UseCodebase>");
                            sb.AppendLine(@"    <CreateVsixContainer>true</CreateVsixContainer>");
                            sb.AppendLine(@"    <IncludeAssemblyInVSIXContainer>false</IncludeAssemblyInVSIXContainer>");
                            sb.AppendLine(@"    <IncludeDebugSymbolsInVSIXContainer>true</IncludeDebugSymbolsInVSIXContainer>");
                            sb.AppendLine(@"    <IncludeDebugSymbolsInLocalVSIXDeployment>true</IncludeDebugSymbolsInLocalVSIXDeployment>");
                            sb.AppendLine(@"    <CopyBuildOutputToOutputDirectory>false</CopyBuildOutputToOutputDirectory>");
                            sb.AppendLine(@"    <CopyOutputSymbolsToOutputDirectory>false</CopyOutputSymbolsToOutputDirectory>");
                            sb.AppendLine(@"    <UseCommonOutputDirectory>true</UseCommonOutputDirectory>");
                            sb.AppendLine(@"  </PropertyGroup>");
                            sb.AppendLine(@"  ");
                            sb.AppendLine(@"  <PropertyGroup>");
                            sb.AppendLine(@"    <OutputType>Library</OutputType>");
                            sb.AppendLine($@"    <TargetFramework>{target}</TargetFramework>");
                            sb.AppendLine($@"    <RootNamespace>{rootNamespace}</RootNamespace>");
                            sb.AppendLine($@"    <AssemblyName>{name}</AssemblyName>");
                            sb.AppendLine(@"  </PropertyGroup>");
                            sb.AppendLine(@"  ");
                            sb.AppendLine(@"  <Import Project=""Sdk.props"" Sdk=""Microsoft.NET.Sdk""/>");
                            sb.AppendLine(@"");
                            sb.AppendLine(@"  <ItemGroup>");
                            sb.AppendLine(@"    <None Include=""source.extension.vsixmanifest"" />");
                            sb.AppendLine(@"  </ItemGroup>");
                            sb.AppendLine(@"  ");
                            sb.AppendLine(@"  <ItemGroup>");
                            sb.AppendLine(@"    <Content Include=""License.txt"">");
                            sb.AppendLine(@"      <CopyToOutputDirectory>Always</CopyToOutputDirectory>");
                            sb.AppendLine(@"      <IncludeInVSIX>true</IncludeInVSIX>");
                            sb.AppendLine(@"    </Content>");
                            sb.AppendLine(@"  </ItemGroup>");
                            sb.AppendLine(@"  ");
                            sb.AppendLine(@"  <Import Project=""Sdk.targets"" Sdk=""Microsoft.NET.Sdk""/>");
                            sb.AppendLine(@"</Project>");
                            break;
                        case ProjectType.Console:
                            sb.AppendLine(@"<Project>");
                            sb.AppendLine(@"");
                            sb.AppendLine(@"  <PropertyGroup Label=""Globals"">");
                            sb.AppendLine(@"    ");
                            sb.AppendLine(@"  </PropertyGroup>");
                            sb.AppendLine(@"  ");
                            sb.AppendLine(@"  <PropertyGroup>");
                            sb.AppendLine(@"    <OutputType>Exe</OutputType>");
                            sb.AppendLine($@"    <TargetFramework>{target}</TargetFramework>");
                            sb.AppendLine($@"    <RootNamespace>{rootNamespace}</RootNamespace>");
                            sb.AppendLine($@"    <AssemblyName>{name}</AssemblyName>");
                            sb.AppendLine(@"  </PropertyGroup>");
                            sb.AppendLine(@"  ");
                            sb.AppendLine(@"  <Import Project=""Sdk.props"" Sdk=""Microsoft.NET.Sdk""/>");
                            sb.AppendLine(@"");
                            sb.AppendLine(@"");
                            sb.AppendLine(@"  ");
                            sb.AppendLine(@"  <Import Project=""Sdk.targets"" Sdk=""Microsoft.NET.Sdk""/>");
                            sb.AppendLine(@"</Project>");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }
                    break;
                case ProjectSystem.Legacy:
                    switch (type)
                    {
                        case ProjectType.ClassLibrary:

                            sb.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
                            sb.AppendLine(@"<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">");
                            sb.AppendLine(@"  ");
                            sb.AppendLine(@"  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" ");
                            sb.AppendLine(@"          Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />");
                            sb.AppendLine(@"  ");
                            sb.AppendLine(@"  ");
                            sb.AppendLine(@"  <PropertyGroup>");
                            sb.AppendLine($@"    <ProjectGuidString>{projectGuid}</ProjectGuidString>");
                            sb.AppendLine($@"    <ProjectTypeGuids>{projectTypeGuids}</ProjectTypeGuids>");
                            sb.AppendLine(@"    <OutputType>Library</OutputType>");
                            sb.AppendLine(@"    <AppDesignerFolder>Properties</AppDesignerFolder>");
                            sb.AppendLine($@"    <TargetFrameworkVersion>{targetFramework}</TargetFrameworkVersion>");
                            sb.AppendLine($@"    <RootNamespace>{rootNamespace}</RootNamespace>");
                            sb.AppendLine($@"    <AssemblyName>{name}</AssemblyName>");
                            sb.AppendLine(@"  </PropertyGroup>");
                            sb.AppendLine(@"");
                            sb.AppendLine(@"");
                            sb.AppendLine(@"  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />");
                            sb.AppendLine(@" </Project>");

                            break;
                        case ProjectType.VSIXProject:
                            sb.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
                            sb.AppendLine(@"<Project ToolsVersion=""15.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">");
                            sb.AppendLine(@"  <PropertyGroup>");
                            sb.AppendLine(@"    <ImportVSSDKTargets>true</ImportVSSDKTargets>");
                            sb.AppendLine(@"    <GeneratePkgDefFile>false</GeneratePkgDefFile>");
                            sb.AppendLine(@"    <DeployExtension>true</DeployExtension>");
                            sb.AppendLine(@"    <DeployVSTemplates>true</DeployVSTemplates>");
                            sb.AppendLine(@"    <UseCodebase>false</UseCodebase>");
                            sb.AppendLine(@"    <CreateVsixContainer>true</CreateVsixContainer>");
                            sb.AppendLine(@"    <IncludeAssemblyInVSIXContainer>false</IncludeAssemblyInVSIXContainer>");
                            sb.AppendLine(@"    <IncludeDebugSymbolsInVSIXContainer>true</IncludeDebugSymbolsInVSIXContainer>");
                            sb.AppendLine(@"    <IncludeDebugSymbolsInLocalVSIXDeployment>true</IncludeDebugSymbolsInLocalVSIXDeployment>");
                            sb.AppendLine(@"    <CopyBuildOutputToOutputDirectory>false</CopyBuildOutputToOutputDirectory>");
                            sb.AppendLine(@"    <CopyOutputSymbolsToOutputDirectory>false</CopyOutputSymbolsToOutputDirectory>");
                            sb.AppendLine(@"    <UseCommonOutputDirectory>true</UseCommonOutputDirectory>");
                            sb.AppendLine(@"  </PropertyGroup>");
                            sb.AppendLine(@"");
                            sb.AppendLine(@"  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />");
                            sb.AppendLine(@"");
                            sb.AppendLine(@"  <PropertyGroup>");
                            sb.AppendLine($@"    <ProjectGuidString>{projectGuid}</ProjectGuidString>");
                            sb.AppendLine(@"    <VsProjectTypeGuids>{82b43b9b-a64c-4715-b499-d71e9ca2bd60};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</VsProjectTypeGuids>");
                            sb.AppendLine(@"    <OutputType>Library</OutputType>");
                            sb.AppendLine($@"    <TargetFrameworkVersion>{targetFramework}</TargetFrameworkVersion>");
                            sb.AppendLine($@"    <RootNamespace>{rootNamespace}</RootNamespace>");
                            sb.AppendLine($@"    <AssemblyName>{name}</AssemblyName>");
                            sb.AppendLine(@"    <TargetVsixContainerName>$(SolutionName)-VS$(VisualStudioVersionMajor).vsix</TargetVsixContainerName>");
                            sb.AppendLine(@"  </PropertyGroup>");
                            sb.AppendLine(@"");
                            sb.AppendLine(@"  <ItemGroup>");
                            sb.AppendLine(@"    <None Include=""source.extension.vsixmanifest"" />");
                            sb.AppendLine(@"  </ItemGroup>");
                            sb.AppendLine(@"  ");
                            sb.AppendLine(@"  <ItemGroup>");
                            sb.AppendLine(@"    <Content Include=""License.txt"">");
                            sb.AppendLine(@"      <CopyToOutputDirectory>Always</CopyToOutputDirectory>");
                            sb.AppendLine(@"      <IncludeInVSIX>true</IncludeInVSIX>");
                            sb.AppendLine(@"    </Content>");
                            sb.AppendLine(@"  </ItemGroup>");
                            sb.AppendLine(@"  ");
                            sb.AppendLine(@"  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />");
                            sb.AppendLine(@"</Project>");
                            break;
                        case ProjectType.Console:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(projectSystem), projectSystem, null);
            }

            return sb.ToString();
        }

        [DebuggerDisplay("Project = {Name}")]
        private class Project : IProject
        {

            public Project(
                string name,
                string rootNamespace,
                string targetFramework,
                ProjectSystem projectSystem,
                ProjectType type,
                string filePath,
                string projectTypeGuids = null,
                string projectFlavorGuid = null,
                string projectGuid = null)
            {
                Name = name;
                RootNamespace = rootNamespace;
                TargetFramework = targetFramework;
                ProjectSystem = projectSystem;
                Type = type;
                FilePath = filePath;
                ProjectFlavorGuid = projectFlavorGuid ?? VsProjectTypeGuids.ExtensibilityFlavorProjectGuid;
                ProjectTypeGuids = projectTypeGuids ?? VsProjectTypeGuids.LegacyProjectGuid;
                ProjectGuidString = projectGuid ?? GenerateGuid();
            }

            public string Name { get; }
            public string RootNamespace { get; }
            public string TargetFramework { get; }
            public ProjectSystem ProjectSystem { get; }

            public string ProjectTypeGuids { get; }

            public ProjectType Type { get; }
            public string FilePath { get; }
            public string ProjectDirectory => Path.GetDirectoryName(FilePath);

            public string UniqueName => $"{Name}\\{Name}{Path.GetExtension(FilePath)}";

            public Guid ProjectGuid => new Guid(ProjectGuidString);

            public string ProjectFlavorGuid { get; }
            public string ProjectGuidString { get; }

            public string GetText()
            {
                return GenerateProjectContent(
                    ProjectGuidString,
                    Name,
                    RootNamespace,
                    TargetFramework,
                    ProjectFlavorGuid,
                    ProjectSystem,
                    Type);
            }



            public void WriteTo(ProjectWriter writer)
            {
                writer.WriteProject(this);
            }
        }
    }
}
