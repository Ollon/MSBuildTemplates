using System.Collections.Generic;
using Ollon.VisualStudio.Extensibility.StrongName;

namespace Ollon.VisualStudio.Extensibility.Model
{
    public interface ISolution
    {
        string DocsDirectory { get; }
        string EditorConfigFilePath { get; }
        string GitIgnoreFilePath { get; }
        string DirectoryBuildPropsFilePath { get; }
        string DirectoryBuildTargetsFilePath { get; }
        IReadOnlyList<IProject> Projects { get; }
        string RepositoryDirectory { get; }
        string RootNamespace { get; }
        string RulesetFilePath { get; }
        string RulesetsDirectory { get; }
        StrongNameKeyInfo Snk { get; }
        string SolutionFilePath { get; }
        string SolutionName { get; }
        string SourceDirectory { get; }
        string StrongNameKeyFilePath { get; }
        string StrongNameKeysDirectory { get; }
        string TargetFramework { get; }
        string ToolsDirectory { get; }


        string BuildCmdFilePath { get; }
        string RestoreCmdFilePath { get; }
        string CleanCmdFilePath { get; }
        string RebuildCmdFilePath { get; }

        IProject FindProject(string projectName);
    }
}
