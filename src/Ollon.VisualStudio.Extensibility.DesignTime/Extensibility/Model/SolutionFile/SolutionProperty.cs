// -----------------------------------------------------------------------
// <copyright file="SolutionProperty.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Ollon.VisualStudio.Extensibility.Model.SolutionFile
{
    public class SolutionProperty
    {
        public SolutionProperty(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public string Value { get; }
    }
}
