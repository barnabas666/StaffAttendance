using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Data;
public class CheckInSqliteData : ICheckInData
{
    public Task<int> CheckInPerformAsync(int staffId)
    {
        throw new NotImplementedException();
    }

    public Task<int> CheckOutPerformAsync(int checkInId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCheckInAsync(int staffId)
    {
        throw new NotImplementedException();
    }
}
