// -----------------------------------------------------------------------
// <copyright file="DynamicCommandAsync.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Ollon.VisualStudio.Extensibility.Input
{
    public abstract class DynamicCommandAsync : DynamicCommand
    {
        protected DynamicCommandAsync(IServiceProvider serviceProvider, CommandID id) : base(serviceProvider, id)
        {
        }

        protected override void Execute()
        {
            ThreadHelper.JoinableTaskFactory.Run(ExecuteAsync);
        }

        protected abstract Task ExecuteAsync();

        protected sealed override void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.Run(() => OnBeforeQueryStatusAsync(sender, e, new CancellationToken()));
        }

        protected virtual Task OnBeforeQueryStatusAsync(object sender, EventArgs e, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
