using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.VisualBasic;
using System.Net.Http.Headers;


namespace Forms;


public static class FormOperations
{
    public static void MapFormOperationsEndpoints(this WebApplication app){
        app.MapGet("/form/{formId}", async (string formId) =>
        {
            return await FormModel.GetModel($"https://api.tally.so/forms/{formId}");
        });
        app.MapGet("/form", async () =>
        {
            return await FormModel.GetModel($"https://api.tally.so/forms");
        });
        app.MapGet("/form-question/{formId}", async (string formId) =>
        {
            return await FormModel.GetModel($"https://api.tally.so/forms/{formId}/questions");
        });
        app.MapGet("/form-delete-submission/{formId}/{submissionId}",async (string formId, string submissionId) =>
        {
            return await FormModel.GetModel($"https://api.tally.so/forms/{formId}/submissions/{submissionId}");
        });
        app.MapPost("/form-creation", async () =>
        {
            return FormCreation.CreateForm();
        });
    }
}