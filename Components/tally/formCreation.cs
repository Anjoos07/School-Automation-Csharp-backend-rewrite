using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using Utilities;

namespace Forms;
public static class FormModel
{
    public static async Task<IResult> GetModel(string url)
    {
        var baseUrl = url;
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "tly-2DX13TTyswPNQm3m0sRLJRFgDJpWtxlY");
        var response = await client.GetAsync(baseUrl);
        var json = await response.Content.ReadAsStringAsync();
        return Results.Text(json, "application/json");
    }
     // For Post Operations
     
    public static async Task<IResult> PostModel(string url, PayloadModel payload)
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

    public static async Task<IResult> CreateForm()
    {
        return null!;
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

public class Condition
{

    // Creates individual Conditionals.
    private static Dictionary<string, object> CreateConditionals(
    string groupUuid,
    string title,
    string fieldType,
    string questionType,
    string comparison,
    object value)
    {
        return new Dictionary<string, object>
        {
            ["uuid"] = Helper.GenUUID(),
            ["type"] = "SINGLE",
            ["payload"] = new Dictionary<string, object>
            {
                ["field"] = new Dictionary<string, object>
                {
                    ["uuid"] = groupUuid,
                    ["title"] = title,
                    ["type"] = fieldType,
                    ["questionType"] = questionType,
                    ["blockGroupUuid"] = groupUuid
                },
                ["comparison"] = comparison,
                ["value"] = value
            }
        };
    }

    // Creates the Actions for the Conditions.
    static class Actions
    {

        // Creates JumpToPage Action.
        public static Dictionary<string, object> JumpToPage(string pageUuid)
        {
            return new Dictionary<string, object>
            {
                ["uuid"] = Helper.GenUUID(),
                ["type"] = "JUMP_TO_PAGE",
                ["payload"] = new Dictionary<string, object>
                {
                    ["jumpToPage"] = pageUuid
                }
            };
        }
    }

    // Main Condition Creation Function
    public static Dictionary<string, object> ConditionalLogic(
    List<Dictionary<string, object>> conditions,
    List<Dictionary<string, object>> actions,
    string logicalOperator = "AND")
    {
        return new Dictionary<string, object>
        {
            ["uuid"] = Helper.GenUUID(),
            ["type"] = "CONDITIONAL_LOGIC",
            ["groupUuid"] = Helper.GenUUID(),
            ["groupType"] = "CONDITIONAL_LOGIC",
            ["payload"] = new Dictionary<string, object>
            {
                ["logicalOperator"] = logicalOperator,
                ["conditionals"] = conditions,
                ["actions"] = actions
            }
        };
    }


    public static Block PageCondition(string groupUUID, bool hideButton)
    {
        return new Block
        {
            type = "CONDITIONAL_LOGIC",
            groupType = "CONDITIONAL_LOGIC",
            payload = {
                ["logicalOperator"] = "AND",
                ["conditionals"] = new List<object>{
                    new Dictionary<string,object>
                    {
                        ["uuid"] = Helper.GenUUID(),
                        ["type"] = "SINGLE",
                        ["payload"] = new Dictionary<string,object>{
                           ["field"] = new Dictionary<string,object>{
                                ["uuid"] = groupUUID,
                                ["title"] = "Events",
                                ["type"] = "INPUT_FIELD",
                                ["questionType"] = "CHECKBOXES",
                                ["blockGroupUuid"] = groupUUID,
                            },
                            ["comparison"] = hideButton ? "IS_NOT_EMPTY" : "IS_EMPTY",
                            ["value"] = null!,
                        },
                    }
                },
                ["actions"] = new List<object>{
                    new Dictionary<string,object>
                    {
                        ["uuid"] = Helper.GenUUID(),
                        ["type"] = hideButton ? "JUMP_TO_PAGE" : "HIDE_BUTTON_TO_DISABLE_COMPLETION",
                    }
                }
            }
        };
    }
}