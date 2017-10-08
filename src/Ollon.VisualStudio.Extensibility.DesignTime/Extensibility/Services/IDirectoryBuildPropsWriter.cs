// -----------------------------------------------------------------------
// <copyright file="IDirectoryBuildPropsWriter.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Ollon.VisualStudio.Extensibility.Services
{
    public interface IDirectoryBuildPropsWriter
    {
        void WriteDirectoryBuildProps(ISolutionOptions options);
    }
}
