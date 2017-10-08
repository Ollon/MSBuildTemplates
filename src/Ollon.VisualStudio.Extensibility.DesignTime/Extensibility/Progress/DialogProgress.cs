// -----------------------------------------------------------------------
// <copyright file="DialogProgress.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Ollon.VisualStudio.Extensibility.Progress
{
    public class DialogProgress
    {
        public DialogProgress(
            string waitMessage,
            string progressText = null,
            string statusBarText = null,
            int currentStep = 0,
            int totalSteps = 0,
            bool isCancelable = true)
        {
            WaitMessage = waitMessage;
            ProgressText = progressText ?? waitMessage;
            StatusBarText = statusBarText ?? progressText;
            CurrentStep = currentStep;
            TotalSteps = totalSteps;
            IsCancelable = isCancelable;
        }

        public string WaitMessage { get; }

        public string ProgressText { get; }

        public string StatusBarText { get; }

        public int CurrentStep { get; }

        public int TotalSteps { get; }

        public bool IsCancelable { get; }
    }
}
