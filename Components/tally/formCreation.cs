using Utilities;
using Request;
using Data;
using System.Text.Json;
using System.Windows.Markup;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

// need to add settings
// need to add more actions in condition

namespace Forms;

public static class FormCreation
{

    public static async Task<FormResponseModel> CreateForm(Dictionary<string, FormField> eventList, AppDbContext db)
    {
        string api = Helper.GetKey();
                if (api is null)
                {
                        return new FormResponseModel
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
                // Dictionary<string, object> settings = new()
                // {
                //     ["isClosed"] = false,
                //     //["closeDate"] = eventList["closeDate"],
                //     //["closeTime"] = eventList["closeTime"],
                //     ["closeTimezone"] = "Asia/Kolkata",
                //     ["closeMessageTitle"] = "Form Closed",
                //     ["closeMessageDescription"] = "The deadline has passed."
                // };
                // //eventList.Remove("closeDate");
                // //eventList.Remove("closeTime");

                PayloadModel payload = new(){
                    blocks = GenBlock.GenBlocks(eventList),
                    status = "PUBLISHED"
                    // settings = settings
                };

                Console.WriteLine(
                    JsonSerializer.Serialize(payload,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }));

                FormResponseModel response = await Requests.PostAsync(baseUrl, payload, header);
                int a = await Database.InsertForm(response, db);

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

    public Block(string blockType, string blockGroupType, Dictionary<string,object> blockPayload, string? blockGroupUuid, string? blockUuid)
    {
        type = blockType;
        groupType = blockGroupType;
        uuid = blockUuid is not null ? blockUuid : Helper.GenUUID();
        groupUuid = blockGroupUuid is not null ? blockGroupUuid : Helper.GenUUID();
        payload = blockPayload;
    }
    public static List<List<string>> SafeHtml(string text)
    {
        return [[text]];
    }
    public static List<Block> FormTitle(string title,string uuid)
    {
        Dictionary<string, object> payload = new ()
            {
                    ["safeHTMLSchema"] = SafeHtml(title),
                    ["title"] = title
            };
        return
        [
            new Block("FORM_TITLE", "TEXT", payload, uuid,null)
                
        ];
    }
    public static List<Block> Title(string text)
    {
        Dictionary<string, object> payload = new ()
        {
            ["safeHTMLSchema"] = SafeHtml(text)
        };
        return new List<Block>
        {
            new Block("TITLE","QUESTION",payload,null,null)
            
        };
    }
    public static List<Block> InputText(string placeholder,string uuid)
    {
        Dictionary<string, object> payload = new ()
        {
            ["placeholder"] = placeholder
        };
        List<Block> block =
        [
            .. Title(placeholder),
            new Block("INPUT_TEXT","INPUT_TEXT", payload, uuid, null)
        ];
        return block;
    }
    public static List<Block> InputNumber(string placeholder,string uuid)
    {
        Dictionary<string, object> payload = new()
        {
            ["placeholder"] = placeholder
        };
        List<Block> block =
        [
            .. Title(placeholder),
            new Block("INPUT_NUMBER", "INPUT_NUMBER", payload,uuid, null)            
        ];
        return block;
    }
    public static List<Block> Checkbox(string name, List<string> events,string uuid)
    {
        
        List<Block> mainBlock = [.. Title(name)];

        for(int index = 0; index < events.Count; index++){
            Dictionary<string, object> payload = new ()
            {
                ["index"] = index,
                ["isFirst"] = index == 0,
                ["isLast"] = index == events.Count - 1,
                ["text"] = events[index]
            };
            Block block = new Block("CHECKBOX", "CHECKBOXES", payload, uuid, null);
            mainBlock.Add(block);
        };
        return mainBlock;
    }
    public static List<Block> Dropdown(string name, List<string> options,string uuid)
    {
        List<Block> mainBlock = [.. Title(name)];

        for(int index = 0; index < options.Count; index++)
        {   
            Dictionary<string, object> payload = new ()
                {
                    ["isRequired"] = true,
                    ["index"] = index,
                    ["isFirst"] = index == 0,
                    ["isLast"] = index == options.Count - 1,
                    ["text"] = options[index]
                };
            Block block = new Block("DROPDOWN_OPTION","DROPDOWN", payload, uuid, null);
            mainBlock.Add(block);
        };
    return mainBlock;
    }
    public static List<Block> PageBreak(string pageUUID)
    {
        Dictionary<string, object> payload= new ()
            {
                ["button"] = new Dictionary<string, object>
                {
                    ["label"] = "Submit"
                }
            };
        return
        [
            new Block("PAGE_BREAK","PAGE_BREAK",payload,pageUUID,pageUUID)
        ];
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
        object finalValue = value;
        if(value is not null){
            if (int.TryParse(value, out int number))
                finalValue = number;
            else
                finalValue = value;
        }

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
        public static Dictionary<string, object> JumpToPage(string? pageUuid)
        {
            return new Dictionary<string, object>
            {
                ["uuid"] = Helper.GenUUID(),
                ["type"] = "JUMP_TO_PAGE",
                ["payload"] = new Dictionary<string, object>
                {
                    ["jumpToPage"] = pageUuid is not null ? pageUuid : 0
                }
            };
        }
        public static Dictionary<string, object> HideButton(string? pageUuid)
        {
            return new Dictionary<string, object>
            {
                ["uuid"] = Helper.GenUUID(),
                ["type"] = "HIDE_BUTTON_TO_DISABLE_COMPLETION"
            };
        }

        
    }

    // Main Condition Creation Function
    public static List<Block> ConditionalLogic(
    List<Dictionary<string, object>> conditions,
    List<Dictionary<string, object>> actions,
    string logicalOperator = "AND")
    {
        Dictionary<string, object> payload = new ()
        {
            ["logicalOperator"] = logicalOperator,
            ["conditionals"] = conditions,
            ["actions"] = actions
        };
        return [
            new Block("CONDITIONAL_LOGIC", "CONDITIONAL_LOGIC", payload, null, null)  
        ];
    }
    


    public static List<Block> ConditionOperation(List<List<Dictionary<string,List<string>>>> conditionList, List<Block> blocks, Dictionary<string,FormField> events)
    {
        List<Block> conditionBlock = [];
        Dictionary<string,Func<string,Dictionary<string, object>>> operations = new()
        {
            {"JumpToPage", Values => Actions.JumpToPage(Values)},
            {"HideButton", Values => Actions.HideButton(Values)}
        };

        foreach (var group in conditionList)
        {
            List<Dictionary<string, object>> condition = [];
            List<Dictionary<string, object>> action = [];
            
            foreach (var dict in group)
            {
                foreach (var kvp in dict)
                {
                    if(kvp.Key != "Action"){
                        string fieldUuid = kvp.Value[0];
                        string comparison = kvp.Value[1];
                        string rawValue = kvp.Value[2];

                        Block? block = blocks.FirstOrDefault(b => b.groupUuid == fieldUuid);

                        if (block == null)
                            {
                                throw new Exception(
                                    $"Condition references field UUID '{fieldUuid}', " +
                                    "but no generated block with that groupUuid exists."
                                );
                            }
                        string? title = events
                        .FirstOrDefault(x => x.Value.groupUUID?.ToString() == fieldUuid)
                        .Key;

                        if (title== null)
                            {
                                throw new Exception(
                                    $"Could not find FormField with groupUUID '{fieldUuid}'."
                                );
                            }
                        condition.AddRange(CreateConditionals(fieldUuid, title, block.type, comparison, rawValue));
                    }
                    else
                    {
                        string ActionToDo = kvp.Value[0];
                        string value = kvp.Value.Count > 1 ? kvp.Value[1] : null!;
                        action.AddRange(operations[ActionToDo](value));
                    }
                }
            }
            conditionBlock.AddRange(ConditionalLogic(condition ,action));
        }

        return conditionBlock;
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
}



// {
//   "PageBreakGen": {
//     "type": "PageBreak",
//     "options": [
//       "d6a9f7d3-5b14-4b58-95cf-0cb6d0d6d5c1",
//       "9e1b2f8c-7f53-4b97-a4c2-6d1e2f8a7b34",
//       "f3c4d98e-12b6-49f5-89c4-3e9a5d1f7b28"
//     ]
//   },
//   "Student Name": {
//     "type": "FormTitle",
//     "options": null,
//     "groupUUID": "2a7e5c91-6d43-4f8e-b2d1-9c5e7a3f8d62"
//   },
//   "Full Name": {
//     "type": "InputText",
//     "options": null,
//     "groupUUID": "b8f31d5a-9a27-4c6e-8d74-1f3b9e2c6a85"
//   },
//   "Age": {
//     "type": "InputNumber",
//     "options": null,
//     "groupUUID": "4c92e1b7-3d68-4a9f-b5c1-8e2d7f4a9b13"
//   },
//   "AgeCondition": {
//     "type": "Condition",
//     "options": null,
//     "groupUUID": "4c92e1b7-3d68-4a9f-b5c1-8e2d7f4a9b13",
//     "conditions": [
//       [
//         {
//           "condition1": [
//             "4c92e1b7-3d68-4a9f-b5c1-8e2d7f4a9b13",
//             "GREATER_THAN",
//             "0"
//           ]
//         },
//         {
//           "condition2": [
//             "4c92e1b7-3d68-4a9f-b5c1-8e2d7f4a9b13",
//             "LESS_OR_EQUAL_THAN",
//             "4"
//           ]
//         },
//         {
//           "Action": [
//             "JumpToPage","9e1b2f8c-7f53-4b97-a4c2-6d1e2f8a7b34"
//           ]
//         }
//       ],
//       [
//         {
//           "condition1": [
//             "4c92e1b7-3d68-4a9f-b5c1-8e2d7f4a9b13",
//             "GREATER_THAN",
//             "4"
//           ]
//         },
//         {
//           "condition2": [
//             "4c92e1b7-3d68-4a9f-b5c1-8e2d7f4a9b13",
//             "LESS_OR_EQUAL_THAN",
//             "7"
//           ]
//         },
//         {
//           "Action": [
//             "JumpToPage",
//             "f3c4d98e-12b6-49f5-89c4-3e9a5d1f7b28"
//           ]
//         }
//       ]
//     ]
//   },
//   "Phone Number": {
//     "type": "InputText",
//     "options": null,
//     "groupUUID": "71d8a3f5-c94e-4b6d-91f8-5a2c7e3d8b46"
//   },
//   "Class": {
//     "type": "Checkbox",
//     "options": [
//       "1",
//       "2",
//       "3",
//       "4"
//     ],
//     "groupUUID": "ce4b7f91-8d25-4e3a-a6c9-2f7d1b5e9a73"
//   },
//   "Department": {
//     "type": "Dropdown",
//     "options": [
//       "CSE",
//       "ECE",
//       "EEE",
//       "ME"
//     ],
//     "groupUUID": "18f5c2d7-a96b-4d81-b3e7-7c4a9f2d5b18"
//   },
//     "9e1b2f8c-7f53-4b97-a4c2-6d1e2f8a7b34": {
//     "type": "PageBreak",
//     "options": null,
//     "groupUUID": "9e1b2f8c-7f53-4b97-a4c2-6d1e2f8a7b34"
//   },
//   "page2": {
//     "type": "Title",
//     "options": null,
//     "groupUUID": "9e1b2f8c-7f53-4b97-a4c2-6d1e2f8a7b34"
//   },
//   "Address": {
//     "type": "InputText",
//     "options": null,
//     "groupUUID": "a5d9e1c3-4f72-47b8-8c15-d2e7f9a6b341"
//   },
//   "Gender": {
//     "type": "Dropdown",
//     "options": [
//       "Male",
//       "Female",
//       "Other"
//     ],
//     "groupUUID": "3f8d7c2e-91a4-4b6f-8e2d-c5a7f9b13d84"
//   },
//     "f3c4d98e-12b6-49f5-89c4-3e9a5d1f7b28": {
//     "type": "PageBreak",
//     "options": null,
//     "groupUUID": "f3c4d98e-12b6-49f5-89c4-3e9a5d1f7b28"
//   },
//   "page3": {
//     "type": "Title",
//     "options": null,
//     "groupUUID": "f3c4d98e-12b6-49f5-89c4-3e9a5d1f7b28"
//   },
//   "Skills": {
//     "type": "Checkbox",
//     "options": [
//       "Python",
//       "C#",
//       "Java",
//       "JavaScript"
//     ],
//     "groupUUID": "b1e4a9d6-7c3f-4d82-a5e1-9f2b6c7d4a18"
//   },
//   "SkillCondition": {
//   "type": "Condition",
//   "options": null,
//   "groupUUID": "b1e4a9d6-7c3f-4d82-a5e1-9f2b6c7d4a18",
//   "conditions": [
//     [
//       {
//         "condition1": [
//           "b1e4a9d6-7c3f-4d82-a5e1-9f2b6c7d4a18",
//           "IS_NOT_EMPTY",
//           null
//         ]
//       },
//       {
//         "Action": [
//           "JumpToPage",null
//         ]
//       }
//     ],
//     [
//       {
//         "condition1": [
//           "b1e4a9d6-7c3f-4d82-a5e1-9f2b6c7d4a18",
//           "IS_EMPTY",
//           null
//         ]
//       },
//       {
//         "Action": [
//           "HideButton"
//         ]
//       }
//     ]
//   ]
// }
// }