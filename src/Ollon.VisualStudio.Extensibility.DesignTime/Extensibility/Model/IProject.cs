using System;

namespace Ollon.VisualStudio.Extensibility.Model
{
    public interface IProject
    {
        string FilePath { get; }
        string ProjectDirectory { get; }
        string UniqueName { get; }
        string ProjectGuidString { get; }
        string ProjectFlavorGuid { get; }
        Guid ProjectGuid { get; }
        string ProjectTypeGuids { get; }
        ProjectType Type { get; }
        string Name { get; }
        string RootNamespace { get; }
        ProjectSystem ProjectSystem { get; }
        string TargetFramework { get; }
        void WriteTo(ProjectWriter writer);
    }
}
