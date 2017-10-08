namespace Ollon.VisualStudio.Extensibility.Services
{
    public interface IDirectoryBuildTargetsWriter
    {
        void WriteDirectoryBuildProps(ISolutionOptions options);
    }
}