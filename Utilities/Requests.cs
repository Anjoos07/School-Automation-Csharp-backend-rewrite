using Forms;
using System.Text.Json.Nodes;

namespace Request;


public static class Requests{
    
    // Get Operation
    public static async Task<Response> GetAsync(string url, Dictionary<string, string>? header = null, Dictionary<string, string>? parameters = null, int timeout = 0)
    {
        HttpClient client = new HttpClient();

        if (parameters is not null)
        {
            var query = string.Join("&",
            parameters.Select(p =>
                $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
            url += "?" + query;
        }

        if (timeoutSeconds is not 0)
        {
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        }

        if (header is not null)
        {
            foreach (var (key, value) in header)
            {
                client.DefaultRequestHeaders.Add(key, value);
            }
        }
        HttpResponseMessage response = await client.GetAsync(url);
        string json = await response.Content.ReadAsStringAsync();
        string data = await response.Content.ReadAsStringAsync();

        return new Response
        {
            StatusCode = (int)response.StatusCode,
            IsSuccess = response.IsSuccessStatusCode,
            Json = JsonNode.Parse(data)
        };
    }

    //Post Operation
    static async Task<Response> PostAsync<T>(string url,T payload,Dictionary<string, string>? headers = null)
    {
        using HttpClient client = new();

        if (headers != null)
        {
            foreach (var (key, value) in headers)
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
            }
        }

        HttpResponseMessage response = await client.PostAsJsonAsync(url, payload);

        string data = await response.Content.ReadAsStringAsync();

        return new Response
        {
            StatusCode = (int)response.StatusCode,
            IsSuccess = response.IsSuccessStatusCode,
            Json = JsonNode.Parse(data)
        };
    }

}