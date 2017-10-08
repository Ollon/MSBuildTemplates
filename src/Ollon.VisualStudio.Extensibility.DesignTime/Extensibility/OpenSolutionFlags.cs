// -----------------------------------------------------------------------
// <copyright file="OpenSolutionFlags.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.VisualStudio.Shell.Interop;

namespace Ollon.VisualStudio.Extensibility
{
    public static class OpenSolutionFlags
    {
        public const uint Silent = (uint) __VSSLNOPENOPTIONS.SLNOPENOPT_Silent;
    }
}
