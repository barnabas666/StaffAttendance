using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StaffAtt.Web.Helpers;
using StaffAtt.Web.Models;
using StaffAttShared.DTOs;

namespace StaffAtt.Web.Controllers;

/// <summary>
/// Controller for basic Staff's CRUD Actions
/// </summary>
[Authorize]
public class StaffController : Controller
{

    private readonly IApiClient _apiClient;
    private readonly IUserService _userService;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IPhoneNumberDtoParser _phoneNumberDtoParser;
    private readonly IDepartmentSelectListService _departmentService;

    public StaffController(IApiClient apiClient,
                           IUserService userService,
                           IUserContext userContext,
                           IMapper mapper,
                           IPhoneNumberDtoParser phoneNumberDtoParser,
                           IDepartmentSelectListService departmentService)
    {
        _apiClient = apiClient;
        _userService = userService;
        _userContext = userContext;
        _mapper = mapper;
        _phoneNumberDtoParser = phoneNumberDtoParser;
        _departmentService = departmentService;
    }

    /// <summary>
    /// This is Get Create Action. We call this after new Staff register his email. We send 
    /// StaffCreateModel populated with collection of SelectList DepartmentItems (Departments info)
    /// to View which modifies StaffCreateModel properties and send back.
    /// </summary>
    /// <returns>View with populated StaffCreateModel.</returns>
    [HttpGet]
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

        // 1. Check if user already has staff account
        var existsResult = await _apiClient.GetAsync<bool>(
            $"staff/check-email?emailAddress={Uri.EscapeDataString(userEmail)}"
        );

        if (!existsResult.IsSuccess)
            return View("Error", new ErrorViewModel { Message = existsResult.ErrorMessage });

        // if user has already created account we redirect him to Details action
        // unreachable code, but we must check it in case someone get to Create action using URL
        if (existsResult.Value)
            return RedirectToAction("Details", new { errorMessage = "You have already created account!" });

        // 2. Build request model for API
        List<PhoneNumberDto> phoneNumbers = _phoneNumberDtoParser.ParseStringToPhoneNumbers(staff.PhoneNumbersText);
        AddressDto address = _mapper.Map<AddressDto>(staff.Address);

        var request = new CreateStaffRequest
        {
            DepartmentId = Convert.ToInt32(staff.DepartmentId),
            Address = address,
            PIN = staff.PIN.ToString(),
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            EmailAddress = userEmail,
            PhoneNumbers = phoneNumbers
        };

        // 3. Call API to create staff
        var createResult = await _apiClient.PostAsync<CreateStaffRequest>("staff", request);
        if (!createResult.IsSuccess)
            return View("Error", new ErrorViewModel { Message = createResult.ErrorMessage });

        IdentityUser newUser = await _userService.FindByEmailAsync(userEmail);
        await _userService.AddToRoleAsync(newUser, "Member");
        await _userService.SignInAsync(newUser, false);

        return RedirectToAction("Details", new { successMessage = "Your staff profile was successfully created." });
    }

    /// <summary>
    /// Get Details Action. Display Staff's personal info. Starting Action of app.
    /// After Staff logs in using Identity we check if he has already created Staff account. 
    /// If not we redirect him to Create Staff action.
    /// </summary>
    /// <param name="message">Optional parameter. Some warning message from different Action.</param>
    /// <returns>View with populated StaffDetailsModel inside.</returns>
    [HttpGet]
    public async Task<IActionResult> Details(string? message = "", string? successMessage = "", string? errorMessage = "")
    {
        string userEmail = _userContext.GetUserEmail();

        // 1. Check if staff exists
        var existsResult = await _apiClient.GetAsync<bool>($"staff/check-email?emailAddress={Uri.EscapeDataString(userEmail)}");
        if (!existsResult.IsSuccess)
            return View("Error", new ErrorViewModel { Message = existsResult.ErrorMessage });

        if (!existsResult.Value)
            return RedirectToAction("Create");

        // 2. Get staff details
        var detailsResult = await _apiClient.GetAsync<StaffFullDto>($"staff/email?emailAddress={Uri.EscapeDataString(userEmail)}");
        if (!detailsResult.IsSuccess || detailsResult.Value is null)
            return View("Error", new ErrorViewModel { Message = detailsResult.ErrorMessage });

        // 3. Map to view model
        var detailsModel = _mapper.Map<StaffDetailsViewModel>(detailsResult.Value);
        detailsModel.Message = message;
        detailsModel.SuccessMessage = successMessage;
        detailsModel.ErrorMessage = errorMessage;

        return View("Details", detailsModel);
    }


    /// <summary>
    /// Get Update Action. We get Staff's personal info and populate StaffUpdateModel with it.
    /// </summary>
    /// <returns>View with populated StaffUpdateModel.</returns>
    [HttpGet]
    public async Task<IActionResult> Update()
    {
        string userEmail = _userContext.GetUserEmail();

        // 1. Call API to get staff by email
        var result = await _apiClient.GetAsync<StaffFullDto>($"staff/email?emailAddress={Uri.EscapeDataString(userEmail)}");

        if (!result.IsSuccess || result.Value is null)
            return View("Error", new ErrorViewModel { Message = result.ErrorMessage });

        // 2. Map DTO => ViewModel
        var updateModel = _mapper.Map<StaffUpdateViewModel>(result.Value);

        // 3. Convert phone numbers into comma-separated string for textbox
        updateModel.PhoneNumbersText = _phoneNumberDtoParser.ParsePhoneNumbersToString(result.Value.PhoneNumbers);

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

        List<PhoneNumberDto> phoneNumbers = _phoneNumberDtoParser.ParseStringToPhoneNumbers(updateModel.PhoneNumbersText);
        AddressDto address = _mapper.Map<AddressDto>(updateModel.Address);

        var request = new UpdateStaffRequest
        {
            Address = address,
            PIN = updateModel.PIN.ToString(),
            FirstName = updateModel.FirstName,
            LastName = updateModel.LastName,
            EmailAddress = userEmail,
            PhoneNumbers = phoneNumbers
        };

        var result = await _apiClient.PutAsync<UpdateStaffRequest>("staff", request);

        if (!result.IsSuccess)
            return View("Error", new ErrorViewModel { Message = result.ErrorMessage });

        return RedirectToAction("Details", new { successMessage = "Your profile was successfully updated." });
    }
}
