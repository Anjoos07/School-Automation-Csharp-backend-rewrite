using DbModelForms;
using Forms;
using Request;
using Utilities;
namespace Data;


public class Database
{
    public static async Task<int> InsertForm(FormResponseModel response, AppDbContext db)
    {
        string? formId = response.Json?["id"]?.ToString();
        string? name = response.Json?["name"]?.ToString();
        bool isClosed = response.Json?["isClosed"]?.GetValue<bool>() ?? false;

        var form = new Form
        {
            FormId = formId!,
            FormName = name!,
            FormClosed = isClosed
        };

        db.Forms.Add(form);
        await db.SaveChangesAsync();

        Form? form_data = await db.Forms.FindAsync(formId);
        if (form_data != null)
        {
            Console.WriteLine(form.FormName);
            Console.WriteLine(form.FormClosed);
        }
        
        return 0;
    }

    public static async Task<int> InsertQuestions(string formId)
    {
        string api = Helper.GetKey();

        Dictionary<string, string> header = new Dictionary<string, string>
        {
            ["Authorization"] = $"Bearer {api}" 
        };
        FormResponseModel response = await Requests.GetAsync($"https://api.tally.so/forms/{formId}",header);

        return 0;
    }
}

public class Function
{
    
}