using System.Buffers.Text;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using Utilities;
using Request;
using Microsoft.OpenApi;
using System.ComponentModel;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Forms;

public static class FormResponse{
        public static async Task<FormResponseModel> FetchResponse(string formId)
        {
                string api = Helper.GetKey();
                if (api is null)
                {
                        return new FormResponseModel
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

                Dictionary<string, string> Params = new Dictionary<string, string>
                {
                        ["limit"] = Convert.ToString(limit)
                };

                FormResponseModel response = await Requests.GetAsync(baseUrl, header, parameters:Params, timeout:10);
                // need to check for http error codes here
                int totalResresponse = response.Json!["totalNumberOfSubmissionsPerFilter"]!["all"]!.GetValue<int>();
                int totalPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalResresponse / limit)));

                // JsonArray all_questions = response.Json!["questions"].AsArray();

                List<JsonObject> allQuestions = response.Json!["questions"]!
                .AsArray()
                .Select(x => x!.AsObject())
                .ToList();

                
                List<JsonObject> allSubmissions = response.Json!["submissions"]!
                .AsArray()
                .Select(x => x!.AsObject())
                .ToList();             

                for (int page = 2; page < totalPages + 1; page++)
                {       
                        Params = new Dictionary<string, string>
                        {
                                ["page"] = Convert.ToString(page),
                                ["limit"] = Convert.ToString(limit)
                        };

                         response = await Requests.GetAsync(baseUrl, header, parameters:Params, timeout:10);
                        if (!response.IsSuccess)
                                return response;
                        allSubmissions.AddRange(
                                response.Json!["submissions"]!
                                .AsArray()
                                .Select(x => x!.AsObject())
                        );  
                }

                if (allSubmissions.Count == 0)
                {
                        return new FormResponseModel
                        {
                                StatusCode = 400,
                                IsSuccess = false,
                                Text = "No Submissions Found"
                        };
                }

                JsonArray respondents =  ProcessResponse(allSubmissions,allQuestions);

                return new FormResponseModel
                {
                        StatusCode = 200,
                        IsSuccess = true,
                        Json = respondents
                };
        }

        public static JsonArray ProcessResponse(List<JsonObject> allSubmissions, List<JsonObject> allQuestions)
        {

                JsonArray respondents = new JsonArray();
                foreach (JsonNode? respondentDataNode in allSubmissions)
                {
                        JsonObject respondentData = respondentDataNode!.AsObject();
                        if (!respondentData["isCompleted"]!.GetValue<bool>())
                        continue;

                        JsonObject respondent = new JsonObject
                        {
                        ["submissionID"] = respondentData["id"]?.DeepClone(),
                        ["respondentID"] = respondentData["respondentId"]?.DeepClone(),
                        ["submittedAt"] = respondentData["submittedAt"]?.DeepClone()
                        };

                        JsonArray responses = new JsonArray();

                        foreach (JsonNode? questionNode in respondentData["responses"]!.AsArray())
                        {
                        JsonObject question = questionNode!.AsObject();
                        JsonObject response = new JsonObject();

                        foreach (JsonNode? questionDataNode in allQuestions)
                        {
                                JsonObject questionData = questionDataNode!.AsObject();

                                if (questionData["id"]!.GetValue<string>() ==
                                question["questionId"]!.GetValue<string>())
                                {
                                response["title"] = questionData["title"]?.DeepClone();
                                break;
                                }
                        }

                        response["answer"] = question["answer"]?.DeepClone();
                        responses.Add(response);
                        }

                        respondent["responses"] = responses;
                        respondents.Add(respondent);
                }

                return respondents;
        }
        
}
        

