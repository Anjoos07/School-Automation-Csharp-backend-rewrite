using System.Net.Http.Headers;
using Npgsql.Internal.Postgres;
using Superpower.Model;
using Utilities;
using Request;
using System.Text.Json;

namespace Forms;

public static class FormCreation
{

    public static async Task<Response> CreateForm(Dictionary<string, FormField> eventList)
    {
        string api = Helper.GetKey();
                if (api is null)
                {
                        return new Response
                        {
                                StatusCode = 0,
                                IsSuccess = false,
                                Text = "API key not found"
                        };
                }
                string baseUrl = $"https://api.tally.so/forms";

                Dictionary<string, string> header = new Dictionary<string, string>
                {
                        ["Authorization"] = $"Bearer {api}" 
                };

                PayloadModel payload = new(){
                    blocks = GenBlock.GenBlocks(eventList),
                    status = "PUBLISHED"
                };

                Console.WriteLine(
                    JsonSerializer.Serialize(payload,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }));

                Response response = await Requests.PostAsync(baseUrl, payload, header);

        return response;
    } 
}

public class Block
{
    public string type {get; set;} = "";
    public string groupType {get; set;} = ""; 
    public Dictionary<string,object> payload {get; set;} = new();
    public string uuid {get; set;} =  Helper.GenUUID();
    public string groupUuid {get; set;} = Helper.GenUUID();
    public static List<List<string>> SafeHtml(string text)
    {
        return [[text]];
    }
    public static List<Block> FormTitle(string title,string uuid)
    {
        return new List<Block>
        {
            new Block
            {
                type = "FORM_TITLE",
                groupType = "TEXT",
                groupUuid = uuid,
                payload = new Dictionary<string, object>
                {
                    ["safeHTMLSchema"] = SafeHtml(title),
                    ["title"] = title
                }
            }
        };
    }
    public static List<Block> Title(string text)
    {
        return new List<Block>
        {
            new Block
            {
            type = "TITLE",
            groupType = "QUESTION",
            payload = new Dictionary<string, object>
                {
                ["safeHTMLSchema"] = SafeHtml(text)
                }  
            }
        };
    }
    public static List<Block> InputText(string placeholder,string uuid)
    {
        List<Block> block =
        [
            .. Title(placeholder),
            new Block
            {
                type = "INPUT_TEXT",
                groupType = "INPUT_TEXT",
                groupUuid = uuid,
                payload = new Dictionary<string, object>
                {
                    ["placeholder"] = placeholder
                }
            },
        ];
        return block;
    }
    public static List<Block> InputNumber(string placeholder,string uuid)
    {
        List<Block> block =
        [
            .. Title(placeholder),
            new Block
            {
                type = "INPUT_NUMBER",
                groupType = "INPUT_NUMBER",
                groupUuid = uuid,
                payload = new Dictionary<string, object>
                {
                    ["placeholder"] = placeholder
                }
            },
        ];
        return block;
    }
    public static List<Block> Checkbox(string name, List<string> events,string uuid)
    {
        List<Block> mainBlock = [.. Title(name)];

        for(int index = 0; index < events.Count; index++)
        {   Block block = new Block()
            {
                type="CHECKBOX",
                groupUuid= uuid,
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
    public static List<Block> Dropdown(string name, List<string> options,string uuid)
    {
        List<Block> mainBlock = [.. Title(name)];

        for(int index = 0; index < options.Count; index++)
        {   Block block = new Block()
            {
                type="DROPDOWN_OPTION",
                groupUuid= uuid,
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
    public static List<Block> PageBreak(string pageUUID)
    {
        return new List<Block>
        {
            new Block
            {
                uuid=pageUUID,
                type="PAGE_BREAK",
                groupUuid=pageUUID,
                groupType="PAGE_BREAK",
                payload= new Dictionary<string, object>
                {
                    ["button"] = new Dictionary<string, object>
                    {
                        ["label"] = "Submit"
                    }
                }
            }
        };
    }
}

public class Condition
{

    // Creates individual Conditionals.
    private static Dictionary<string, object> CreateConditionals(
    string groupUuid,
    string title,
    string questionType,
    string comparison,
    string value,
    string fieldType = "INPUT_FIELD")
    {
        object finalValue;

        if (int.TryParse(value, out int number))
            finalValue = number;
        else
            finalValue = value;

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
                ["value"] = finalValue
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
    public static List<Block> ConditionalLogic(
    List<Dictionary<string, object>> conditions,
    List<Dictionary<string, object>> actions,
    string logicalOperator = "AND")
    {
        return new List<Block>{
            new Block{
                uuid = Helper.GenUUID(),
                type = "CONDITIONAL_LOGIC",
                groupUuid = Helper.GenUUID(),
                groupType = "CONDITIONAL_LOGIC",
                payload = new Dictionary<string, object>
                {
                    ["logicalOperator"] = logicalOperator,
                    ["conditionals"] = conditions,
                    ["actions"] = actions
                }
            }
        };
    }


    public static List<Block> ConditionOperation(List<List<Dictionary<string,List<string>>>> conditionList, List<Block> blocks, Dictionary<string,FormField> events)
    {
        List<Block> conditionBlock = [];
        Dictionary<string,Func<string,Dictionary<string, object>>> operations = new()
        {
            {"JumpToPage", Actions.JumpToPage}
        };

        foreach (List<Dictionary<string,List<string>>> group in conditionList)
        {
            List<Dictionary<string, object>> condition = [];
            List<Dictionary<string, object>> action = [];
            foreach (Dictionary<string,List<string>> dict in group)
            {
                foreach (KeyValuePair<string,List<string>> kvp in dict)
                {
                    Console.WriteLine($"  {kvp.Key}");
                    if(kvp.Key != "Action"){
                        Block? block = blocks.FirstOrDefault(b => b.groupUuid == kvp.Value[0]);
                        string? key = events
                        .FirstOrDefault(x => x.Value.groupUUID?.ToString() == kvp.Value[0])
                        .Key;
                        Console.WriteLine($"    {block.type} {key}");
                        condition.AddRange(CreateConditionals(kvp.Value[0], key, block.type, kvp.Value[1], kvp.Value[2]));
                    }
                    else
                    {
                        action.AddRange(operations[kvp.Value[0]](kvp.Value[1]));
                    }
                }
            }
            conditionBlock.AddRange(ConditionalLogic(condition ,action));
        }

        return conditionBlock;
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

public class Operations
{
   public static Dictionary<string,List<Block>> GenPageBreak(List<string> pages)
    {
        Dictionary<string,List<Block>> pageBlock = new();
        foreach(string uuid in pages)
        {
            pageBlock[uuid] = Block.PageBreak(uuid);
        }
        return pageBlock;
    }
}

// Main Function
public class GenBlock
{
    public static List<Block> GenBlocks(Dictionary<string, FormField> events)
    {
        var mainBlock = new List<object>();
        // Operations Dictionary
        
        Dictionary<string,
        Func<string,List<string>?,string,List<List<Dictionary<string,List<string>>>>?,List<Block>,Dictionary<string,FormField>,
        List<Block>>>
        operations = new()
        {
            { "FormTitle", (key, options, uuid,condition,block,events) => Block.FormTitle(key,uuid) },
            { "Title", (key, options,uuid,condition,block,events) => Block.Title(key) },
            { "InputText", (key, options,uuid,condition,block,events) => Block.InputText(key,uuid) },
            { "InputNumber", (key, options,uuid,condition,block,events) => Block.InputNumber(key,uuid) },
            { "Checkbox", (key, options,uuid,condition,block,events) => Block.Checkbox(key,options!,uuid) },
            { "Dropdown", (key, options,uuid,condition,block,events) => Block.Dropdown(key,options!,uuid) },
            { "Condition", (key,options,uuid,condition,block,events) => Condition.ConditionOperation(condition!,block,events)}
        };

        Dictionary<string,List<Block>> pages = Operations.GenPageBreak(events["PageBreakGen"].Options!);
        events.Remove("PageBreakGen");

        List<Block> allBlocks = [];

        foreach (var field in events)
        {
            string key = field.Key;
            string type = field.Value.Type;
            List<string>? options = field.Value.Options;
            string uuid = field.Value.groupUUID;
            List<List<Dictionary<string,List<string>>>>? conditions = field.Value.conditions;

            List<Block> block = type == "PageBreak" ? pages[key] : operations[type](key, options, uuid, conditions,allBlocks,events);
            allBlocks.AddRange(block);
        }

        return allBlocks;
    }

    // Form Definition
        // Dictionary<string, (string Type, List<string>? Options)> form = new()
        // {
        //     {
        //         "PageBreakGen",
        //         ("PageBreak", new()
        //         {
        //             "page1",
        //             "page2"
        //         })
        //     },
        //     { "Name", ("form_title", null) },
        //     { "Class", ("checkbox", new()
        //         {
        //             "1",
        //             "2",
        //             "3"
        //         })
        //     },
        //     { "Department", ("dropdown", new()
        //         {
        //             "CSE",
        //             "ECE",
        //             "ME"
        //         })
        //     },
        //     { "page2", ("page_break", null) },
        //     { "Age", ("input_text", null) }
        // };
}