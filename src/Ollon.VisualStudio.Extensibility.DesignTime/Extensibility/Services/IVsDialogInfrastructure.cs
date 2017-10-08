// -----------------------------------------------------------------------
// <copyright file="IVsDialogInfrastructureProvider.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.VisualStudio.PlatformUI;

namespace Ollon.VisualStudio.Extensibility.Services
{

    public interface IVsDialogInfrastructure<TModel, TViewModel, TDialogWindow>
        where TDialogWindow : DialogWindow
    {
        TModel Model { get; }
        TViewModel ViewModel { get; }
        TDialogWindow View { get; }
    }
}
