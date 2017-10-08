// -----------------------------------------------------------------------
// <copyright file="IStrongNameKeyGenerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Ollon.VisualStudio.Extensibility.StrongName;

namespace Ollon.VisualStudio.Extensibility.Services
{
    public interface IStrongNameKeyGenerator
    {
        StrongNameKeyInfo Generate();
    }
}
