namespace StaffAttShared.DTOs;

public class CheckInDto
{
    /// <summary>
    /// CheckIn's Id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Staff's Id.
    /// </summary>
    public int StaffId { get; set; }

    /// <summary>
    /// CheckIn's Date.
    /// </summary>
    public DateTime CheckInDate { get; set; }

    /// <summary>
    /// CheckOut's Date, possible null value.
    /// </summary>
    public DateTime? CheckOutDate { get; set; }
}
