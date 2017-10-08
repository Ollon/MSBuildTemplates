namespace Ollon.VisualStudio.Extensibility
{
    public interface ISolutionOptions
    {
        string CompanyName { get; set; }
        string RepositoryDirectory { get; set; }

        string BuildDirectory { get; }
        string DocsDirectory { get; }

        string StrongNameKeysDirectory { get; }

        string RuleSetDirectory { get; }

        string DirectoryBuildPropsPath { get; }

        string SolutionDirectory { get; set; }
        string SolutionFilePath { get; set; }
        string SolutionName { get; set; }
        int SolutionVersionMajor { get; set; }
        int SolutionVersionMinor { get; set; }
    }
}