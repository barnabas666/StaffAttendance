using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StaffAttLibrary.Data;
using StaffAttLibrary.Enums;
using StaffAttLibrary.Models;
using StaffAttShared.DTOs;

namespace StaffAttApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Require JWT for all endpoints
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;
    private readonly ILogger<StaffController> _logger;
    private readonly IMapper _mapper;

    public StaffController(IStaffService staffService, ILogger<StaffController> logger, IMapper mapper)
    {
        _staffService = staffService;
        _logger = logger;
        _mapper = mapper;
    }

    // GET: api/staff/departments
    [HttpGet("departments")]
    public async Task<ActionResult<List<DepartmentModel>>> GetAllDepartments()
    {
        _logger.LogInformation("GET: api/Staff/departments");

        try
        {
            var departments = await _staffService.GetAllDepartmentsAsync();
            return Ok(departments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The GET call to api/Staff/departments failed.");
            return BadRequest();
        }
    }

    // POST: api/staff
    [HttpPost]
    public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequest request)
    {
        _logger.LogInformation("POST: api/Staff (Email: {Email})", request.EmailAddress);

        try
        {
            var address = _mapper.Map<AddressModel>(request.Address);
            var phoneNumbers = _mapper.Map<List<PhoneNumberModel>>(request.PhoneNumbers);

            await _staffService.CreateStaffAsync(
                request.DepartmentId,
                address,
                request.PIN,
                request.FirstName,
                request.LastName,
                request.EmailAddress,
                phoneNumbers);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The POST call to api/Staff failed. Email: {Email}", request.EmailAddress);
            return BadRequest();
        }
    }

    // PUT: api/staff
    [HttpPut]
    public async Task<IActionResult> UpdateStaff([FromBody] UpdateStaffRequest request)
    {
        _logger.LogInformation("PUT: api/Staff (Email: {Email})", request.EmailAddress);

        try
        {
            var address = _mapper.Map<AddressModel>(request.Address);
            var phoneNumbers = _mapper.Map<List<PhoneNumberModel>>(request.PhoneNumbers);

            await _staffService.UpdateStaffAsync(
                address,
                request.PIN,
                request.FirstName,
                request.LastName,
                request.EmailAddress,
                phoneNumbers);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The PUT call to api/Staff failed. Email: {Email}", request.EmailAddress);
            return BadRequest();
        }
    }

    // Example request: GET /api/staff/basic/filter?departmentId=5&approvedType=Approved    
    // GET: api/staff/basic/filter?departmentId={departmentId}&approvedType={approvedType}
    [HttpGet("basic/filter")]
    public async Task<ActionResult<List<StaffBasicDto>>> GetAllBasicStaffFiltered(
        [FromQuery] int departmentId,
        [FromQuery] ApprovedType approvedType)
    {
        _logger.LogInformation("GET: api/Staff/basic/filter (DepartmentId: {departmentId}, ApprovedType: {approvedType})",
            departmentId, approvedType);

        try
        {
            var staffList = await _staffService.GetAllBasicStaffFilteredAsync(departmentId, approvedType);

            if (staffList == null)
            {
                _logger.LogWarning("No staff found for DepartmentId: {departmentId}, ApprovedType: {approvedType}",
                    departmentId, approvedType);
                return NotFound();
            }
            var staffDto = _mapper.Map<List<StaffBasicDto>>(staffList);
            return Ok(staffDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The GET call to api/Staff/basic/filter failed. DepartmentId: {departmentId}, ApprovedType: {approvedType}",
                departmentId, approvedType);
            return BadRequest();
        }
    }

    // GET: api/staff/basic
    [HttpGet("basic")]
    public async Task<ActionResult<List<StaffBasicDto>>> GetAllBasicStaff()
    {
        _logger.LogInformation("GET: api/Staff/basic");

        try
        {
            var staffList = await _staffService.GetAllBasicStaffAsync();

            if (staffList == null)
            {
                _logger.LogWarning("No staff found.");
                return NotFound();
            }
            var staffDto = _mapper.Map<List<StaffBasicDto>>(staffList);
            return Ok(staffDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The GET call to api/Staff/basic failed.");
            return BadRequest();
        }
    }

    // Example request: GET /api/staff/email?emailAddress=someone%40example.com
    // remember to URL-encode @ in email address as %40
    // GET: api/staff/email?emailAddress={emailAddress}    
    [HttpGet("email")]
    public async Task<ActionResult<StaffFullDto>> GetStaffByEmail([FromQuery] string emailAddress)
    {
        _logger.LogInformation("GET: api/Staff/email (Email: {Email})", emailAddress);

        try
        {
            var staffFull = await _staffService.GetStaffByEmailAsync(emailAddress);
            if (staffFull == null)
            {
                _logger.LogWarning("Staff not found for Email: {Email}", emailAddress);
                return NotFound();
            }
            var staffDto = _mapper.Map<StaffFullDto>(staffFull);
            return Ok(staffDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The GET call to api/Staff/email failed. Email: {Email}", emailAddress);
            return BadRequest();
        }
    }

    // Example request: GET /api/staff/123
    // GET: api/staff/{id}    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<StaffFullDto>> GetStaffById(int id)
    {
        _logger.LogInformation("GET: api/Staff/{id} (Staff's Id: {Id})", id, id);

        try
        {
            var staffFull = await _staffService.GetStaffByIdAsync(id);
            if (staffFull == null)
            {
                _logger.LogWarning("Staff not found for Id: {Id}", id);
                return NotFound();
            }
            var staffDto = _mapper.Map<StaffFullDto>(staffFull);
            return Ok(staffDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The GET call to api/Staff/{id} failed. Staff's Id: {Id}", id, id);
            return BadRequest();
        }
    }

    // Example request: GET /api/staff/basic/123
    // GET: api/staff/basic/{id}    
    [HttpGet("basic/{id:int}")]
    public async Task<ActionResult<StaffBasicModel>> GetBasicStaffById(int id)
    {
        _logger.LogInformation("GET: api/Staff/basic/{id} (Staff's Id: {Id})", id, id);

        try
        {
            var staff = await _staffService.GetBasicStaffByIdAsync(id);
            if (staff == null)
            {
                _logger.LogWarning("Basic staff not found for Id: {Id}", id);
                return NotFound();
            }
            return Ok(staff);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The GET call to api/Staff/basic/{id} failed. Staff's Id: {Id}", id, id);
            return BadRequest();
        }
    }

    // Example request: GET /api/staff/basic/alias/456
    // GET: api/staff/basic/alias/{aliasId}    
    [HttpGet("basic/alias/{aliasId:int}")]
    public async Task<ActionResult<StaffBasicModel>> GetBasicStaffByAliasId(int aliasId)
    {
        _logger.LogInformation("GET: api/Staff/basic/alias/{aliasId} (AliasId: {AliasId})", aliasId, aliasId);

        try
        {
            var staff = await _staffService.GetBasicStaffByAliasIdAsync(aliasId);
            if (staff == null)
            {
                _logger.LogWarning("Staff not found for AliasId: {AliasId}", aliasId);
                return NotFound();
            }
            return Ok(staff);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The GET call to api/Staff/basic/alias/{AliasId} failed.", aliasId);
            return BadRequest();
        }
    }

    // Example request: GET /api/staff/email/123
    // GET: api/staff/email/{id}    
    [HttpGet("email/{id:int}")]
    public async Task<ActionResult<string>> GetStaffEmailById(int id)
    {
        _logger.LogInformation("GET: api/Staff/email/{id} (Staff's Id: {Id})", id, id);

        try
        {
            var email = await _staffService.GetStaffEmailByIdAsync(id);
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Email not found for staff Id: {Id}", id);
                return NotFound();
            }
            return Ok(email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The GET call to api/Staff/email/{id} failed. Staff's Id: {Id}", id, id);
            return BadRequest();
        }
    }

    // Example request: GET /api/staff/check-email?emailAddress=someone%40example.com
    // remember to URL-encode @ in email address as %40
    // GET: api/staff/check-email?emailAddress={emailAddress}    
    [HttpGet("check-email")]
    public async Task<ActionResult<bool>> CheckStaffByEmail([FromQuery] string emailAddress)
    {
        _logger.LogInformation("GET: api/Staff/check-email (Email: {Email})", emailAddress);

        try
        {
            var exists = await _staffService.CheckStaffByEmailAsync(emailAddress);
            return Ok(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The GET call to api/Staff/check-email failed. Email: {Email}", emailAddress);
            return BadRequest();
        }
    }

    // PUT: api/staff/admin
    [HttpPut("admin")]
    public async Task<IActionResult> UpdateStaffByAdmin([FromBody] UpdateStaffByAdminRequest request)
    {
        _logger.LogInformation("PUT: api/Staff/admin (Staff's Id: {Id}, DepartmentId: {DepartmentId}, IsApproved: {IsApproved})",
            request.Id, request.DepartmentId, request.IsApproved);

        try
        {
            await _staffService.UpdateStaffByAdminAsync(request.Id, request.DepartmentId, request.IsApproved);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The PUT call to api/Staff/admin failed. Staff's Id: {Id}", request.Id);
            return BadRequest();
        }
    }

    // Example request: DELETE /api/staff/123
    // DELETE: api/staff/{staffId}
    [HttpDelete("{staffId:int}")]
    public async Task<IActionResult> DeleteStaff(int staffId)
    {
        _logger.LogInformation("DELETE: api/Staff/{staffId} (StaffId: {StaffId})", staffId, staffId);

        try
        {
            await _staffService.DeleteStaffAsync(staffId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The DELETE call to api/Staff/{staffId} failed. StaffId: {StaffId}", staffId, staffId);
            return BadRequest();
        }
    }
}
