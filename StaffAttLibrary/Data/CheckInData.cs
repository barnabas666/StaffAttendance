using StaffAttLibrary.Db;
using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Data;

/// <summary>
/// Class servicing Check-Ins - CRUD actions.
/// CheckInService talks to this class. This class calls SqlDataAccess methods.
/// </summary>
public class CheckInData : ICheckInData
{
    private readonly ISqlDataAccess _db;
    /// <summary>
    /// Holds default connection string name.
    /// </summary>
    private readonly string _connectionStringName;

    public CheckInData(ISqlDataAccess db, ConnectionStringData connectionString)
    {
        _db = db;
        _connectionStringName = connectionString.SqlConnectionName;
    }

    /// <summary>
    /// Perform CheckIn for given Staff with current date.
    /// </summary>
    /// <param name="staffId">Staff's id.</param>
    /// <returns>CheckIn id.</returns>
    public async Task<int> CheckInPerformAsync(int staffId)
    {
        return await _db.SaveDataGetIdAsync("spCheckIns_InsertCheckIn",
                           new { staffId },
                           _connectionStringName);
    }

    /// <summary>
    /// Perform CheckOut for given CheckIn record with current date.
    /// </summary>
    /// <param name="checkInId">CheckIn's id.</param>
    /// <returns>CheckIn id.</returns>
    public async Task<int> CheckOutPerformAsync(int checkInId)
    {
        return await _db.SaveDataGetIdAsync("spCheckIns_InsertCheckOut",
                           new { checkInId },
                           _connectionStringName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="aliasId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">For invalid aliasId.</exception>
    public async Task<StaffBasicModel> GetBasicStaffByAliasIdAsync(int aliasId)
    {
        List<StaffBasicModel> output = await _db.LoadDataAsync<StaffBasicModel, dynamic>("spStaffs_GetBasicByAliasId",
                                                                                 new { aliasId },
                                                                                 _connectionStringName);
        if (output.FirstOrDefault() == null)
            throw new ArgumentException("You passed in an invalid parameter", "aliasId");

        return output.FirstOrDefault();
    }
}
