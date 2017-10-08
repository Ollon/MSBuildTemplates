// -----------------------------------------------------------------------
// <copyright file="ProjectItemState.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Ollon.VisualStudio.Extensibility.Model
{
    [Flags]
    public enum ProjectItemState : byte
    {
        Default = 0,
        Exists = 1 << 0,
        Generated = 1 << 1,
        Missing = 1 << 2,
        Link = 1 << 3,
        DependentUpon = 1 << 4,
        Visible = 1 << 5,
        Hidden = 1 << 6
    }
}
