// -----------------------------------------------------------------------
// <copyright file="StrongNameKeyManager.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.Composition;
using Ollon.VisualStudio.Extensibility.Services;
using Ollon.VisualStudio.Extensibility.StrongName;

namespace Ollon.VisualStudio.Extensibility.Implementation.Services
{
    [Export(typeof(IStrongNameKeyGenerator))]
    public class StrongNameKeyGenerator : IStrongNameKeyGenerator
    {
        public StrongNameKeyInfo Generate()
        {
            return StrongNameKeyManager.GenerateStrongNameKeyInfo();
        }
    }
}
