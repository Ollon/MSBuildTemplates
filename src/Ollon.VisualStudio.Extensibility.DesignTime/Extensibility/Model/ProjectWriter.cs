// -----------------------------------------------------------------------
// <copyright file="ProjectWriter.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Text;

namespace Ollon.VisualStudio.Extensibility.Model
{
    public class ProjectWriter : TextWriter
    {
        private readonly Stream _stream;
        private readonly StreamWriter _writer;

        public ProjectWriter(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            _stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            _writer = new StreamWriter(_stream);
        }


        public void WriteProject(IProject project)
        {
            Directory.CreateDirectory(project.ProjectDirectory);

            GenerateProjectContent(
                project.ProjectGuidString,
                project.Name,
                project.RootNamespace,
                project.TargetFramework,
                project.ProjectTypeGuids,
                project.ProjectSystem,
                project.Type
            );

        }

        /// <summary>Writes a string followed by a line terminator to the text string or stream.</summary>
        /// <param name="value">The string to write. If <paramref name="value" /> is <see langword="null" />, only the line terminator is written. </param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void WriteLine(string value)
        {
            _writer.WriteLine(value);
        }

        public override Encoding Encoding => Encoding.UTF8;

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            using (_stream)
            using (_writer)
            {
                _writer.Flush();
                _stream.Position = 0;
                StreamReader reader = new StreamReader(_stream);
                return reader.ReadToEnd();
            }
        }

