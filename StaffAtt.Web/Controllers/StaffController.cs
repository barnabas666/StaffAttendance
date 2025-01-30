using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StaffAtt.Web.Models;
using StaffAttLibrary.Data;
using StaffAttLibrary.Db;
using StaffAttLibrary.Models;
using System.Security.Claims;

namespace StaffAtt.Web.Controllers;

[Authorize]
public class StaffController : Controller
{
    private readonly IDatabaseData _sqlData;

    public StaffController(IDatabaseData sqlData)
    {
        _sqlData = sqlData;
    }

    public async Task<IActionResult> Details()
    {
        string userEmail = User.FindFirst(ClaimTypes.Email).Value;

        // check if user has already created Staff account
        bool isCreated = await _sqlData.CheckStaffByEmail(userEmail);

        // if user has no account yet we redirect him to Create Staff action
        if (isCreated == false)
            return RedirectToAction("Create");

        StaffDetailsModel detailsModel = new StaffDetailsModel();

        StaffFullModel fullModel = await _sqlData.GetStaffByEmail(userEmail);

        if (fullModel != null)
        {
            detailsModel.FirstName = fullModel.FirstName;
            detailsModel.LastName = fullModel.LastName;
            detailsModel.EmailAddress = fullModel.EmailAddress;
            detailsModel.Alias = fullModel.Alias;
            detailsModel.IsApproved = fullModel.IsApproved;
            detailsModel.Description = fullModel.Description;
            detailsModel.Street = fullModel.Street;
            detailsModel.City = fullModel.City;
            detailsModel.Zip = fullModel.Zip;
            detailsModel.State = fullModel.State;
            detailsModel.PhoneNumbers = fullModel.PhoneNumbers;
        }     

        return View(detailsModel);
    }
}
