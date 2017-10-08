// -----------------------------------------------------------------------
// <copyright file="IVsSolutionWriter.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Ollon.VisualStudio.Extensibility.Services
{
    public interface IVsSolutionWriter
    {
        void WriteSolution(string solutionName, string solutionDirectory);
    }
}
