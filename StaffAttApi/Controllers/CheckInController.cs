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

    public CheckInController(ICheckInService checkInService, ILogger<CheckInController> logger, IMapper mapper)
    {
        _checkInService = checkInService;
        _logger = logger;
        _mapper = mapper;
    }

    // Example request: GET /api/checkin/last/1
    // GET: api/checkin/last/{staffId}
    [HttpGet("last/{staffId:int}")]
    public async Task<ActionResult<CheckInDto>> GetLastCheckIn(int staffId)
    {
        _logger.LogInformation("GET: api/CheckIn/last/{staffId} (StaffId: {StaffId})", staffId, staffId);

        try
        {
            var checkIn = await _checkInService.GetLastCheckInAsync(staffId);
            if (checkIn == null)
            {
                _logger.LogWarning("No check-in found for StaffId: {StaffId}", staffId);
                return NotFound();
            }
            var checkInDto = _mapper.Map<CheckInDto>(checkIn);
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
        _logger.LogInformation("GET: api/CheckIn/all (StartDate: {StartDate}, EndDate: {EndDate})", startDate, endDate);

        try
        {
            var checkIns = await _checkInService.GetAllCheckInsByDateAsync(startDate, endDate);
            if (checkIns == null)
            {
                _logger.LogWarning("No check-ins found between {StartDate} and {EndDate}", startDate, endDate);
                return NotFound();
            }
            var checkInDtos = _mapper.Map<List<CheckInFullDto>>(checkIns);
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
        _logger.LogInformation("GET: api/CheckIn/byEmail (Email: {Email}, StartDate: {StartDate}, EndDate: {EndDate})",
                               emailAddress, startDate, endDate);

        try
        {
            var checkIns = await _checkInService.GetCheckInsByDateAndEmailAsync(emailAddress, startDate, endDate);
            if (checkIns == null)
            {
                _logger.LogWarning("No check-ins found for Email: {Email} between {StartDate} and {EndDate}", emailAddress, startDate, endDate);
                return NotFound();
            }
            var checkInDtos = _mapper.Map<List<CheckInFullDto>>(checkIns);
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
        _logger.LogInformation("GET: api/CheckIn/byId/{id} (Id: {Id}, StartDate: {StartDate}, EndDate: {EndDate})", 
                               id, id, startDate, endDate);

        try
        {
            var checkIns = await _checkInService.GetCheckInsByDateAndIdAsync(id, startDate, endDate);
            if (checkIns == null)
            {
                _logger.LogWarning("No check-ins found for Id: {Id} between {StartDate} and {EndDate}", id, startDate, endDate);
                return NotFound();
            }
            var checkInDtos = _mapper.Map<List<CheckInFullDto>>(checkIns);
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
