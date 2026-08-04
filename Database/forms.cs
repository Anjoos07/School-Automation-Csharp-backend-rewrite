using System.Text.Json.Nodes;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using DbModelForms;
using Forms;
using Request;
using Utilities;
using Microsoft.VisualBasic;

namespace Data;


public class Database
{
    public static async Task<int> InsertForm(FormResponseModel response, AppDbContext db)
    {
        string? formId = response.Json?["id"]?.ToString();
        string? name = response.Json?["name"]?.ToString();
        bool isClosed = response.Json?["isClosed"]?.GetValue<bool>() ?? false;

        var form = new Form
        {
            FormId = formId!,
            FormName = name!,
            FormClosed = isClosed
        };

        db.Forms.Add(form);
        await db.SaveChangesAsync();

        Form? form_data = await db.Forms.FindAsync(formId);
        if (form_data != null)
        {
            Console.WriteLine(form.FormName);
            Console.WriteLine(form.FormClosed);
        }
        
        int a = await InsertFields(formId!, db);
        return 0;
    }

    public static async Task<int> InsertFields(string formId, AppDbContext db)
    {
        string api = Helper.GetKey();

        Dictionary<string, string> header = new Dictionary<string, string>
        {
            ["Authorization"] = $"Bearer {api}" 
        };
        FormResponseModel response = await Requests.GetAsync($"https://api.tally.so/forms/{formId}/questions",header);

        JsonArray? questions = response.Json?["questions"]?.AsArray();

        foreach (JsonNode question in questions!)
        {
            var field = new Field
            {
              FieldId = question?["id"]?.ToString()!,
              FormId = question?["formId"]?.ToString()!,
              FieldName = question?["title"]?.ToString()!,
              FieldType = question?["type"]?.ToString()!
            };
            db.Fields.Add(field);
            await db.SaveChangesAsync();      

        }
        List<Field> fields = await db.Fields.ToListAsync();
        foreach (var field1 in fields)
        {
             Console.WriteLine(field1.FieldId);
        }  
        return 0;
    }
    public static async Task<int> InsertResponse(string formId, AppDbContext db)
    {

        FormResponseModel response = await FormResponse.FetchResponse(formId);
        JsonArray respondents = response.Json!.AsArray(); 

        foreach(JsonNode respondent in respondents!)
        {
            var response_data = new Response
            {
                SubmissionId = respondent!["submissionID"]!.ToString(),
                RespondentId = respondent!["respondentID"]!.ToString(),
                ResponseData = JsonSerializer.Deserialize<JsonElement>(respondent!["responses"]!.ToJsonString()),
                SubmittedAt = DateTime.Parse(respondent!["submittedAt"]!.ToString()),
                FormId = formId
                
            };
            db.Responses.Add(response_data);
            await db.SaveChangesAsync();
        }


        return 0;
    }
}

public class Function
{
    
}