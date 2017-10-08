// -----------------------------------------------------------------------
// <copyright file="ProjectItemInfo.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Immutable;
using System.IO;

namespace Ollon.VisualStudio.Extensibility.Model
{
    public class ProjectItemInfo
    {
        public ProjectItemInfo(
            string itemType,
            string specifiedPath,
            IImmutableDictionary<string, string> metadata = null)
        {
            SpecifiedPath = specifiedPath ?? throw new ArgumentNullException(nameof(specifiedPath));
            ItemType = itemType ?? throw new ArgumentNullException(nameof(itemType));
            Metadata = metadata ?? ImmutableDictionary<string, string>.Empty;
        }

        public string ItemType { get; }

        public string SpecifiedPath { get; }

        public ProjectItemState State
        {
            get
            {
                ProjectItemState state = ProjectItemState.Default;
                if (Metadata.ContainsKey("DependentUpon"))
                {
                    state |= ProjectItemState.DependentUpon;
                }
                if (Metadata.ContainsKey("Link"))
                {
                    state |= ProjectItemState.Link;
                }
                if (Metadata.ContainsKey("Visible"))
                {
                    bool visible = bool.Parse(Metadata["Visible"]);
                    if (visible)
                    {
                        state |= ProjectItemState.Visible;
                    }
                    else
                    {
                        state |= ProjectItemState.Hidden;
                    }
                }
                if (Metadata.ContainsKey("FullPath"))
                {
                    string fullPath = Metadata["FullPath"];
                    if (File.Exists(fullPath))
                    {
                        state |= ProjectItemState.Exists;
                    }
                    else
                    {
                        state |= ProjectItemState.Missing;
                    }
                }
                return state;
            }
        }

        public IImmutableDictionary<string, string> Metadata { get; }
    }
}
