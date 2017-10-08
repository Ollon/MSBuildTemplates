using Microsoft.Build.Evaluation;

namespace Ollon.VisualStudio.Extensibility.Services
{
    public interface IMSBuildProjectLoader
    {

        Project LoadProject(string filePath);

    }
}