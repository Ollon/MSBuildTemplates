// -----------------------------------------------------------------------
// <copyright file="DynamicFileLoader.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Newtonsoft.Json;
using Ollon.VisualStudio.Extensibility.Model.ProjectItem;
using Ollon.VisualStudio.Extensibility.Utilities;

namespace Ollon.VisualStudio.Extensibility.Model
{
    public static class DynamicFileLoader
    {
        public static DynamicFiles LoadDynamicFiles()
        {
            string downloadString = WebUtilities.GetWebFileString("http://ollon.scienceontheweb.net/files.json");

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            return JsonConvert.DeserializeObject<DynamicFiles>(downloadString, settings);
        }

    }
}
