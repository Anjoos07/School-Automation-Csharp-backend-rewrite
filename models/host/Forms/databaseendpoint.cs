namespace FormHost;

public class FieldResponse
{
    public string FieldId { get; set; } = null!;
    public string FieldName { get; set; } = null!;
    public string FieldType { get; set; } = null!;
    public int? GroupingPriority { get; set; }
}

public class FieldGroupSet
{
    public List<int> GroupPriority {get; set;} = null!;
}