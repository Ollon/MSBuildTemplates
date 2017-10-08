// -----------------------------------------------------------------------
// <copyright file="IProjectXmlService.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.Composition;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio.ProjectSystem;
using Task = System.Threading.Tasks.Task;

namespace Ollon.VisualStudio.Extensibility.Services
{

    public interface IProjectXmlService
    {
        Task SetExplicitSdkImportsIfNecessaryAsync(string filePath);
    }
}
