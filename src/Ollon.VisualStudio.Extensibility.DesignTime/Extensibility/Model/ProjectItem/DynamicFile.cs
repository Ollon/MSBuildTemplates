using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ollon.VisualStudio.Extensibility.Model.ProjectItem
{
    [JsonObject(MemberSerialization.OptOut)]
    public class DynamicFile
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("baseName")]
        public string BaseName { get; set; }

        [JsonProperty("extension")]
        public string Extension { get; set; }

        [JsonProperty("lines")]
        public List<string> Lines { get; set; }

        [JsonProperty("lineCount")]
        public int LineCount => Lines.Count;
    }
}
