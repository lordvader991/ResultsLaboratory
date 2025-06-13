namespace CassandraJwtAuth.Models;

public class Order
{
    public int PatientId { get; set; }
    public int TestTypeId { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
}
