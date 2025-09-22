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
    private readonly ILogger<CheckInController> _logger;

    public CheckInController(ICheckInService checkInService, ILogger<CheckInController> logger)
    {
        _checkInService = checkInService;
        _logger = logger;
    }

    // GET: api/checkin/last/{staffId}
    [HttpGet("last/{staffId}")]
    public async Task<ActionResult<CheckInModel>> GetLastCheckIn(int staffId)
    {
        _logger.LogInformation("GET: api/CheckIn/last (StaffId: {staffId})", staffId);

        var checkIn = await _checkInService.GetLastCheckInAsync(staffId);
        if (checkIn == null)
            return NotFound();
        return Ok(checkIn);
    }

    // POST: api/checkin/do/{staffId}
    [HttpPost("do/{staffId}")]
    public async Task<IActionResult> DoCheckInOrCheckOut(int staffId)
    {
        _logger.LogInformation("POST: api/CheckIn/do (StaffId: {staffId})", staffId);

        await _checkInService.DoCheckInOrCheckOutAsync(staffId);
        return NoContent();
    }
}
