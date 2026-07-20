using Microsoft.AspNetCore.Mvc.Routing;
using System.Net.Http.Headers;


namespace Forms;
public static class FormModel
{
    public static async Task<IResult> Model(string url)
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
    public static void MapFormCreationEndpoints(this WebApplication app){
        app.MapGet("/form/{formId}", async (string formId) =>
        {
            return await FormModel.Model($"https://api.tally.so/forms/{formId}");
        });
        app.MapGet("/form", async () =>
        {
            return await FormModel.Model($"https://api.tally.so/forms");
        });
        app.MapGet("/form-question/{formId}", async (string formId) =>
        {
            return await FormModel.Model($"https://api.tally.so/forms/{formId}/questions");
        });
        app.MapGet("/form-delete-submission/{formId}/{submissionId}",async (string formId, string submissionId) =>
        {
            return await FormModel.Model($"https://api.tally.so/forms/{formId}/submissions/{submissionId}");
        });
    }
}