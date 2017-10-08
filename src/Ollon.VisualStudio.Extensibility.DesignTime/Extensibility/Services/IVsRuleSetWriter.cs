// -----------------------------------------------------------------------
// <copyright file="IVsRuleSetWriter.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Ollon.VisualStudio.Extensibility.Services
{
    public interface IVsRuleSetWriter
    {
        void WriteRuleSet(ISolutionOptions options);
    }
}
