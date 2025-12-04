namespace StaffAttApi.Services;

public interface IJwtService
{
    string GenerateToken(string userId, string email, IEnumerable<string> roles);
}
