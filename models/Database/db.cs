namespace DbModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("Kdrrod", Schema = "responses")]
public class Kdrrod
{
    public string SubmissionId { get; set; } = null!;

    public string? RespondentId { get; set; }

    public DateTime SubmittedAt { get; set; }

    public string Name { get; set; } = null!;

    public string? StudentClass { get; set; }

    public string? Division { get; set; }

    public string? RollNo { get; set; }

    public string? Events { get; set; }

    public string? Classification { get; set; }
}


[Table("forms", Schema = "core")]
public class Form
{
    [Key]
    [Column("form_id")]
    public string FormId { get; set; } = null!;

    [Column("event_title")]
    public string? EventTitle { get; set; }

    [Column("lp_events")]
    public string? LpEvents { get; set; }

    [Column("up_events")]
    public string? UpEvents { get; set; }

    [Column("hs_events")]
    public string? HsEvents { get; set; }

    [Column("hss_events")]
    public string? HssEvents { get; set; }
}