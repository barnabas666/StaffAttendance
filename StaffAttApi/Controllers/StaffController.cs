using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StaffAttLibrary.Data;
using StaffAttLibrary.Models;
using StaffAttShared.DTOs;
using StaffAttShared.Enums;

namespace StaffAttApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Require JWT for all endpoints
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;
    private readonly ILogger<StaffController> _logger;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;

    public StaffController(IStaffService staffService, ILogger<StaffController> logger, IMapper mapper, IConfiguration config)
    {
        _staffService = staffService;
        _logger = logger;
        _mapper = mapper;
        _config = config;
    }

    /// <summary>
    /// Applies response caching headers based on configuration settings inside appsettings.json.
    /// Atm short is 10 seconds, medium is 30 seconds, long is 120 seconds.
    /// </summary>
    /// <param name="level"></param>
    private void ApplyResponseCache(string level)
    {
        int duration = _config.GetValue<int>($"Caching:{level}");
        if (Response?.Headers != null)
        {
            Response.Headers["Cache-Control"] = $"public,max-age={duration}";
        }
    }

    // GET: api/staff/departments
    [HttpGet("departments")]
    public async Task<ActionResult<List<DepartmentDto>>> GetAllDepartments()
    {
        ApplyResponseCache("LongDuration");
        _logger.LogInformation("GET: api/Staff/departments");

        List<DepartmentModel> departments = await _staffService.GetAllDepartmentsAsync();
        List<DepartmentDto> departmentDtos = _mapper.Map<List<DepartmentDto>>(departments);

        _logger.LogInformation("Returned {Count} departments", departmentDtos.Count);
        return Ok(departmentDtos); 
    }

    // POST: api/staff
    [HttpPost]
    public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequest request)
    {
        _logger.LogInformation("POST api/staff - Creating staff (Email={Email}, DepartmentId={DepartmentId})",
                               request.EmailAddress,
                               request.DepartmentId);

        AddressModel address = _mapper.Map<AddressModel>(request.Address);
        List<PhoneNumberModel> phoneNumbers = _mapper.Map<List<PhoneNumberModel>>(request.PhoneNumbers);

        int staffId = await _staffService.CreateStaffAsync(
            request.DepartmentId,
            address,
            request.PIN,
            request.FirstName,
            request.LastName,
            request.EmailAddress,
            phoneNumbers);

        _logger.LogInformation("Staff created successfully (StaffId={StaffId}, Email={Email})",
                               staffId,
                               request.EmailAddress);
        return Ok(new { StaffId = staffId });
    }

    // PUT: api/staff
    [HttpPut]
    public async Task<IActionResult> UpdateStaff([FromBody] UpdateStaffRequest request)
    {
        _logger.LogInformation("PUT api/staff - Updating staff (Email={Email})", request.EmailAddress);

        AddressModel address = _mapper.Map<AddressModel>(request.Address);
        List<PhoneNumberModel> phoneNumbers = _mapper.Map<List<PhoneNumberModel>>(request.PhoneNumbers);

        await _staffService.UpdateStaffAsync(
            address,
            request.PIN,
            request.FirstName,
            request.LastName,
            request.EmailAddress,
            phoneNumbers);

        _logger.LogInformation("Staff updated successfully (Email={Email})", request.EmailAddress);
        return Ok(new { request.EmailAddress });
    }

    // Example request: GET /api/staff/basic/filter?departmentId=5&approvedType=Approved    
    // GET: api/staff/basic/filter?departmentId={departmentId}&approvedType={approvedType}
    [HttpGet("basic/filter")]
    public async Task<ActionResult<List<StaffBasicDto>>> GetAllBasicStaffFiltered(
        [FromQuery] int departmentId,
        [FromQuery] ApprovedType approvedType)
    {
        ApplyResponseCache("MediumDuration");
        _logger.LogInformation("GET: api/Staff/basic/filter (DepartmentId: {departmentId}, ApprovedType: {approvedType})",
            departmentId, approvedType);

        List<StaffBasicModel> staffList = await _staffService.GetAllBasicStaffFilteredAsync(departmentId, approvedType);

        if (staffList == null)
        {
            _logger.LogWarning("No staff found for DepartmentId: {departmentId}, ApprovedType: {approvedType}",
                departmentId, approvedType);
            return NotFound();
        }
        List<StaffBasicDto> staffDto = _mapper.Map<List<StaffBasicDto>>(staffList);
        return Ok(staffDto);
    }

    // GET: api/staff/basic
    [HttpGet("basic")]
    public async Task<ActionResult<List<StaffBasicDto>>> GetAllBasicStaff()
    {
        ApplyResponseCache("MediumDuration");
        _logger.LogInformation("GET: api/Staff/basic");

        List<StaffBasicModel> staffList = await _staffService.GetAllBasicStaffAsync();

        if (staffList == null)
        {
            _logger.LogWarning("No staff found.");
            return NotFound();
        }
        List<StaffBasicDto> staffDto = _mapper.Map<List<StaffBasicDto>>(staffList);
        return Ok(staffDto);
    }

    // Example request: GET /api/staff/email?emailAddress=someone%40example.com
    // remember to URL-encode @ in email address as %40
    // GET: api/staff/email?emailAddress={emailAddress}    
    [HttpGet("email")]
    public async Task<ActionResult<StaffFullDto>> GetStaffByEmail([FromQuery] string emailAddress)
    {
        ApplyResponseCache("ShortDuration");
        _logger.LogInformation("GET: api/Staff/email (Email: {Email})", emailAddress);

        var staffFull = await _staffService.GetStaffByEmailAsync(emailAddress);

        if (staffFull == null)
        {
            _logger.LogWarning("Staff not found for Email: {Email}", emailAddress);
            return NotFound();
        }

        var staffDto = _mapper.Map<StaffFullDto>(staffFull);
        return Ok(staffDto);
    }

    // Example request: GET /api/staff/123
    // GET: api/staff/{id}    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<StaffFullDto>> GetStaffById(int id)
    {
        ApplyResponseCache("ShortDuration");
        _logger.LogInformation("GET: api/Staff/{id} (Staff's Id: {Id})", id, id);

        StaffFullModel staffFull = await _staffService.GetStaffByIdAsync(id);
        if (staffFull == null)
        {
            _logger.LogWarning("Staff not found for Id: {Id}", id);
            return NotFound();
        }
        StaffFullDto staffDto = _mapper.Map<StaffFullDto>(staffFull);
        return Ok(staffDto);
    }

    // Example request: GET /api/staff/basic/123
    // GET: api/staff/basic/{id}    
    [HttpGet("basic/{id:int}")]
    public async Task<ActionResult<StaffBasicDto>> GetBasicStaffById(int id)
    {
        ApplyResponseCache("ShortDuration");
        _logger.LogInformation("GET: api/Staff/basic/{id} (Staff's Id: {Id})", id, id);

        StaffBasicModel staff = await _staffService.GetBasicStaffByIdAsync(id);
        if (staff == null)
        {
            _logger.LogWarning("Basic staff not found for Id: {Id}", id);
            return NotFound();
        }
        StaffBasicDto staffDto = _mapper.Map<StaffBasicDto>(staff);
        return Ok(staffDto);
    }

    // Example request: GET /api/staff/basic/alias/456
    // GET: api/staff/basic/alias/{aliasId}    
    [HttpGet("basic/alias/{aliasId:int}")]
    public async Task<ActionResult<StaffBasicDto>> GetBasicStaffByAliasId(int aliasId)
    {
        ApplyResponseCache("ShortDuration");
        _logger.LogInformation("GET: api/Staff/basic/alias/{aliasId} (AliasId: {AliasId})", aliasId, aliasId);

        StaffBasicModel staff = await _staffService.GetBasicStaffByAliasIdAsync(aliasId);
        if (staff == null)
        {
            _logger.LogWarning("Staff not found for AliasId: {AliasId}", aliasId);
            return NotFound();
        }
        StaffBasicDto staffDto = _mapper.Map<StaffBasicDto>(staff);
        return Ok(staffDto);
    }

    // Example request: GET /api/staff/email/123
    // GET: api/staff/email/{id}    
    [HttpGet("email/{id:int}")]
    public async Task<ActionResult<string>> GetStaffEmailById(int id)
    {
        ApplyResponseCache("ShortDuration");
        _logger.LogInformation("GET: api/Staff/email/{id} (Staff's Id: {Id})", id, id);

        string email = await _staffService.GetStaffEmailByIdAsync(id);
        if (string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("Email not found for staff Id: {Id}", id);
            return NotFound();
        }
        return Ok(new { EmailAddress = email });
    }

    // Example request: GET /api/staff/check-email?emailAddress=someone%40example.com
    // remember to URL-encode @ in email address as %40
    // GET: api/staff/check-email?emailAddress={emailAddress}    
    [HttpGet("check-email")]
    public async Task<ActionResult<bool>> CheckStaffByEmail([FromQuery] string emailAddress)
    {
        ApplyResponseCache("ShortDuration");
        _logger.LogInformation("GET: api/Staff/check-email (Email: {Email})", emailAddress);

        bool exists = await _staffService.CheckStaffByEmailAsync(emailAddress);
        return Ok(exists);
    }

    // PUT: api/staff/admin
    [HttpPut("admin")]
    public async Task<IActionResult> UpdateStaffByAdmin([FromBody] UpdateStaffByAdminRequest request)
    {
        _logger.LogInformation("PUT: api/Staff/admin (Staff's Id: {Id}, DepartmentId: {DepartmentId}, IsApproved: {IsApproved})",
            request.Id, request.DepartmentId, request.IsApproved);

        await _staffService.UpdateStaffByAdminAsync(request.Id, request.DepartmentId, request.IsApproved);
        return Ok(new { request.Id });
    }

    // Example request: DELETE /api/staff/123
    // DELETE: api/staff/{staffId}
    [HttpDelete("{staffId:int}")]
    public async Task<IActionResult> DeleteStaff(int staffId)
    {
        _logger.LogInformation("DELETE: api/Staff/{staffId} (StaffId: {StaffId})", staffId, staffId);

        await _staffService.DeleteStaffAsync(staffId);
        return Ok(new { StaffId = staffId });
    }
}
