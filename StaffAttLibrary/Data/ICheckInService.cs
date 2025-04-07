using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Data
{
    public interface ICheckInService
    {
        Task<bool> CheckApproveStatusAsync(int aliasId);
        Task DoCheckInOrCheckOutAsync(int staffId);
        Task<CheckInModel> GetLastCheckInAsync(int staffId);
        Task<List<CheckInFullModel>> GetAllCheckInsAsync();
        Task<List<CheckInFullModel>> GetAllCheckInsByDateAsync(DateTime startDate, DateTime endDate);
        Task<List<CheckInFullModel>> GetCheckInsByEmailAsync(string emailAddress);
        Task<List<CheckInFullModel>> GetCheckInsByDateAndEmailAsync(string emailAddress, DateTime startDate, DateTime endDate);
        Task<List<CheckInFullModel>> GetCheckInsByDateAndIdAsync(int id, DateTime startDate, DateTime endDate);
    }
}
