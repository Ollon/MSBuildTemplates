using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ollon.VisualStudio.Extensibility.Services
{
    public interface IProjectFileWriter
    {
        string FileExtension { get; }
        Task WriteExplicitSdkImportsIfNecessaryAsync(IProjectFileLoader loader);

        Task WriteProjectFileAsync(string destination, CancellationToken cancellationToken);
    }
}
