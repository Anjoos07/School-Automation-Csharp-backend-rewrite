using Microsoft.AspNetCore.Mvc.Routing;
using System.Net.Http.Headers;


namespace Forms;

public static class FormCreation
{
    public static void MapFormCreationEndpoints(this WebApplication app){
        app.MapGet("/form/{formId}", async (string formId) =>
        {
            HeaderModel header = new HeaderModel("Bearer {api_key}");
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "tly-2DX13TTyswPNQm3m0sRLJRFgDJpWtxlY");
            var response = await client.GetAsync($"https://api.tally.so/forms/{formId}");
            var json = await response.Content.ReadAsStringAsync();
            return Results.Text(json, "application/json");
        });
    }
}