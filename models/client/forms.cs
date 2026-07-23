using System.Net;

namespace Forms;

public class PayloadModel
    {
        public object Blocks { get; set; } = default!;
        public string Status { get; set; } = "";
        public object Settings { get; set; } = default!;
    }

