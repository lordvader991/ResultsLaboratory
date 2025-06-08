namespace FieldsService.Models;

public class Field
{
    public int TestTypeId { get; set; }
    public int FieldId { get; set; }
    public string FieldName { get; set; }
    public string ReferenceRange { get; set; }
    public string Unit { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
