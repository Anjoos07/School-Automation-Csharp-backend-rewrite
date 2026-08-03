namespace DbModelForms;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

[Table("form", Schema = "core")]
public class Form
{
    [Key]
    [Column("form_id")]
    public string FormId { get; set; } = null!;

    [Column("form_name")]
    public string FormName {get; set; } = null!;
    [Column("form_closed")]
    public bool FormClosed {get; set;}
    public ICollection<Field> Fields { get; set; }
    = new List<Field>();
    public ICollection<Response> Responses { get; set; }
    = new List<Response>();
}

[Table("field", Schema = "core")]
public class Field
{
    [Key]
    [Column("field_id")]
    public string FieldId { get; set; } = null!;
    [ForeignKey(nameof(Form))]
    [Column("form_id")]
    public string FormId { get; set; } = null!;
    public Form Form { get; set; } = null!;
    [Column("field_name")]
    public string FieldName { get; set; } = null!;
    [Column("field_type")]
    public string FieldType { get; set; } = null!;
}

[Table("response", Schema = "responses")]
public class Response
{
    [Key]
    [Column("submission_id")]
    public string SubmissionId { get; set; } = null!;
    [Column("form_id")]
    public string FormId { get; set; } = null!;
    public Form Form { get; set; } = null!;
    [Column("submitted_at")]
    public DateTime SubmittedAt { get; set; }

    [Column("respondent_id")]
    public string? RespondentId { get; set; }

    [Column("response")]
    public JsonElement ResponseData { get; set; }
}


