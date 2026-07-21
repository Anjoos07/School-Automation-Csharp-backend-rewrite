using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using Utilities;

namespace Forms;
public static class FormPostModel
{
     // For Post Operations
    public class Payload
    {
        public object Blocks { get; set; } = default!;
        public string Status { get; set; } = "";
        public object Settings { get; set; } = default!;
    }

    public static async Task<IResult> PostModel(string url, Payload payload)
    {
        var baseUrl = url;
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "tly-2DX13TTyswPNQm3m0sRLJRFgDJpWtxlY");
        var response = await client.PostAsJsonAsync(baseUrl,payload);
        var json = await response.Content.ReadAsStringAsync();
        return Results.Text(json, "application/json");
    }
}

public static class FormCreation
{
    public static void MapFormCreationEndpoints(this WebApplication app)
    {
        app.MapPost("/form-creation", async () =>
        {
            return 1;
        });
    }
}

public class Block
{
    public string type {get; set;} = "";
    public string groupType {get; set;} = ""; 
    public Dictionary<string,object> payload {get; set;} = new();
    public string uuid {get; set;} =  Helper.GenUUID();
    public string groupUuid {get; set;} = Helper.GenUUID();

    public static Block FormTitle(string title)
    {
        return new Block
        {
            type = "FORM_TITLE",
            groupType = "TEXT",
            payload = new Dictionary<string, object>
            {
                ["title"] = title,
                ["html"] = title 
            }
        };
    }
    public static Block Title(string text)
    {
        return new Block
        {
          type = "TITLE",
          groupType = "QUESTION",
          payload = new Dictionary<string, object>
            {
              ["html"] = text
            }  
        };
    }
    public static Block InputText(string placeholder)
    {
        return new Block
        {
            type = "INPUT_TEXT",
            groupType = "INPUT_TEXT",
            payload = new Dictionary<string, object>
            {
                ["placeholder"] = placeholder
            }
        };
    }
    public static Block InputNumber(string placeholder)
    {
        return new Block
        {
            type = "INPUT_NUMBER",
            groupType = "INPUT_NUMBER",
            payload = new Dictionary<string, object>
            {
                ["placeholder"] = placeholder
            }
        };
    }
    public static List<Block> Checkbox(string groupUUID, List<string> events)
    {
        List<Block> mainBlock = new();

        for(int index = 0; index < events.Count; index++)
        {   Block block = new Block()
            {
                type="CHECKBOX",
                groupUuid= groupUUID,
                groupType="CHECKBOXES",
                payload = new Dictionary<string, object>
                {
                    ["index"] = index,
                    ["isFirst"] = index == 0,
                    ["isLast"] = index == events.Count - 1,
                    ["text"] = events[index]
                }
            };
            mainBlock.Add(block);
        };
        return mainBlock;
    }
    public static Block PageBreak(string pageUUID)
    {
        return new Block
        {
            uuid=pageUUID,
            type="PAGE_BREAK",
            groupUuid=pageUUID,
            groupType="PAGE_BREAK",
            payload= new Dictionary<string, object>
            {
                ["buttom"] = new Dictionary<string, object>
                {
                    ["label"] = "Submit"
                }
            }
        };
    }
    public static List<Block> Dropdown(string groupUUID, List<string> options)
    {
        List<Block> mainBlock = new();

        for(int index = 0; index < options.Count; index++)
        {   Block block = new Block()
            {
                type="DROPDOWN_OPTION",
                groupUuid= groupUUID,
                groupType="DROPDOWN",
                payload = new Dictionary<string, object>
                {
                    ["isRequired"] = true,
                    ["index"] = index,
                    ["isFirst"] = index == 0,
                    ["isLast"] = index == options.Count - 1,
                    ["text"] = options[index]
                }
            };
            mainBlock.Add(block);
        };
    return mainBlock;
    }
}