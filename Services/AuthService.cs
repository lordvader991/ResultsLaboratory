using Cassandra;
using CassandraJwtAuth.Models;
using System.Security.Cryptography;
using System.Text;

namespace CassandraJwtAuth.Services;

public class AuthService
{
    private readonly Cassandra.ISession _session;

    public AuthService(Cassandra.ISession session)
    {
        _session = session;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
{
    var stmt = await _session.ExecuteAsync(
        new SimpleStatement("SELECT id, email, passwordhash FROM users_by_email WHERE email = ?", email)
    );
    var row = stmt.FirstOrDefault();
    if (row != null)
    {
        return new User
        {
            Id = row.GetValue<int>("id"),
            Email = row.GetValue<string>("email"),
            PasswordHash = row.GetValue<string>("passwordhash")
        };
    }
    return null;
}


    //public bool VerifyPassword(string password, string storedHash)
    //{
    // using var sha256 = SHA256.Create();
    //var hash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
    //return hash == storedHash;
    //}
    public bool VerifyPassword(string password, string storedPassword)
    {
        return password == storedPassword;
    }
}
