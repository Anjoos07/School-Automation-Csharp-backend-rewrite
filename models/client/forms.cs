using System.Net;
using System.Text.Json.Nodes;
using Utilities;

namespace Forms;

public class PayloadModel
    {
        public object blocks { get; set; } = default!;
        public string status { get; set; } = "";
    }

public class Response
    {
        public int StatusCode { get; init; }
        public JsonNode? Json { get; init; }
        public bool IsSuccess { get; init; }
        public string? Text { get; set; }
    }

public class FormField
{
    public string Type { get; set; } = "";
    public List<string>? Options { get; set; }
    public string groupUUID {get; set;} = Helper.GenUUID();
    public List<List<Dictionary<string,List<string>>>>? conditions {get; set;}
}