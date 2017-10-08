// -----------------------------------------------------------------------
// <copyright file="VsMonitorSelectionService.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Ollon.VisualStudio.Extensibility.Services;

namespace Ollon.VisualStudio.Extensibility.Implementation.Services
{
    [Export(typeof(IVsMonitorSelectionService))]
    internal sealed class VsMonitorSelectionService : IVsMonitorSelectionService
    {
        public async Task<IVsHierarchy> GetCurrentSelectionAsync(CancellationToken cancellationToken)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            IVsMonitorSelection monitorSelection = (IVsMonitorSelection) Package.GetGlobalService(typeof(SVsShellMonitorSelection));

            List<IVsHierarchy> list = new List<IVsHierarchy>();
            int hr = monitorSelection.GetCurrentSelection(
                out IntPtr hierarchyPtr, out uint itemID, out IVsMultiItemSelect multiSelect, out IntPtr containerPtr);
            if (IntPtr.Zero != containerPtr)
            {
                Marshal.Release(containerPtr);
                containerPtr = IntPtr.Zero;
            }
            if (itemID == (uint)VSConstants.VSITEMID.Selection)
            {
                hr = multiSelect.GetSelectionInfo(out uint itemCount, out int fSingleHierarchy);
                VSITEMSELECTION[] items = new VSITEMSELECTION[itemCount];
                hr = multiSelect.GetSelectedItems(0, itemCount, items);
                foreach (VSITEMSELECTION item in items)
                {
                    list.Add(item.pHier);
                }
            }
            else
            {
                if (hierarchyPtr != IntPtr.Zero)
                {
                    IVsHierarchy hierarchy = (IVsHierarchy)Marshal.GetUniqueObjectForIUnknown(hierarchyPtr);
                    list.Add(hierarchy);
                }
            }
            return list.FirstOrDefault();
        }
    }
}