        private void GenerateProjectContent(string projectGuid, string name, string rootNamespace, string targetFramework, string projectTypeGuids, ProjectSystem projectSystem, ProjectType type)
        {
            switch (projectSystem)
            {
                case ProjectSystem.CPS:

                    string target = "net" + targetFramework.Substring(1).Replace(".", string.Empty);

                    switch (type)
                    {
                        case ProjectType.ClassLibrary:
                            WriteLine(@"<Project>");
                            WriteLine(@"");
                            WriteLine(@"  <PropertyGroup Label=""Globals"">");
                            WriteLine(@"    ");
                            WriteLine(@"  </PropertyGroup>");
                            WriteLine(@"  ");
                            WriteLine(@"  <Import Project=""Sdk.props"" Sdk=""Microsoft.NET.Sdk""/>");
                            WriteLine(@"");
                            WriteLine(@"  <PropertyGroup>");
                            WriteLine(@"    <OutputType>Library</OutputType>");
                            WriteLine($@"    <TargetFramework>{target}</TargetFramework>");
                            WriteLine($@"    <RootNamespace>{rootNamespace}</RootNamespace>");
                            WriteLine($@"    <AssemblyName>{name}</AssemblyName>");
                            WriteLine(@"  </PropertyGroup>");
                            WriteLine(@"");
                            WriteLine(@"  ");
                            WriteLine(@"  <Import Project=""Sdk.targets"" Sdk=""Microsoft.NET.Sdk""/>");
                            WriteLine(@"</Project>");
                            break;
                        case ProjectType.VSIXProject:
                            WriteLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
                            WriteLine(@"<Project>");
                            WriteLine(@"");
                            WriteLine(@"  <PropertyGroup Label=""Globals"">");
                            WriteLine(@"    <EnableGlobbing>false</EnableGlobbing>");
                            WriteLine(@"    <ImportVSSDKTargets>true</ImportVSSDKTargets>");
                            WriteLine(@"    <GeneratePkgDefFile>false</GeneratePkgDefFile>");
                            WriteLine(@"    <DeployExtension>true</DeployExtension>");
                            WriteLine(@"    <DeployVSTemplates>true</DeployVSTemplates>");
                            WriteLine(@"    <UseCodebase>false</UseCodebase>");
                            WriteLine(@"    <CreateVsixContainer>true</CreateVsixContainer>");
                            WriteLine(@"    <IncludeAssemblyInVSIXContainer>false</IncludeAssemblyInVSIXContainer>");
                            WriteLine(@"    <IncludeDebugSymbolsInVSIXContainer>true</IncludeDebugSymbolsInVSIXContainer>");
                            WriteLine(@"    <IncludeDebugSymbolsInLocalVSIXDeployment>true</IncludeDebugSymbolsInLocalVSIXDeployment>");
                            WriteLine(@"    <CopyBuildOutputToOutputDirectory>false</CopyBuildOutputToOutputDirectory>");
                            WriteLine(@"    <CopyOutputSymbolsToOutputDirectory>false</CopyOutputSymbolsToOutputDirectory>");
                            WriteLine(@"    <UseCommonOutputDirectory>true</UseCommonOutputDirectory>");
                            WriteLine(@"  </PropertyGroup>");
                            WriteLine(@"  ");
                            WriteLine(@"  <PropertyGroup>");
                            WriteLine(@"    <OutputType>Library</OutputType>");
                            WriteLine($@"    <TargetFramework>{target}</TargetFramework>");
                            WriteLine($@"    <RootNamespace>{rootNamespace}</RootNamespace>");
                            WriteLine($@"    <AssemblyName>{name}</AssemblyName>");
                            WriteLine(@"  </PropertyGroup>");
                            WriteLine(@"  ");
                            WriteLine(@"  <Import Project=""Sdk.props"" Sdk=""Microsoft.NET.Sdk""/>");
                            WriteLine(@"");
                            WriteLine(@"  <ItemGroup>");
                            WriteLine(@"    <None Include=""source.extension.vsixmanifest"" />");
                            WriteLine(@"  </ItemGroup>");
                            WriteLine(@"  ");
                            WriteLine(@"  <ItemGroup>");
                            WriteLine(@"    <Content Include=""License.txt"">");
                            WriteLine(@"      <CopyToOutputDirectory>Always</CopyToOutputDirectory>");
                            WriteLine(@"      <IncludeInVSIX>true</IncludeInVSIX>");
                            WriteLine(@"    </Content>");
                            WriteLine(@"  </ItemGroup>");
                            WriteLine(@"  ");
                            WriteLine(@"  <Import Project=""Sdk.targets"" Sdk=""Microsoft.NET.Sdk""/>");
                            WriteLine(@"</Project>");
                            break;
                        case ProjectType.Console:
                            WriteLine(@"<Project>");
                            WriteLine(@"");
                            WriteLine(@"  <PropertyGroup Label=""Globals"">");
                            WriteLine(@"    ");
                            WriteLine(@"  </PropertyGroup>");
                            WriteLine(@"  ");
                            WriteLine(@"  <PropertyGroup>");
                            WriteLine(@"    <OutputType>Exe</OutputType>");
                            WriteLine($@"    <TargetFramework>{target}</TargetFramework>");
                            WriteLine($@"    <RootNamespace>{rootNamespace}</RootNamespace>");
                            WriteLine($@"    <AssemblyName>{name}</AssemblyName>");
                            WriteLine(@"  </PropertyGroup>");
                            WriteLine(@"  ");
                            WriteLine(@"  <Import Project=""Sdk.props"" Sdk=""Microsoft.NET.Sdk""/>");
                            WriteLine(@"");
                            WriteLine(@"  <Import Project=""Sdk.targets"" Sdk=""Microsoft.NET.Sdk""/>");
                            WriteLine(@"</Project>");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }
                    break;
                case ProjectSystem.Legacy:
                    switch (type)
                    {
                        case ProjectType.ClassLibrary:

                            WriteLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
                            WriteLine(@"<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">");
                            WriteLine(@"  ");
                            WriteLine(@"  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" ");
                            WriteLine(@"          Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />");
                            WriteLine(@"  ");
                            WriteLine(@"  ");
                            WriteLine(@"  <PropertyGroup>");
                            WriteLine($@"    <ProjectGuidString>{projectGuid}</ProjectGuidString>");
                            WriteLine($@"    <ProjectTypeGuids>{projectTypeGuids}</ProjectTypeGuids>");
                            WriteLine(@"    <OutputType>Library</OutputType>");
                            WriteLine(@"    <AppDesignerFolder>Properties</AppDesignerFolder>");
                            WriteLine($@"    <TargetFrameworkVersion>{targetFramework}</TargetFrameworkVersion>");
                            WriteLine($@"    <RootNamespace>{rootNamespace}</RootNamespace>");
                            WriteLine($@"    <AssemblyName>{name}</AssemblyName>");
                            WriteLine(@"  </PropertyGroup>");
                            WriteLine(@"");
                            WriteLine(@"");
                            WriteLine(@"  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />");
                            WriteLine(@" </Project>");

                            break;
                        case ProjectType.VSIXProject:
                            WriteLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
                            WriteLine(@"<Project ToolsVersion=""15.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">");
                            WriteLine(@"  <PropertyGroup>");
                            WriteLine(@"    <ImportVSSDKTargets>true</ImportVSSDKTargets>");
                            WriteLine(@"    <GeneratePkgDefFile>false</GeneratePkgDefFile>");
                            WriteLine(@"    <DeployExtension>true</DeployExtension>");
                            WriteLine(@"    <DeployVSTemplates>true</DeployVSTemplates>");
                            WriteLine(@"    <UseCodebase>false</UseCodebase>");
                            WriteLine(@"    <CreateVsixContainer>true</CreateVsixContainer>");
                            WriteLine(@"    <IncludeAssemblyInVSIXContainer>false</IncludeAssemblyInVSIXContainer>");
                            WriteLine(@"    <IncludeDebugSymbolsInVSIXContainer>true</IncludeDebugSymbolsInVSIXContainer>");
                            WriteLine(@"    <IncludeDebugSymbolsInLocalVSIXDeployment>true</IncludeDebugSymbolsInLocalVSIXDeployment>");
                            WriteLine(@"    <CopyBuildOutputToOutputDirectory>false</CopyBuildOutputToOutputDirectory>");
                            WriteLine(@"    <CopyOutputSymbolsToOutputDirectory>false</CopyOutputSymbolsToOutputDirectory>");
                            WriteLine(@"    <UseCommonOutputDirectory>true</UseCommonOutputDirectory>");
                            WriteLine(@"  </PropertyGroup>");
                            WriteLine(@"");
                            WriteLine(@"  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />");
                            WriteLine(@"");
                            WriteLine(@"  <PropertyGroup>");
                            WriteLine($@"    <ProjectGuidString>{projectGuid}</ProjectGuidString>");
                            WriteLine(@"    <VsProjectTypeGuids>{82b43b9b-a64c-4715-b499-d71e9ca2bd60};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</VsProjectTypeGuids>");
                            WriteLine(@"    <OutputType>Library</OutputType>");
                            WriteLine($@"    <TargetFrameworkVersion>{targetFramework}</TargetFrameworkVersion>");
                            WriteLine($@"    <RootNamespace>{rootNamespace}</RootNamespace>");
                            WriteLine($@"    <AssemblyName>{name}</AssemblyName>");
                            WriteLine(@"    <TargetVsixContainerName>$(SolutionName)-VS$(VisualStudioVersionMajor).vsix</TargetVsixContainerName>");
                            WriteLine(@"  </PropertyGroup>");
                            WriteLine(@"");
                            WriteLine(@"  <ItemGroup>");
                            WriteLine(@"    <None Include=""source.extension.vsixmanifest"" />");
                            WriteLine(@"  </ItemGroup>");
                            WriteLine(@"  ");
                            WriteLine(@"  <ItemGroup>");
                            WriteLine(@"    <Content Include=""License.txt"">");
                            WriteLine(@"      <CopyToOutputDirectory>Always</CopyToOutputDirectory>");
                            WriteLine(@"      <IncludeInVSIX>true</IncludeInVSIX>");
                            WriteLine(@"    </Content>");
                            WriteLine(@"  </ItemGroup>");
                            WriteLine(@"  ");
                            WriteLine(@"  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />");
                            WriteLine(@"</Project>");
                            break;
                        case ProjectType.Console:

                            WriteLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
                            WriteLine(@"<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">");
                            WriteLine(@"  ");
                            WriteLine(@"  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" ");
                            WriteLine(@"          Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />");
                            WriteLine(@"  ");
                            WriteLine(@"  <PropertyGroup>");
                            WriteLine(@"     <OutputType>Exe</OutputType>");
                            WriteLine($@"    <ProjectGuidString>{projectGuid}</ProjectGuidString>");
                            WriteLine($@"    <ProjectTypeGuids>{projectTypeGuids}</ProjectTypeGuids>");
                            WriteLine(@"    <OutputType>Library</OutputType>");
                            WriteLine(@"    <AppDesignerFolder>Properties</AppDesignerFolder>");
                            WriteLine($@"    <TargetFrameworkVersion>{targetFramework}</TargetFrameworkVersion>");
                            WriteLine($@"    <RootNamespace>{rootNamespace}</RootNamespace>");
                            WriteLine($@"    <AssemblyName>{name}</AssemblyName>");
                            WriteLine(@"  </PropertyGroup>");
                            WriteLine(@"");
                            WriteLine(@"  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />");
                            WriteLine(@" </Project>");

                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(projectSystem), projectSystem, null);
            }
        }
    }
}
