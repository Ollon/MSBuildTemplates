// -----------------------------------------------------------------------
// <copyright file="ProjectPropertyGroupElementExtensions.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Build.Construction;

namespace Ollon.VisualStudio.Extensibility.Utilities
{
    public static class ProjectPropertyGroupElementExtensions
    {
        public static void AddProperty(this ProjectPropertyGroupElement element, string name, string value, string condition)
        {
            ProjectPropertyElement property = element.AddProperty(name, value);
            property.Condition = condition;
        }


        public static void AddDefaultProperty(this ProjectPropertyGroupElement element, string name, string value)
        {
            ProjectPropertyElement property = element.AddProperty(name, value);
            property.Condition = $"'$({name})' == ''";
        }
    }
}
