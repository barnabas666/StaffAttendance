﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StaffAtt.Web.Models;
using StaffAttLibrary.Data;
using StaffAttLibrary.Models;
using System.Security.Claims;

namespace StaffAtt.Web.Controllers;

/// <summary>
/// Controller for basic Staff's CRUD Actions
/// </summary>
[Authorize]
public class StaffController : Controller
{
    private readonly IStaffService _staffService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IMapper _mapper;

    public StaffController(IStaffService staffService,
                           UserManager<IdentityUser> userManager,
                           SignInManager<IdentityUser> signInManager,
                           IMapper mapper)
    {
        _staffService = staffService;
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
    }

    /// <summary>
    /// This is Get Create Action. We call this after new Staff register his email. We send 
    /// StaffCreateModel populated with collection of SelectList DepartmentItems (Departments info)
    /// to View which modifies StaffCreateModel properties and send back.
    /// </summary>
    /// <returns>View with populated StaffCreateModel.</returns>
    public async Task<IActionResult> Create()
    {
        // We get all Departments from our database.
        List<DepartmentModel> departments = await _staffService.GetAllDepartmentsAsync();

        // Model to send to our View, populate it there and send model back.
        StaffCreateViewModel model = new StaffCreateViewModel();

        // Source is departments, value (Id here) gonna be saved to database, Text (Title) gets displayed to user, both expect string.
        model.DepartmentItems = new SelectList(departments, nameof(DepartmentModel.Id), nameof(DepartmentModel.Title));

        return View(model);
    }

    /// <summary>
    /// This is Post Create Action. We use form to create new Staff and save it to database.
    /// We get Phone Numbers from PhoneNumbersText string property and Email from currently logged User info.
    /// Than we assign new Staff to Member Role.
    /// </summary>
    /// <param name="staff">Staff information.</param>
    /// <returns>Redirect to Details Action.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(StaffCreateViewModel staff)
    {
        if (ModelState.IsValid == false)
            return RedirectToAction("Create");

        string userEmail = User.FindFirst(ClaimTypes.Email).Value;
        // check if user has already created Staff account
        bool isCreated = await _staffService.CheckStaffByEmailAsync(userEmail);
        // if user has already created account we redirect him to Details action
        if (isCreated)
            return RedirectToAction("Details", new { message = "You have already created account!" });

        List<PhoneNumberModel> phoneNumbers = new List<PhoneNumberModel>();
        string[] cols = staff.PhoneNumbersText.Split(',');
        for (int i = 0; i < cols.Length; i++)
        {
            phoneNumbers.Add(new PhoneNumberModel { PhoneNumber = cols[i].Trim() });
        }

        AddressModel address = _mapper.Map<AddressModel>(staff.Address);

        await _staffService.CreateStaffAsync(Convert.ToInt32(staff.DepartmentId),
                                   address,
                                   staff.PIN.ToString(),
                                   staff.FirstName,
                                   staff.LastName,
                                   userEmail,
                                   phoneNumbers);

        IdentityUser newUser = await _userManager.FindByEmailAsync(userEmail);
        await _userManager.AddToRoleAsync(newUser, "Member");
        await _signInManager.SignInAsync(newUser, false);

        return RedirectToAction("Details");
    }

    /// <summary>
    /// Get Details Action. Display Staff's personal info. Starting Action of app.
    /// After Staff logs in using Identity we check if he has already created Staff account. 
    /// If not we redirect him to Create Staff action.
    /// </summary>
    /// <param name="message">Optional parameter. Some warning message from different Action.</param>
    /// <returns>View with populated StaffDetailsModel inside.</returns>
    public async Task<IActionResult> Details(string message = "")
    {
        string userEmail = User.FindFirst(ClaimTypes.Email).Value;

        // check if user has already created Staff account
        bool isCreated = await _staffService.CheckStaffByEmailAsync(userEmail);

        // if user has no account yet we redirect him to Create Staff action
        if (isCreated == false)
            return RedirectToAction("Create");

        StaffFullModel fullModel = await _staffService.GetStaffByEmailAsync(userEmail);

        StaffDetailsViewModel detailsModel = _mapper.Map<StaffDetailsViewModel>(fullModel);
        detailsModel.Message = message;

        return View(detailsModel);
    }

    /// <summary>
    /// Get Update Action. We get Staff's personal info and populate StaffUpdateModel with it.
    /// </summary>
    /// <returns>View with populated StaffUpdateModel.</returns>
    public async Task<IActionResult> Update()
    {
        string userEmail = User.FindFirst(ClaimTypes.Email).Value;

        StaffFullModel fullModel = await _staffService.GetStaffByEmailAsync(userEmail);

        StaffUpdateViewModel updateModel = _mapper.Map<StaffUpdateViewModel>(fullModel);

        foreach (PhoneNumberModel phoneNumber in fullModel.PhoneNumbers)
        {
            updateModel.PhoneNumbersText += phoneNumber.PhoneNumber + ",";
        }
        updateModel.PhoneNumbersText = updateModel.PhoneNumbersText.TrimEnd(',');

        return View(updateModel);
    }

    /// <summary>
    /// Post Update Action. We update Staff's personal info.
    /// </summary>
    /// <param name="updateModel">Staff Updated Information.</param>
    /// <returns>Redirect to Details Action.</returns>
    [HttpPost]
    public async Task<IActionResult> Update(StaffUpdateViewModel updateModel)
    {
        if (ModelState.IsValid == false)
            return RedirectToAction("Update");

        string userEmail = User.FindFirst(ClaimTypes.Email).Value;

        List<PhoneNumberModel> phoneNumbers = new List<PhoneNumberModel>();
        string[] cols = updateModel.PhoneNumbersText.Split(',');
        for (int i = 0; i < cols.Length; i++)
        {
            phoneNumbers.Add(new PhoneNumberModel { PhoneNumber = cols[i].Trim() });
        }

        AddressModel address = _mapper.Map<AddressModel>(updateModel.Address);

        await _staffService.UpdateStaffAsync(address,
                                   updateModel.PIN.ToString(),
                                   updateModel.FirstName,
                                   updateModel.LastName,
                                   userEmail,
                                   phoneNumbers);

        return RedirectToAction("Details");
    }
}
