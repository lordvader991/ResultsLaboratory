namespace CassandraJwtAuth.Models;

public class User
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Specialty { get; set; }
    public string? Address { get; set; }
    public string? LicenseNumber { get; set; }
    public string? PasswordHash { get; set; }
}
