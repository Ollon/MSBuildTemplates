// -----------------------------------------------------------------------
// <copyright file="DynamicCommand.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ollon.VisualStudio.Extensibility.Input
{
    public abstract class DynamicCommand : OleMenuCommand
    {
        protected DynamicCommand(IServiceProvider serviceProvider, CommandID id)
            : base(OnExecute, id)
        {
            ServiceProvider = serviceProvider;
            BeforeQueryStatus += OnBeforeQueryStatus;
        }

        protected IServiceProvider ServiceProvider { get; }

        protected EnvDTE80.DTE2 DTE
        {
            get
            {
                return ServiceProvider.GetService(typeof(SDTE)) as EnvDTE80.DTE2;
            }
        }

        protected Solution2 Solution2 => DTE.Solution as Solution2;

        protected virtual void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            CanExecute(sender as OleMenuCommand);
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            if (sender is DynamicCommand command)
            {
                command.Execute();
            }
        }

        protected virtual void CanExecute(OleMenuCommand command)
        {
            command.Enabled = command.Visible = command.Supported = true;
        }

        protected abstract void Execute();
    }
}
