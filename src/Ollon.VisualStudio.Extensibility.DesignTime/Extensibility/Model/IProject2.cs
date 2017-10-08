// -----------------------------------------------------------------------
// <copyright file="IProject.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ollon.VisualStudio.Extensibility.Model
{
    public interface IProject2
    {

        Task<ProjectInfo> GetProjectInfoAsync(CancellationToken cancellationToken);

        void AddDocument(string filePath);

        void RemoveDocument(string documentFilePath);

        void AddAssemblyReference(string assemblyFilePath);

        void RemoveAssemblyReference(string assemblyFilePath);

        void AddPackageReference(string packageName, string packageVersion, string privateAssets = null, string publicAssets = null);

        void RemovePackageReference(string packageName);

        void AddProjectReference(string projectName, string projectFilePath);

        void RemoveProjectReference(string projectFilePath);
    }
}
