using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    /// <summary>
    /// This is Get Create Action. We call this after new Staff register his email. We send StaffCreateModel populated 
    /// with collection of SelectList DepartmentItems (Departments info) to View which modifies StaffCreateModel properties and send back.
    /// </summary>
    /// <returns>View with populated StaffCreateModel inside.</returns>
    public async Task<IActionResult> Create()
    {
        // We get all Departments from our database.
        List<DepartmentModel> departments = await _sqlData.GetAllDepartments();

        // Model to send to our View, populate it there and send model back.
        StaffCreateModel model = new StaffCreateModel();

        // Source is departments, value (Id here) gonna be saved to database, Text (Title) gets displayed to user, both expect string.
        model.DepartmentItems = new SelectList(departments, nameof(DepartmentModel.Id), nameof(DepartmentModel.Title));

        return View(model);
    }

    /// <summary>
    /// This is Post Create Action. We use form to create new Staff and save it to database.
    /// We get Phone Numbers from PhoneNumbersText string property and Email from currently logged User info.
    /// </summary>
    /// <param name="staff">Staff information.</param>
    /// <returns>Redirect to Details Action.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(StaffCreateModel staff)
    {
        // this is validation for model we are passing as parameter
        if (ModelState.IsValid == false)
        {            
            return View(staff);
        }

        List<string> phoneNumbers = new List<string>();

        string[] cols = staff.PhoneNumbersText.Split(',');

        for (int i = 0; i < cols.Length; i++)
        {
            phoneNumbers.Add(cols[i]);
        }

        string userEmail = User.FindFirst(ClaimTypes.Email).Value;

        await _sqlData.CreateStaff(Convert.ToInt32(staff.DepartmentId),
                                   staff.Street,
                                   staff.City,
                                   staff.Zip,
                                   staff.State,
                                   staff.PIN.ToString(),
                                   staff.FirstName,
                                   staff.LastName, 
                                   userEmail, 
                                   phoneNumbers);

        return RedirectToAction("Details");
    }

    /// <summary>
    /// Get Details Action. Display Staff's personal info. Homepage for Staff.
    /// </summary>
    /// <returns>View with populated StaffDetailsModel inside.</returns>
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

        // maybe we could use AutoMapper here
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

        return View(detailsModel);
    }
}
