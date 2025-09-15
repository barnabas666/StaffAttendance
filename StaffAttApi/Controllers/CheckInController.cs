using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StaffAttLibrary.Data;
using StaffAttLibrary.Models;

namespace StaffAttApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Require JWT for all endpoints
public class CheckInController : ControllerBase
{
    private readonly ICheckInService _checkInService;

    public CheckInController(ICheckInService checkInService)
    {
        _checkInService = checkInService;
    }

    // GET: api/checkin/last/{staffId}
    [HttpGet("last/{staffId}")]
    public async Task<ActionResult<CheckInModel>> GetLastCheckIn(int staffId)
    {
        var checkIn = await _checkInService.GetLastCheckInAsync(staffId);
        if (checkIn == null)
            return NotFound();
        return Ok(checkIn);
    }

    // POST: api/checkin/do/{staffId}
    [HttpPost("do/{staffId}")]
    public async Task<IActionResult> DoCheckInOrCheckOut(int staffId)
    {
        await _checkInService.DoCheckInOrCheckOutAsync(staffId);
        return NoContent();
    }
}
