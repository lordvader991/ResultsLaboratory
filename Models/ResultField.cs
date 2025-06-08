namespace ResultFieldsService.Models;

public class ResultField
{
    public int ResultFieldId { get; set; }
    public int ResultId { get; set; }
    public int TestTypeId { get; set; }
    public int FieldId { get; set; }
    public string Value { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
