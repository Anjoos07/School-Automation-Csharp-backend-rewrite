namespace Utilities;

public static class Helper
{
    public static string GenUUID()
    {
        return Guid.NewGuid().ToString();
    }
}