// -----------------------------------------------------------------------
// <copyright file="DirectoryBuildPropsWriter.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.Composition;
using System.IO;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Ollon.VisualStudio.Extensibility.Services;
using Ollon.VisualStudio.Extensibility.StrongName;
using Ollon.VisualStudio.Extensibility.Utilities;

namespace Ollon.VisualStudio.Extensibility.Implementation.Services
{
    [Export(typeof(IDirectoryBuildPropsWriter))]
    internal class DirectoryBuildPropsWriter : IDirectoryBuildPropsWriter
    {

        [Import]
        internal IStrongNameKeyGenerator StrongNameKeyGenerator { get; private set; }

        public void WriteDirectoryBuildProps(ISolutionOptions options)
        {
            string strongNameKeyDirectory = Path.Combine(options.RepositoryDirectory, "build\\strong name keys\\");

            string strongNameKeyFile = Path.Combine(strongNameKeyDirectory, $"{options.SolutionName}SharedKey.snk");

            Directory.CreateDirectory(strongNameKeyDirectory);


            ProjectRootElement root = ProjectRootElement.Create(NewProjectFileOptions.None);

            ProjectPropertyGroupElement p0 = root.AddPropertyGroup();
            p0.AddProperty("MSBuildAllProjects", "$(MSBuildAllProjects);$(MSBuildThisFileFullPath)");

            ProjectPropertyGroupElement p1 = root.AddPropertyGroup();
            p1.AddDefaultProperty("DisableStandardFrameworkResolution", "false");
            p1.AddDefaultProperty("AppendTargetFrameworkToOutputDirectory", "false");

            ProjectPropertyGroupElement p3 = root.AddPropertyGroup();
            p3.AddProperty("SolutionDir", "$(MSBuildThisFileDirectory)");
            p3.AddProperty("RepositoryDirectory", "$([System.IO.Path]::GetFullPath('$(SolutionDir)..\\'))");
            p3.AddDefaultProperty("OutputPath", @"$([System.IO.Path]::GetFullPath('$(RepositoryDirectory)bin\$(Configuration)\$(MSBuildProjectName)\'))");
            p3.AddDefaultProperty("IntermediateOutputPath", @"$([System.IO.Path]::GetFullPath('$(RepositoryDirectory)bin\obj\$(MSBuildProjectName)\$(Configuration)\'))");


            StrongNameKeyInfo snk = StrongNameKeyManager.GenerateStrongNameKeyInfo();
            ProjectPropertyGroupElement signingProperties = root.AddPropertyGroup();
            signingProperties.AddDefaultProperty("SignAssembly", "true");
            signingProperties.AddDefaultProperty("AssemblyOriginatorKeyFile", "$(RepositoryDirectory)build\\strong name keys\\$(SolutionName)SharedKey.snk");
            signingProperties.AddDefaultProperty("PublicKey", snk.PublicKey);
            signingProperties.AddDefaultProperty("PublicKeyToken", snk.PublicKeyToken);

            File.WriteAllBytes(strongNameKeyFile, snk.RawBytes);

            root.Save(options.DirectoryBuildPropsPath);

        }
    }
}
