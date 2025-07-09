using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Data;
public class CheckInSqliteService : ICheckInService
{
    public Task DoCheckInOrCheckOutAsync(int staffId)
    {
        throw new NotImplementedException();
    }

    public Task<List<CheckInFullModel>> GetAllCheckInsByDateAsync(DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }

    public Task<List<CheckInFullModel>> GetCheckInsByDateAndEmailAsync(string emailAddress, DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }

    public Task<List<CheckInFullModel>> GetCheckInsByDateAndIdAsync(int id, DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }

    public Task<CheckInModel> GetLastCheckInAsync(int staffId)
    {
        throw new NotImplementedException();
    }
}
