using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ollon.VisualStudio.Extensibility.Services
{
    public interface ISolutionFileWriter
    {
        string FileExtension { get; }
        Task WriteAsync(string solutionName, string solutionDirectory);
    }
}
