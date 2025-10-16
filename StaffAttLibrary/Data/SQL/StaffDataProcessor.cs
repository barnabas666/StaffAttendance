using StaffAttLibrary.Db.SQL;
using StaffAttLibrary.Models;

namespace StaffAttLibrary.Data.SQL;

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

    public StaffDataProcessor(ISqlDataAccess db, IConnectionStringData connectionStringData)
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
        int phoneNumberId = await _db.SaveDataGetIdAsync("spPhoneNumbers_Insert",
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
        await _db.SaveDataAsync("spStaffPhoneNumbers_Insert",
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
        List<bool> output = await _db.LoadDataAsync<bool, dynamic>(
            "spPhoneNumbers_Check",
            new { phoneNumber = phoneNumber.PhoneNumber },
            _connectionStringName);

        return output.FirstOrDefault();
    }

    /// <summary>
    /// Get PhoneNumber Id from DB by Phone Number.
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public async Task<int> GetPhoneNumberIdAsync(PhoneNumberModel phoneNumber)
    {
        List<int> output = await _db.LoadDataAsync<int, dynamic>(
            "spPhoneNumbers_GetIdByPhoneNumber",
            new { phoneNumber = phoneNumber.PhoneNumber },
            _connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Delete PhoneNumber from DB.
    /// </summary>
    /// <param name="phoneNumberId"></param>
    /// <returns></returns>
    public async Task DeletePhoneNumberAsync(int phoneNumberId)
    {
        await _db.SaveDataAsync("spPhoneNumbers_Delete",
                           new { phoneNumberId },
                           _connectionStringName);
    }

    /// <summary>
    /// Check if PhoneNumber link already exists in DB.
    /// </summary>
    /// <param name="staffId"></param>
    /// <param name="phoneNumberId"></param>
    /// <returns></returns>
    public async Task<bool> CheckPhoneNumberLinkAsync(int staffId, int phoneNumberId)
    {
        List<bool> output = await _db.LoadDataAsync<bool, dynamic>(
            "StaffPhoneNumbers_Check",
            new { staffId, phoneNumberId },
            _connectionStringName);

        return output.FirstOrDefault();
    }

    /// <summary>
    /// Delete PhoneNumber link from DB.
    /// </summary>
    /// <param name="staffId"></param>
    /// <param name="phoneNumberId"></param>
    /// <returns></returns>
    public async Task DeletePhoneNumberLinkAsync(int staffId, int phoneNumberId)
    {
        await _db.SaveDataAsync("spStaffPhoneNumbers_Delete",
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
        return await _db.LoadDataAsync<StaffPhoneNumberModel, dynamic>(
            "spStaffPhoneNumbers_GetByPhoneNumber",
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
        List<bool> output = await _db.LoadDataAsync<bool, dynamic>("spAliases_Check",
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
        return await _db.SaveDataGetIdAsync("spAliases_Insert",
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
