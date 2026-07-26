namespace DbModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("kdrrod", Schema = "responses")]
public class Kdrrod
{
    [Key]
    [Column("submission_id")]
    public string SubmissionId { get; set; } = null!;
    [Column("respondent_id")]
    public string? RespondentId { get; set; }
    [Column("submitted_at")]
    public DateTime SubmittedAt { get; set; }
    [Column("name")]
    public string Name { get; set; } = null!;
    [Column("student_class")]
    public string? StudentClass { get; set; }
    [Column("division")]
    public string? Division { get; set; }
    [Column("roll_no")]
    public string? RollNo { get; set; }
    [Column("events")]
    public string? Events { get; set; }
    [Column("classification")]
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