namespace ResultsService.Models;

public class Result
{
    public int ResultId { get; set; }
    public int OrderId { get; set; }
    public int PatientId { get; set; }
    public int TestTypeId { get; set; }
    public string? Status { get; set; }  // Opcional en el request, lo autogeneras como "Pendiente"
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
