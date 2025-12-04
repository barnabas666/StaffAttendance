namespace StaffAtt.Web.Helpers;

public interface IUserContext
{
    string GetUserEmail();
    IEnumerable<string> GetRoles();
}
