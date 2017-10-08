// -----------------------------------------------------------------------
// <copyright file="DialogScope.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ollon.VisualStudio.Extensibility.Progress
{
    public sealed class DialogScope : IProgress<DialogProgress>, IDisposable
    {
        private readonly IVsThreadedWaitDialog4 _dialog;
        private readonly VsThreadedWaitDialogCallback _callBack;
        private DialogProgress _previousProgress;
        public DialogScope()
        {
            _callBack = new VsThreadedWaitDialogCallback();
            _callBack.Cancelled += OnCancelled;

            IVsThreadedWaitDialogFactory factory =
                (IVsThreadedWaitDialogFactory) Package.GetGlobalService(typeof(SVsThreadedWaitDialogFactory));

            factory.CreateInstance(out IVsThreadedWaitDialog2 ppIVsThreadedWaitDialog);

            if (ppIVsThreadedWaitDialog is IVsThreadedWaitDialog4 dialog4)
            {
                _dialog = dialog4;
            }

            _dialog.StartWaitDialogWithCallback("", "", "", null, "", true, 0, true, 0, 0, _callBack);
        }

        private void OnCancelled(object sender, EventArgs e)
        {
            Dispose();
        }

        public void Update(string progressText)
        {
            if (_previousProgress != null)
            {
                Update(_previousProgress.WaitMessage, progressText, _previousProgress.StatusBarText, _previousProgress.IsCancelable);
            }
            else
            {
                Update("", progressText);
            }
        }

        public void Update(string waitMessage, string progressText = null, string statusBarText = null, bool isCancelable = false, int currentStep = 0, int totalSteps = 0)
        {
           _previousProgress =  new DialogProgress(
                waitMessage,
                progressText,
                statusBarText,
                currentStep,
                totalSteps,
                isCancelable);

            Report(_previousProgress);
        }

        /// <summary>Reports a progress update.</summary>
        /// <param name="value">The value of the updated progress.</param>
        public void Report(DialogProgress value)
        {
            _dialog.UpdateProgress(
                value.WaitMessage,
                value.ProgressText,
                value.StatusBarText,
                value.CurrentStep,
                value.TotalSteps,
                value.IsCancelable,
                out bool pfCanceled);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            _dialog.EndWaitDialog(out int pfCanceled);
        }

        private class VsThreadedWaitDialogCallback : IVsThreadedWaitDialogCallback
        {
            public event EventHandler Cancelled;

            public void OnCanceled()
            {
                Cancelled?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
