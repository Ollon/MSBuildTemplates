// -----------------------------------------------------------------------
// <copyright file="MSBuildProjectCollection.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.Composition;
using Microsoft.Build.Evaluation;

namespace Ollon.VisualStudio.Extensibility.Implementation.Services
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MSBuildProjectCollection : ProjectCollection
    {
        [ImportingConstructor]
        public MSBuildProjectCollection()
            :base(null, null, null, ToolsetDefinitionLocations.Default, 8, false)
        {
            
        }
    }
}
