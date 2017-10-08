// -----------------------------------------------------------------------
// <copyright file="DynamicFiles.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ollon.VisualStudio.Extensibility.Model.ProjectItem
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DynamicFiles
    {

        public DynamicFile this[string name]
        {
            get
            {
                DynamicFile file = null;
                foreach (DynamicFile dynamicFile in Files)
                {
                    if (name.Equals(dynamicFile.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        file = dynamicFile;
                        break;
                    }
                }
                return file;
            }
        }

        [JsonProperty("files")]
        public List<DynamicFile> Files { get; set; }
    }
}
