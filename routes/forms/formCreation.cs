using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.VisualBasic;
using System.Net.Http.Headers;


namespace Forms;
public static class FormModel
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

    // For Post Operations
    public class Payload
    {
        public object Blocks { get; set; } = default!;
        public string Status { get; set; } = "";
        public object Settings { get; set; } = default!;
    }

    public static async Task<IResult> PostModel(string url, Payload payload)
    {
        var baseUrl = url;
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "tly-2DX13TTyswPNQm3m0sRLJRFgDJpWtxlY");
        var response = await client.PostAsJsonAsync(baseUrl,payload);
        var json = await response.Content.ReadAsStringAsync();
        return Results.Text(json, "application/json");
    }
}


public static class FormOperations
{
    public static void MapFormCreationEndpoints(this WebApplication app){
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
    }
}