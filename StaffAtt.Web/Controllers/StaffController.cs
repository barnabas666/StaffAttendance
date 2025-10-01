using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StaffAtt.Web.Helpers;
using StaffAtt.Web.Models;
using StaffAttLibrary.Data;
using StaffAttLibrary.Models;
using StaffAttShared.DTOs;
using System.Net.Http.Headers;

namespace StaffAtt.Web.Controllers;

/// <summary>
/// Controller for basic Staff's CRUD Actions
/// </summary>
[Authorize]
public class StaffController : Controller
{
    private HttpClient httpClient;
    private readonly IApiClient _apiClient;
    private readonly IStaffService _staffService;
    private readonly IUserService _userService;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IPhoneNumberParser _phoneNumberParser;
    private readonly IDepartmentSelectListService _departmentService;

    public StaffController(IHttpClientFactory httpClientFactory,
                           IApiClient apiClient,
                           IStaffService staffService,
                           IUserService userService,
                           IUserContext userContext,
                           IMapper mapper,
                           IPhoneNumberParser phoneNumberParser,
                           IDepartmentSelectListService departmentService)
    {
        httpClient = httpClientFactory.CreateClient("api");
        _apiClient = apiClient;
        _staffService = staffService;
        _userService = userService;
        _userContext = userContext;
        _mapper = mapper;
        _phoneNumberParser = phoneNumberParser;
        _departmentService = departmentService;
    }

    /// <summary>
    /// This is Get Create Action. We call this after new Staff register his email. We send 
    /// StaffCreateModel populated with collection of SelectList DepartmentItems (Departments info)
    /// to View which modifies StaffCreateModel properties and send back.
    /// </summary>
    /// <returns>View with populated StaffCreateModel.</returns>
    public async Task<IActionResult> Create()
    {
        // Model to send to our View, populate it there and send model back.
        StaffCreateViewModel model = new StaffCreateViewModel();
        model.DepartmentItems = await _departmentService.GetDepartmentSelectListAsync(String.Empty);

        return View("Create", model);
    }

    /// <summary>
    /// This is Post Create Action. We use form to create new Staff and save it to database.
    /// We get Phone Numbers from PhoneNumbersText string property and Email from currently logged User info.
    /// Than we assign new Staff to Member Role. 
    /// We use Custom Action Filter (class ValidateModelAttribute) to validate model state - [ValidateModel].
    /// </summary>
    /// <param name="staff">Staff information.</param>
    /// <returns>Redirect to Details Action.</returns>
    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> Create(StaffCreateViewModel staff)
    {
        string userEmail = _userContext.GetUserEmail();
        // check if user has already created Staff account
        bool isCreated = await _staffService.CheckStaffByEmailAsync(userEmail);
        // if user has already created account we redirect him to Details action
        // unreachable code, but we must check it in case someone get to Create action using URL
        if (isCreated)
            return RedirectToAction("Details", new { message = "You have already created account!" });

        List<PhoneNumberModel> phoneNumbers = _phoneNumberParser.ParseStringToPhoneNumbers(staff.PhoneNumbersText);

        AddressModel address = _mapper.Map<AddressModel>(staff.Address);

        await _staffService.CreateStaffAsync(Convert.ToInt32(staff.DepartmentId),
                                   address,
                                   staff.PIN.ToString(),
                                   staff.FirstName,
                                   staff.LastName,
                                   userEmail,
                                   phoneNumbers);

        IdentityUser newUser = await _userService.FindByEmailAsync(userEmail);
        await _userService.AddToRoleAsync(newUser, "Member");
        await _userService.SignInAsync(newUser, false);

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
        string userEmail = _userContext.GetUserEmail();

        // 1. Check if staff exists
        var existsResult = await _apiClient.GetAsync<bool>($"staff/check-email?emailAddress={Uri.EscapeDataString(userEmail)}");
        if (!existsResult.IsSuccess)
            return View("Error", existsResult.ErrorMessage);

        if (!existsResult.Value)
            return RedirectToAction("Create");

        // 2. Get staff details
        var detailsResult = await _apiClient.GetAsync<StaffFullModel>($"staff/email?emailAddress={Uri.EscapeDataString(userEmail)}");
        if (!detailsResult.IsSuccess || detailsResult.Value is null)
            return View("Error", detailsResult.ErrorMessage ?? "Staff details could not be loaded.");

        // 3. Map to view model
        var detailsModel = _mapper.Map<StaffDetailsViewModel>(detailsResult.Value);
        detailsModel.Message = message;

        return View("Details", detailsModel);
    }


    /// <summary>
    /// Get Update Action. We get Staff's personal info and populate StaffUpdateModel with it.
    /// </summary>
    /// <returns>View with populated StaffUpdateModel.</returns>
    public async Task<IActionResult> Update()
    {
        string userEmail = _userContext.GetUserEmail();

        StaffFullModel fullModel = await _staffService.GetStaffByEmailAsync(userEmail);

        StaffUpdateViewModel updateModel = _mapper.Map<StaffUpdateViewModel>(fullModel);

        updateModel.PhoneNumbersText = _phoneNumberParser.ParsePhoneNumbersToString(fullModel.PhoneNumbers);

        return View("Update", updateModel);
    }

    /// <summary>
    /// Post Update Action. We update Staff's personal info.
    /// We use Custom Action Filter (class ValidateModelAttribute) to validate model state - [ValidateModel].
    /// </summary>
    /// <param name="updateModel">Staff Updated Information.</param>
    /// <returns>Redirect to Details Action.</returns>
    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> Update(StaffUpdateViewModel updateModel)
    {
        string userEmail = _userContext.GetUserEmail();

        List<PhoneNumberModel> phoneNumbers = _phoneNumberParser.ParseStringToPhoneNumbers(updateModel.PhoneNumbersText);

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
