using Microsoft.AspNetCore.Mvc.Rendering;
using StaffAtt.Web.Models;

namespace StaffAtt.Web.Helpers;
public interface IStaffSelectListService
{
    Task<SelectList> GetStaffSelectListAsync(CheckInDisplayAdminViewModel dateDisplayModel, string defaultValue = "");
}