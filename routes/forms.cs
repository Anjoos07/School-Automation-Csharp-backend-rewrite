using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.VisualBasic;
using Superpower.Model;
using System.Net.Http.Headers;
using Utilities;
using Request;
using Data;


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
        app.MapGet("/fetch-response/{formId}", async (string formId) =>
        {
            return await FormResponse.FetchResponse(formId);            
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