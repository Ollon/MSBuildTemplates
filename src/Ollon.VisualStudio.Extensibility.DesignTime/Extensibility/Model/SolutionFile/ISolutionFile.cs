using System.Collections.Generic;

namespace Ollon.VisualStudio.Extensibility.Model.SolutionFile
{
    public interface ISolutionFile
    {
        string FilePath { get; }
        IEnumerable<SectionBlock> GlobalSectionBlocks { get; }
        IEnumerable<string> HeaderLines { get; }
        string MinimumVisualStudioVersionLineOpt { get; }
        IEnumerable<ProjectBlock> ProjectBlocks { get; }
        string SolutionDir { get; }
        string VisualStudioVersionLineOpt { get; }

        void Dispose();
        string GetText();
    }
}
