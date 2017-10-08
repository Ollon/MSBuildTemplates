// -----------------------------------------------------------------------
// <copyright file="IProjectFile.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Ollon.VisualStudio.Extensibility.Model.ProjectFile
{
    public interface IProjectFile
    {
        event ProjectChangedEventHandler ProjectChanged;

        string FilePath { get; }
        ProjectInfo GetProjectInfo();
        void AddDocument(string filePath);
        void RemoveDocument(string documentFilePath);
        void AddAssemblyReference(string assemblyFilePath);
        void RemoveAssemblyReference(string assemblyFilePath);
        void AddPackageReference(string packageName, string packageVersion, string privateAssets = null, string publicAssets = null);
        void RemovePackageReference(string packageName);
        void AddProjectReference(string projectName, string projectFilePath);
        void RemoveProjectReference(string projectFilePath);
        void Save();
        void Save(string filePath);
    }

    public enum ProjectChangedKind
    {
        DocumentAdded
    }

    public class ProjectChangedEventArgs : EventArgs
    {
        public ProjectChangedEventArgs(ProjectChangedKind kind)
        {
            Kind = kind;
        }

        public ProjectChangedKind Kind { get; }
    }

    public delegate void ProjectChangedEventHandler(object sender, ProjectChangedEventArgs e);
}
