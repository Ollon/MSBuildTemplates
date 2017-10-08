// -----------------------------------------------------------------------
// <copyright file="ProjectFileLoader.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.Composition;
using Microsoft.Build.Evaluation;

namespace Ollon.VisualStudio.Extensibility.Model
{
    [Export(typeof(IProjectFileLoader))]
    public class ProjectFileLoader : IProjectFileLoader
    {
        [Import]
        internal ProjectCollection ProjectCollection { get; private set; }

        public IProject2 Load(string filePath)
        {
            return new BuildProject(filePath, this);
        }
    }
}
