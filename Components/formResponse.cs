using System.Buffers.Text;
using Microsoft.AspNetCore.Mvc;
using Utilities;

namespace Forms;

public static class FormResponse{
        public static async Task<IResult> FetchResponse(string formId)
        {
                string api = Helper.GetKey();
                if (api is null)
                {
                        return Results.InternalServerError("Api not found");
                }
                string baseUrl = $"https://api.tally.so/forms/{formId}/submissions";
                
                int limit = 500;

                Dictionary<string, string> header = new Dictionary<string, string>
                {
                        ["Authorization"] = $"Bearer {api}" 
                };

                var response = await Requests.GetAsync(baseUrl, header);
                // need to check for http error codes here
                

        }
        

}