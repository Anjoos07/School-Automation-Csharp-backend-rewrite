using System.Net;
using System.Text.Json.Nodes;

namespace Forms;

public class PayloadModel
    {
        public object Blocks { get; set; } = default!;
        public string Status { get; set; } = "";
        public object Settings { get; set; } = default!;
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
}