using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StaffAttLibrary.Data;
using StaffAttLibrary.Models;
using StaffAttShared.DTOs;

namespace StaffAttApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Require JWT for all endpoints
public class CheckInController : ControllerBase
{
    private readonly ICheckInService _checkInService;
    private readonly ILogger<CheckInController> _logger;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;

    public CheckInController(ICheckInService checkInService, ILogger<CheckInController> logger, IMapper mapper, IConfiguration config)
    {
        _checkInService = checkInService;
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

    // Example request: GET /api/checkin/last/1
    // GET: api/checkin/last/{staffId}
    [HttpGet("last/{staffId:int}")]
    public async Task<ActionResult<CheckInDto>> GetLastCheckIn(int staffId)
    {
        ApplyResponseCache("ShortDuration");
        _logger.LogInformation("GET: api/CheckIn/last/{staffId} (StaffId: {StaffId})", staffId, staffId);

        try
        {
            CheckInModel checkInModel = await _checkInService.GetLastCheckInAsync(staffId);
            if (checkInModel == null)
            {
                _logger.LogInformation("No previous check-in found for StaffId: {StaffId}", staffId);
                // returns JSON null body
                return Ok((CheckInDto?)null);
            }

            CheckInDto checkInDto = _mapper.Map<CheckInDto>(checkInModel);
            return Ok(checkInDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The GET call to api/CheckIn/last/{staffId} failed. StaffId: {StaffId}", staffId, staffId);
            return BadRequest();
        }
    }

    // Example request: POST /api/checkin/do/1
    // POST: api/checkin/do/{staffId}    
    [HttpPost("do/{staffId:int}")]
    public async Task<IActionResult> DoCheckInOrCheckOut(int staffId)
    {
        _logger.LogInformation("POST: api/CheckIn/do/{staffId} (StaffId: {StaffId})", staffId, staffId);

        try
        {
            await _checkInService.DoCheckInOrCheckOutAsync(staffId);
            return Ok(new { StaffId = staffId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The POST call to api/CheckIn/do/{staffId} failed. StaffId: {StaffId}", staffId, staffId);
            return BadRequest();
        }
    }

    // Example request: GET /api/checkin/all?startDate=2025-09-01&endDate=2025-09-22
    // GET: api/checkin/all
    [HttpGet("all")]
    public async Task<ActionResult<List<CheckInFullDto>>> GetAllCheckInsByDate([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        ApplyResponseCache("MediumDuration");
        _logger.LogInformation("GET: api/CheckIn/all (StartDate: {StartDate}, EndDate: {EndDate})", startDate, endDate);

        try
        {
            List<CheckInFullModel> checkIns = await _checkInService.GetAllCheckInsByDateAsync(startDate, endDate);
            if (checkIns == null)
            {
                _logger.LogWarning("No check-ins found between {StartDate} and {EndDate}", startDate, endDate);
                return NotFound();
            }
            List<CheckInFullDto> checkInDtos = _mapper.Map<List<CheckInFullDto>>(checkIns);
            return Ok(checkInDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The GET call to api/CheckIn/all failed. StartDate: {StartDate}, EndDate: {EndDate}", startDate, endDate);
            return BadRequest();
        }
    }

    // Example request: GET /api/checkin/byEmail?emailAddress=someone%40example.com&startDate=2025-09-01&endDate=2025-09-22
    // remember to URL-encode @ in email address as %40
    // GET: api/checkin/byEmail
    [HttpGet("byEmail")]
    public async Task<ActionResult<List<CheckInFullDto>>> GetCheckInsByDateAndEmail([FromQuery] string emailAddress, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        ApplyResponseCache("MediumDuration");
        _logger.LogInformation("GET: api/CheckIn/byEmail (Email: {Email}, StartDate: {StartDate}, EndDate: {EndDate})",
                               emailAddress, startDate, endDate);

        try
        {
            List<CheckInFullModel> checkIns = await _checkInService.GetCheckInsByDateAndEmailAsync(emailAddress, startDate, endDate);
            if (checkIns == null)
            {
                _logger.LogWarning("No check-ins found for Email: {Email} between {StartDate} and {EndDate}", emailAddress, startDate, endDate);
                return NotFound();
            }
            List<CheckInFullDto> checkInDtos = _mapper.Map<List<CheckInFullDto>>(checkIns);
            return Ok(checkInDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                             "The GET call to api/CheckIn/byEmail failed. Email: {Email}, StartDate: {StartDate}, EndDate: {EndDate}",
                             emailAddress, startDate, endDate);
            return BadRequest();
        }
    }

    // Example request: GET /api/checkin/byId/1?startDate=2025-09-01&endDate=2025-09-22
    // GET: api/checkin/byId/{id}  
    [HttpGet("byId/{id:int}")]
    public async Task<ActionResult<List<CheckInFullDto>>> GetCheckInsByDateAndId(int id, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        ApplyResponseCache("MediumDuration");
        _logger.LogInformation("GET: api/CheckIn/byId/{id} (Id: {Id}, StartDate: {StartDate}, EndDate: {EndDate})",
                               id, id, startDate, endDate);

        try
        {
            List<CheckInFullModel> checkIns = await _checkInService.GetCheckInsByDateAndIdAsync(id, startDate, endDate);
            if (checkIns == null)
            {
                _logger.LogWarning("No check-ins found for Id: {Id} between {StartDate} and {EndDate}", id, startDate, endDate);
                return NotFound();
            }
            List<CheckInFullDto> checkInDtos = _mapper.Map<List<CheckInFullDto>>(checkIns);
            return Ok(checkInDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The GET call to api/CheckIn/byId/{id} failed. Id: {Id}, StartDate: {StartDate}, EndDate: {EndDate}",
                             id, id, startDate, endDate);
            return BadRequest();
        }
    }
}
