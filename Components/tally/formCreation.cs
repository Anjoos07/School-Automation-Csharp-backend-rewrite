using System.Net.Http.Headers;
using Utilities;

namespace Forms;

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

    public static List<Block> FormTitle(string title)
    {
        return new List<Block>
        {
            new Block
            {
                type = "FORM_TITLE",
                groupType = "TEXT",
                payload = new Dictionary<string, object>
                {
                    ["title"] = title,
                    ["html"] = title 
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
                ["html"] = text
                }  
            }
        };
    }
    public static List<Block> InputText(string placeholder)
    {
        return new List<Block>
        {
            new Block
            {
                type = "INPUT_TEXT",
                groupType = "INPUT_TEXT",
                payload = new Dictionary<string, object>
                {
                    ["placeholder"] = placeholder
                }
            }
        };
    }
    public static List<Block> InputNumber(string placeholder)
    {
        return new List<Block>
        {
            new Block
            {
                type = "INPUT_NUMBER",
                groupType = "INPUT_NUMBER",
                payload = new Dictionary<string, object>
                {
                    ["placeholder"] = placeholder
                }
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
    public static Block PageBreak(string pageUUID)
    {
        return  new Block
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

public class Operations
{
   public static Dictionary<string,Block> GenPageBreak(List<string> pages)
    {
        Dictionary<string,Block> pageBlock = new();
        foreach(string page in pages)
        {
            pageBlock[page] = Block.PageBreak(Helper.GenUUID());
        }
        return null!;
    }
}

// Main Function
public class GenBlock
{
    public static List<object> GenBlocks(Dictionary<string, (string Type, List<string>? Options)> events)
    {
        var mainBlock = new List<object>();
        // Operations Dictionary
        
        Dictionary<string, Func<string, List<string>?, List<Block>>> operations = new()
        {
            { "form_title", (key, options) => Block.FormTitle(key) },
            { "form_title", (key, options) => Block.Title(key) },
            { "input_text", (key, options) => Block.InputText(key) },
            { "form_title", (key, options) => Block.InputNumber(key) },
            { "checkbox", (key, options) => Block.Checkbox(key, options!) },
            { "dropdown", (key, options) => Block.Dropdown(key, options!) }
        };

        Dictionary<string,Block> pages = Operations.GenPageBreak(events["page_break_gen"].Options!);
        events.Remove("page_break_gen");

        List<Block> allBlocks = new();

        foreach (var field in events)
        {
            string key = field.Key;
            string type = field.Value.Type;
            List<string>? options = field.Value.Options;

            allBlocks.AddRange(operations[type](key, options));
        }

        return null!;
    }

    // Form Definition
        Dictionary<string, (string Type, List<string>? Options)> form = new()
        {
            {
                "PageBreakGen",
                ("PageBreak", new()
                {
                    "page1",
                    "page2"
                })
            },
            { "Name", ("form_title", null) },
            { "Class", ("checkbox", new()
                {
                    "1",
                    "2",
                    "3"
                })
            },
            { "Department", ("dropdown", new()
                {
                    "CSE",
                    "ECE",
                    "ME"
                })
            },
            { "page2", ("page_break", null) },
            { "Age", ("input_text", null) }
        };
}