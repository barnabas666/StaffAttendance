using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StaffAtt.Web.Helpers;
using StaffAtt.Web.Models;
using StaffAttLibrary.Data;
using StaffAttLibrary.Models;
using System.Net.Http.Headers;

namespace StaffAtt.Web.Controllers;

/// <summary>
/// Controller for basic Staff's CRUD Actions
/// </summary>
[Authorize]
public class StaffController : Controller
{
    private HttpClient httpClient;
    private readonly IStaffService _staffService;
    private readonly IUserService _userService;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IPhoneNumberParser _phoneNumberParser;
    private readonly IDepartmentSelectListService _departmentService;

    public StaffController(IHttpClientFactory httpClientFactory,                           
                           IStaffService staffService,
                           IUserService userService,
                           IUserContext userContext,
                           IMapper mapper,
                           IPhoneNumberParser phoneNumberParser,
                           IDepartmentSelectListService departmentService)
    {
        httpClient = httpClientFactory.CreateClient("api");
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
        // HttpContext is from Controller, here we get the stored token from session
        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
        {
            // Handle missing token (e.g., redirect to login)
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        string userEmail = _userContext.GetUserEmail();

        // 1. Check if user has already created Staff account (API call)
        var checkStaffResponse = await httpClient.GetAsync($"staff/check-email?emailAddress={Uri.EscapeDataString(userEmail)}");
        if (!checkStaffResponse.IsSuccessStatusCode)
        {
            var errorContent = await checkStaffResponse.Content.ReadAsStringAsync();
            string errorMsg = $"Failed to check staff account ({(int)checkStaffResponse.StatusCode} {checkStaffResponse.ReasonPhrase})";
            if (checkStaffResponse.Content.Headers.ContentType?.MediaType == "application/problem+json")
            {
                errorMsg += $"\nDetails: {errorContent}";
            }
            // Show error view or message
            return View("Error", errorMsg);
        }
        bool? isCreated = await checkStaffResponse.Content.ReadFromJsonAsync<bool>();
        if (isCreated is null)
        {
            return View("Error", "Unexpected response from staff/check-email endpoint.");
        }

        if (!isCreated.Value)
            return RedirectToAction("Create");

        // 2. Get StaffFullModel from API
        var getStaffResponse = await httpClient.GetAsync($"staff/email?emailAddress={Uri.EscapeDataString(userEmail)}");
        if (!getStaffResponse.IsSuccessStatusCode)
        {
            var errorContent = await getStaffResponse.Content.ReadAsStringAsync();
            string errorMsg = $"Failed to get staff details ({(int)getStaffResponse.StatusCode} {getStaffResponse.ReasonPhrase})";
            if (getStaffResponse.Content.Headers.ContentType?.MediaType == "application/problem+json")
            {
                errorMsg += $"\nDetails: {errorContent}";
            }
            return View("Error", errorMsg);
        }

        var fullModel = await getStaffResponse.Content.ReadFromJsonAsync<StaffFullModel>();
        if (fullModel == null)
        {
            return View("Error", "Staff details could not be loaded.");
        }

        StaffDetailsViewModel detailsModel = _mapper.Map<StaffDetailsViewModel>(fullModel);
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
