using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.VisualBasic;
using System.Net.Http.Headers;


namespace Forms;
public static class FormGetModel
{
    // For Get Operations
    public static async Task<IResult> GetModel(string url)
    {
        var baseUrl = url;
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "tly-2DX13TTyswPNQm3m0sRLJRFgDJpWtxlY");
        var response = await client.GetAsync(baseUrl);
        var json = await response.Content.ReadAsStringAsync();
        return Results.Text(json, "application/json");
    }

}


public static class FormOperations
{
    public static void MapFormOperationsEndpoints(this WebApplication app){
        app.MapGet("/form/{formId}", async (string formId) =>
        {
            return await FormGetModel.GetModel($"https://api.tally.so/forms/{formId}");
        });
        app.MapGet("/form", async () =>
        {
            return await FormGetModel.GetModel($"https://api.tally.so/forms");
        });
        app.MapGet("/form-question/{formId}", async (string formId) =>
        {
            return await FormGetModel.GetModel($"https://api.tally.so/forms/{formId}/questions");
        });
        app.MapGet("/form-delete-submission/{formId}/{submissionId}",async (string formId, string submissionId) =>
        {
            return await FormGetModel.GetModel($"https://api.tally.so/forms/{formId}/submissions/{submissionId}");
        });
        app.MapPost("/form-creation", async () =>
        {
            return FormCreation.CreateForm();
        });
    }
}