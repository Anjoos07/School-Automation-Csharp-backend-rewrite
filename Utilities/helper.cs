using DotNetEnv;
using System.Net.Http.Headers;
namespace Utilities;
public static class Helper
{
    public static string GenUUID()
    {
        return Guid.NewGuid().ToString();
    }

    public static string GetKey()
    {
        Env.Load(".env");
        string? apiKey = Environment.GetEnvironmentVariable("API_KEY");
        return apiKey;
    } 


}

public static class Requests{
    public static async Task<IResult> GetAsync(string url, Dictionary<string, string>? header = null)
    {

        HttpClient client = new HttpClient();
        if (header is not null)
        {
            foreach (var (key, value) in header)
            {
                client.DefaultRequestHeaders.Add(key, value);
            }
        }
        var response = await client.GetAsync(url);
        var json = await response.Content.ReadAsStringAsync();
        return Results.Text(json, "application/json");
    }

        static async Task<IResult> Get(string url, Dictionary<string, string>? header = null)
    {

        HttpClient client = new HttpClient();
        if (header is not null)
        {
            foreach (var (key, value) in header)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(key, value);
            }
        }
        var response = await client.GetAsync(url);
        var json = await response.Content.ReadAsStringAsync();
        return Results.Text(json, "application/json");
    }

}