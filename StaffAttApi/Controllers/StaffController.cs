using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StaffAttLibrary.Data;
using StaffAttLibrary.Models;

namespace StaffAttApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Require JWT for all endpoints
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;

    public StaffController(IStaffService staffService)
    {
        _staffService = staffService;
    }

    // GET: api/staff/basic/{aliasId}
    [HttpGet("basic/{aliasId}")]
    public async Task<ActionResult<StaffBasicModel>> GetBasicStaffByAliasId(int aliasId)
    {
        var staff = await _staffService.GetBasicStaffByAliasIdAsync(aliasId);
        if (staff == null)
            return NotFound();
        return Ok(staff);
    }
}
