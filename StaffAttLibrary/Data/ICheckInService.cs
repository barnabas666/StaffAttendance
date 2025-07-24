using StaffAttLibrary.Models;

namespace StaffAttLibrary.Data
{
    public interface ICheckInService
    {
        Task DoCheckInOrCheckOutAsync(int staffId);
        Task<CheckInModel> GetLastCheckInAsync(int staffId);
        Task<List<CheckInFullModel>> GetAllCheckInsByDateAsync(DateTime startDate, DateTime endDate);
        Task<List<CheckInFullModel>> GetCheckInsByDateAndEmailAsync(string emailAddress, DateTime startDate, DateTime endDate);
        Task<List<CheckInFullModel>> GetCheckInsByDateAndIdAsync(int id, DateTime startDate, DateTime endDate);
    }
}
