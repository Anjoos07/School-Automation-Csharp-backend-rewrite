using DotNetEnv;
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
        return(apiKey);
    } 


}