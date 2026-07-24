using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.VisualBasic;
using Superpower.Model;
using System.Net.Http.Headers;
using Utilities;


namespace Forms;


public static class FormOperations
{
    public static void MapFormOperationsEndpoints(this WebApplication app){
        app.MapGet("/form/{formId}", async (string formId) =>
        {
            return Results.Ok(await Requests.GetAsync($"https://api.tally.so/forms/{formId}"));
        });
        app.MapGet("/form", async () =>
        {
            return Results.Ok(await Requests.GetAsync($"https://api.tally.so/forms"));
        });
        app.MapGet("/form-question/{formId}", async (string formId) =>
        {
            return Results.Ok(await Requests.GetAsync($"https://api.tally.so/forms/{formId}/questions"));
        });
        app.MapGet("/form-delete-submission/{formId}/{submissionId}",async (string formId, string submissionId) =>
        {
            return Results.Ok(await Requests.GetAsync($"https://api.tally.so/forms/{formId}/submissions/{submissionId}"));
        });
        app.MapPost("/form-creation", async () =>
        {
            return await FormCreation.CreateForm();
        });
        app.MapGet("/fetch-response/{formId}", async (string formId) =>
        {
            return await FormResponse.FetchResponse(formId);            
        });
    }
}