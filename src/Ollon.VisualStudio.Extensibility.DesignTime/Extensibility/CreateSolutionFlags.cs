// -----------------------------------------------------------------------
// <copyright file="CreateSolutionFlags.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.VisualStudio.Shell.Interop;

namespace Ollon.VisualStudio.Extensibility
{
    public static class CreateSolutionFlags
    {
        public const uint Overwrite = (uint) __VSCREATESOLUTIONFLAGS.CSF_OVERWRITE;
    }
}
