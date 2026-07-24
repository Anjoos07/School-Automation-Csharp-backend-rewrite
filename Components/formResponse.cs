using System.Buffers.Text;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using Utilities;

namespace Forms;

public static class FormResponse{
        public static async Task<Response> FetchResponse(string formId)
        {
                string api = Helper.GetKey();
                if (api is null)
                {
                        return new Response
                        {
                                StatusCode = 0,
                                IsSuccess = false,
                                Text = "API key not found"
                        };
                }
                string baseUrl = $"https://api.tally.so/forms/{formId}/submissions";
                
                int limit = 500;

                Dictionary<string, string> header = new Dictionary<string, string>
                {
                        ["Authorization"] = $"Bearer {api}" 
                };

                Response response = await Requests.GetAsync(baseUrl, header);
                // need to check for http error codes here

                return response;
                

        }
        

}