using Microsoft.AspNetCore.Mvc.Rendering;

namespace StaffAtt.Web.Helpers;
public interface IDepartmentSelectListService
{
    Task<SelectList> GetDepartmentSelectListAsync(string defaultValue);
}