namespace StaffAtt.Web.Services;

public interface IUserContext
{
    string GetUserEmail();
    IEnumerable<string> GetRoles();
}
