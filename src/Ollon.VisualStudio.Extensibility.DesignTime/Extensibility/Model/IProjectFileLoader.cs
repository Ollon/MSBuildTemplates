// -----------------------------------------------------------------------
// <copyright file="IProjectFileLoader.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Ollon.VisualStudio.Extensibility.Model
{
    public interface IProjectFileLoader
    {
        IProject2 Load(string filePath);
    }
}
