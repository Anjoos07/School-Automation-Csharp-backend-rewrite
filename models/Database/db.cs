namespace DbModels;
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
}

[Table("field", Schema = "core")]
public class Field
{
    [Key]
    [Column("field_id")]
    public string FieldId { get; set; } = null!;
    [Column("form_id")]
    public string FormId { get; set; } = null!;
    [Column("field_name")]
    public string FieldName { get; set; } = null!;
    [Column("field_type")]
    public string FieldType { get; set; } = null!;
}

[Table("response", Schema = "core")]
public class Response
{
    [Key]
    [Column("form_id")]
    public string FormId { get; set; } = null!;
    [Column("submission_id")]
    public string SubId { get; set; } = null!;
    [Column("response")]
    public JsonElement response { get; set; };
}



