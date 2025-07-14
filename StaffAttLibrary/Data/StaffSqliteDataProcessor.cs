using StaffAttLibrary.Db;
using StaffAttLibrary.Helpers;
using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Data;
public class StaffSqliteDataProcessor : IStaffDataProcessor
{
    private readonly ISqliteDataAccess _db;
    /// <summary>
    /// Holds default connection string name.
    /// </summary>
    private readonly string _connectionStringName;

    public StaffSqliteDataProcessor(ISqliteDataAccess db, IConnectionStringData connectionStringData)
    {
        _db = db;
        _connectionStringName = connectionStringData.SqlConnectionName;
    }

    /// <summary>
    /// Create new PhoneNumber in DB.
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public async Task<int> SavePhoneNumberAsync(PhoneNumberModel phoneNumber)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("PhoneNumbers_Insert.sql");
        int phoneNumberId = await _db.SaveDataGetIdAsync(sql,
                                                         new { phoneNumber = phoneNumber.PhoneNumber },
                                                         _connectionStringName);
        return phoneNumberId;
    }

    /// <summary>
    /// Create new PhoneNumber link in DB.
    /// </summary>
    /// <param name="staffId"></param>
    /// <param name="phoneNumberId"></param>
    /// <returns></returns>
    public async Task SavePhoneNumberLinkAsync(int staffId, int phoneNumberId)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("StaffPhoneNumbers_Insert.sql");
        await _db.SaveDataAsync(sql,
                                new { staffId, phoneNumberId },
                                _connectionStringName);
    }

    /// <summary>
    /// Check if PhoneNumber already exists in DB.
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public async Task<bool> CheckPhoneNumberAsync(PhoneNumberModel phoneNumber)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("PhoneNumbers_CheckPhoneNumber.sql");
        List<bool> output = await _db.LoadDataAsync<bool, dynamic>(
            sql,
            new { phoneNumber = phoneNumber.PhoneNumber },
            _connectionStringName);

        return output.FirstOrDefault();
    }

    /// <summary>
    /// Get PhoneNumber from DB.
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public async Task<List<PhoneNumberModel>> GetPhoneNumberAsync(PhoneNumberModel phoneNumber)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("PhoneNumbers_GetPhoneNumber.sql");
        return await _db.LoadDataAsync<PhoneNumberModel, dynamic>(
            sql,
            new { phoneNumber = phoneNumber.PhoneNumber },
            _connectionStringName);
    }

    /// <summary>
    /// Delete PhoneNumber from DB.
    /// </summary>
    /// <param name="phoneNumberId"></param>
    /// <returns></returns>
    public async Task DeletePhoneNumberAsync(int phoneNumberId)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("PhoneNumbers_Delete.sql");
        await _db.SaveDataAsync(sql,
                                new { phoneNumberId },
                                _connectionStringName);
    }

    /// <summary>
    /// Delete PhoneNumber link from DB.
    /// </summary>
    /// <param name="staffId"></param>
    /// <param name="phoneNumberId"></param>
    /// <returns></returns>
    public async Task DeletePhoneNumberLinkAsync(int staffId, int phoneNumberId)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("StaffPhoneNumbers_Delete.sql");
        await _db.SaveDataAsync(sql,
                                new { staffId, phoneNumberId },
                                _connectionStringName);
    }

    /// <summary>
    /// Get PhoneNumber links from DB.
    /// </summary>
    /// <param name="phoneNumberId"></param>
    /// <returns></returns>
    public async Task<List<StaffPhoneNumberModel>> GetPhoneNumberLinksAsync(int phoneNumberId)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("StaffPhoneNumbers_GetByPhoneNumber.sql");
        return await _db.LoadDataAsync<StaffPhoneNumberModel, dynamic>(sql,
                                                                       new { phoneNumberId },
                                                                       _connectionStringName);
    }

    /// <summary>
    /// Check if Alias exists in Db. Returns true if it does.
    /// </summary>
    /// <param name="alias"></param>
    /// <returns>True if Alias already exists.</returns>
    public async Task<bool> CheckAliasAsync(string alias)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("Aliases_Check.sql");
        List<bool> output = await _db.LoadDataAsync<bool, dynamic>(sql,
                                                                   new { alias },
                                                                   _connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Insert Alias into Db and return Id.
    /// </summary>
    /// <param name="alias"></param>
    /// <param name="pIN"></param>
    /// <returns></returns>
    public async Task<int> SaveAliasAsync(string alias, string pIN)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("Aliases_Insert.sql");
        return await _db.SaveDataGetIdAsync(sql,
                                            new { alias, pIN },
                                            _connectionStringName);
    }

    /// <summary>
    /// Create Alias from First Name, Last Name and Order Number (just in case of same aliases)
    /// John Doe will have alias JDO1, than Jason Doherty JDO2, Susan Storm SST1 etc. 
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="orderNumber"></param>
    /// <returns></returns>
    public string CreateAlias(string firstName, string lastName, int orderNumber)
    {
        string output = "";

        output += firstName.Substring(0, 1).ToUpper();
        output += lastName.Substring(0, 2).ToUpper();
        output += orderNumber.ToString();

        return output;
    }
}
