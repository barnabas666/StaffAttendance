using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Data
{
    public interface ICheckInData
    {        
        Task<bool> CheckApproveStatus(int aliasId);
        Task<CheckInModel> GetLastCheckIn(int staffId);
        Task<int> CheckInPerform(int staffId);
        Task<int> CheckOutPerform(int checkInId);
        Task<List<CheckInFullModel>> GetAllCheckIns();
        Task<List<CheckInFullModel>> GetAllCheckInsByDate(DateTime startDate, DateTime endDate);
        Task<List<CheckInFullModel>> GetCheckInsByEmail(string emailAddress);
        Task<List<CheckInFullModel>> GetCheckInsByDateAndEmail(string emailAddress, DateTime startDate, DateTime endDate);
        Task<List<CheckInFullModel>> GetCheckInsByDateAndId(int id, DateTime startDate, DateTime endDate);
    }
}
