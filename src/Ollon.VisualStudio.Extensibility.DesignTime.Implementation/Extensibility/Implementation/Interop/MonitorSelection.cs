// -----------------------------------------------------------------------
// <copyright file="MonitorSelection.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ollon.VisualStudio.Extensibility.Implementation.Interop
{
    [Export(typeof(IVsMonitorSelection))]
    public class MonitorSelection : IVsMonitorSelection
    {
        private readonly IVsMonitorSelection _selection;

        [ImportingConstructor]
        private MonitorSelection([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
            : this ((IVsMonitorSelection)serviceProvider.GetService(typeof(SVsShellMonitorSelection)))
        {
            
        }
        public MonitorSelection(IVsMonitorSelection selection)
        {
            _selection = selection;
        }

        public int GetCurrentSelection(out IntPtr ppHier, out uint pitemid, out IVsMultiItemSelect ppMIS, out IntPtr ppSC)
        {
            return _selection.GetCurrentSelection(out ppHier, out pitemid, out ppMIS, out ppSC);
        }

        public int AdviseSelectionEvents(IVsSelectionEvents pSink, out uint pdwCookie)
        {
            return _selection.AdviseSelectionEvents(pSink, out pdwCookie);
        }

        public int UnadviseSelectionEvents(uint dwCookie)
        {
            return _selection.UnadviseSelectionEvents(dwCookie);
        }

        public int GetCurrentElementValue(uint elementid, out object pvarValue)
        {
            return _selection.GetCurrentElementValue(elementid, out pvarValue);
        }

        public int GetCmdUIContextCookie(ref Guid rguidCmdUI, out uint pdwCmdUICookie)
        {
            return _selection.GetCmdUIContextCookie(ref rguidCmdUI, out pdwCmdUICookie);
        }

        public int IsCmdUIContextActive(uint dwCmdUICookie, out int pfActive)
        {
            return _selection.IsCmdUIContextActive(dwCmdUICookie, out pfActive);
        }

        public int SetCmdUIContext(uint dwCmdUICookie, int fActive)
        {
            return _selection.SetCmdUIContext(dwCmdUICookie, fActive);
        }
    }
}
