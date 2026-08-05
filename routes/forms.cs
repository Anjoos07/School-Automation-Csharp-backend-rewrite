using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.VisualBasic;
using Superpower.Model;
using System.Net.Http.Headers;
using Utilities;
using Request;
using Data;
using System.Text.Json;


namespace Forms;

public static class FormOperations
{
    public static void MapFormOperationsEndpoints(this WebApplication app){
        app.MapGet("/form/{formId}", async (string formId) =>
        {
            return Results.Ok(await Requests.GetAsync($"https://api.tally.so/forms/{formId}",Header()));
        });
        app.MapGet("/form", async () =>
        {
            return Results.Ok(await Requests.GetAsync($"https://api.tally.so/forms",Header()));
        });
        app.MapGet("/form-question/{formId}", async (string formId) =>
        {
            return Results.Ok(await Requests.GetAsync($"https://api.tally.so/forms/{formId}/questions",Header()));
        });
        app.MapGet("/form-delete-submission/{formId}/{submissionId}",async (string formId, string submissionId) =>
        {
            return Results.Ok(await Requests.GetAsync($"https://api.tally.so/forms/{formId}/submissions/{submissionId}",Header()));
        });
        app.MapPost("/form-creation", async (Dictionary<string, FormField> eventList, AppDbContext db) =>
        {
            return await FormCreation.CreateForm(eventList, db);
        });
        app.MapGet("/form/{formId}/fields", async (string formId, AppDbContext db) =>
        {
           return await Database.getFields(formId, db); 
        });
        app.MapPut("/form/{formId}/fields/grouping", async (string formId, List<string> priority, AppDbContext db) =>
        {
           return await Database.SetGroupingPriority(formId, priority, db); 
        });
        app.MapGet("/form/{formId}/submissions", async (string formId,AppDbContext db) =>
        {
            // await Database.InsertSubmission(formId, db);
            return await Database.GetSubmissionsDb(formId, db);            
        });
        app.MapGet("/form/{formId}/submissions/{submissionId}", async (string formId, string submissionID, AppDbContext db) =>
        {
            return await Database.GetSubmissionsDb(formId, submissionID, db);  
        });
        app.MapPatch("/form/{formid}/submission/{submissionId}", async (string formId, string submissionId, JsonElement responseData, AppDbContext db) =>
        {
           return await Database.ModifySubmissionDb(formId, submissionId, responseData, db);
        });
    }
    
    public static Dictionary<string, string> Header()
    {
        string api = Helper.GetKey();
        if (api is null)
        {
            return new Dictionary<string, string>
            {
                ["StatusCode"] = "0",
                ["IsSuccess"] = "false",
                ["Text"] = "API key not found"
            };
        }

        Dictionary<string, string> header = new Dictionary<string, string>
        {
            ["Authorization"] = $"Bearer {api}" 
        };
        return header;
    }
}