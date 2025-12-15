using Microsoft.AspNetCore.Mvc.Rendering;

namespace StaffAtt.Web.Services;
public interface IDepartmentSelectListService
{
    Task<SelectList> GetDepartmentSelectListAsync(string defaultValue);
}