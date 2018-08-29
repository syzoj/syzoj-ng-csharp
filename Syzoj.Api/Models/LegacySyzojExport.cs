using Newtonsoft.Json;

namespace Syzoj.Api.Models
{
    public class LegacySyzojExport
    {
        [JsonProperty(propertyName: "success")]
        public bool Success { get; set; }
        [JsonProperty(propertyName: "obj")]
        public LegacySyzojExportObject Obj { get; set; }
    }

    public class LegacySyzojExportObject
    {
        [JsonProperty(propertyName: "title")]
        public string Title { get; set; }
        [JsonProperty(propertyName: "description")]
        public string Description { get; set; }
        [JsonProperty(propertyName: "input_format")]
        public string InputFormat { get; set; }
        [JsonProperty(propertyName: "output_format")]
        public string OutputFormat { get; set; }
        [JsonProperty(propertyName: "example")]
        public string Example { get; set; }
        [JsonProperty(propertyName: "limit_and_hint")]
        public string LimitAndHint { get; set; }
        [JsonProperty(propertyName: "time_limit")]
        public int TimeLimit { get; set; }
        [JsonProperty(propertyName: "memory_limit")]
        public int MemoryLimit { get; set; }
        [JsonProperty(propertyName: "file_io")]
        public bool FileIo { get; set; }
        [JsonProperty(propertyName: "file_io_input_name")]
        public string FileIoInputName { get; set; }
        [JsonProperty(propertyName: "File_io_output_name")]
        public string FileIoOutputName { get; set; }
        [JsonProperty(propertyName: "type")]
        public string Type { get; set; }
        [JsonProperty(propertyName: "tags")]
        public string[] Tags { get; set; }
    }
}