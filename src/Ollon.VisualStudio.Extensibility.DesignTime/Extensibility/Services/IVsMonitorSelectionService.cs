// -----------------------------------------------------------------------
// <copyright file="IVsMonitorSelectionService.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ollon.VisualStudio.Extensibility.Services
{
    public interface IVsMonitorSelectionService
    {
        Task<IVsHierarchy> GetCurrentSelectionAsync(CancellationToken cancellationToken);
    }
}
