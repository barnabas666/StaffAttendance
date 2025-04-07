using StaffAttLibrary.Db;
using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Data;

/// <summary>
/// Class servicing Staffs - CRUD actions.
/// StaffData talks to this class. This class calls SqlDataAccess methods.
/// </summary>
public class StaffDataProcessor : IStaffDataProcessor
{
    private readonly ISqlDataAccess _db;
    /// <summary>
    /// Holds default connection string name.
    /// </summary>
    private readonly string _connectionStringName;

    public StaffDataProcessor(ISqlDataAccess db, IConnectionStringData connectionString)
    {
        _db = db;
        _connectionStringName = connectionString.SqlConnectionName;
    }

    public async Task<int> CreatePhoneNumberAsync(int phoneNumberId, PhoneNumberModel phoneNumber)
    {
        phoneNumberId = await _db.SaveDataGetIdAsync("spPhoneNumbers_Insert",
                                                new { phoneNumber = phoneNumber.PhoneNumber },
                                                _connectionStringName);
        return phoneNumberId;
    }

    public async Task CreatePhoneNumberLinkAsync(int staffId, int phoneNumberId)
    {
        await _db.SaveDataAsync("spStaffPhoneNumbers_Insert",
                           new { staffId, phoneNumberId },
                           _connectionStringName);
    }

    public async Task<List<PhoneNumberModel>> GetByPhoneNumberAsync(PhoneNumberModel phoneNumber)
    {
        return await _db.LoadDataAsync<PhoneNumberModel, dynamic>(
            "spPhoneNumbers_GetByPhoneNumber",
            new { phoneNumber = phoneNumber.PhoneNumber },
            _connectionStringName);
    }

    public async Task DeletePhoneNumberAsync(int phoneNumberId)
    {
        await _db.SaveDataAsync("spPhoneNumbers_Delete",
                           new { phoneNumberId },
                           _connectionStringName);
    }

    public async Task DeletePhoneNumberLinkAsync(int staffId, int phoneNumberId)
    {
        await _db.SaveDataAsync("spStaffPhoneNumbers_Delete",
                           new { staffId, phoneNumberId },
                           _connectionStringName);
    }

    public async Task<List<StaffPhoneNumberModel>> GetPhoneNumberLinksAsync(int phoneNumberId)
    {
        return await _db.LoadDataAsync<StaffPhoneNumberModel, dynamic>(
            "spStaffPhoneNumbers_GetByPhoneNumber",
            new { phoneNumberId },
            _connectionStringName);
    }
}
