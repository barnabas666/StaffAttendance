using StaffAttLibrary.Db;
using StaffAttLibrary.Enums;
using StaffAttLibrary.Helpers;
using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Data;
public class StaffSqliteData : IStaffData
{
    private readonly ISqliteDataAccess _db;
    private readonly IStaffDataProcessor _staffDataProcessor;

    /// <summary>
    /// Holds default connection string name.
    /// </summary>
    private readonly string _connectionStringName;

    public StaffSqliteData(ISqliteDataAccess db, IStaffDataProcessor staffDataProcessor, IConnectionStringData connectionStringData)
    {
        _db = db;
        _staffDataProcessor = staffDataProcessor;
        _connectionStringName = connectionStringData.SQLiteConnectionName;
    }

    /// <summary>
    /// Save Staff into Db and returns back Id.
    /// </summary>
    /// <param name="departmentId"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="emailAddress"></param>
    /// <param name="addressId"></param>
    /// <param name="aliasId"></param>
    /// <returns>Staff Id.</returns>
    public async Task<int> SaveStaffAsync(int departmentId,
                                      string firstName,
                                      string lastName,
                                      string emailAddress,
                                      int addressId,
                                      int aliasId)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("Staffs_Insert.sql");
        return await _db.SaveDataGetIdAsync(sql,
                                            new
                                            {
                                                departmentId,
                                                addressId,
                                                aliasId,
                                                firstName,
                                                lastName,
                                                emailAddress
                                            },
                                            _connectionStringName);
    }

    /// <summary>
    /// Create Alias for given Staff and save it to Db. If Alias already exists in Db we will
    /// create new one with incrementing order number.
    /// </summary>
    /// <param name="pIN"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <returns></returns>
    public async Task<int> CreateAliasAsync(string pIN, string firstName, string lastName)
    {
        int orderNumber = 0;
        string alias = "";
        int aliasId = 0;
        bool isAliasExistingInDb = true;
        // Repeat until we have available Alias        
        do
        {
            orderNumber++;
            alias = _staffDataProcessor.CreateAlias(firstName, lastName, orderNumber);
            isAliasExistingInDb = await _staffDataProcessor.CheckAliasAsync(alias);
            if (isAliasExistingInDb == false)
                aliasId = await _staffDataProcessor.SaveAliasAsync(alias, pIN);
        } while (aliasId == 0);
        return aliasId;
    }

    /// <summary>
    /// Save Address into Db and return Id.
    /// </summary>
    /// <param name="address">Address Info.</param>
    /// <returns>Address Id.</returns>
    public async Task<int> SaveAddressAsync(AddressModel address)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("Addresses_Insert.sql");
        return await _db.SaveDataGetIdAsync(sql,
                                            new
                                            {
                                                street = address.Street,
                                                city = address.City,
                                                zip = address.Zip,
                                                state = address.State
                                            },
                                            _connectionStringName);
    }

    /// <summary>
    /// Create Phone Numbers for given Staff and save it to Db
    /// </summary>
    /// <param name="staffId">Staffs Id.</param>
    /// <param name="phoneNumbers">Phone Numbers.</param>
    /// <returns></returns>
    public async Task CreatePhoneNumbersAsync(int staffId, List<PhoneNumberModel> phoneNumbers)
    {
        foreach (PhoneNumberModel phoneNumber in phoneNumbers)
        {
            // Check if given Phone Number is already in Db (some Staff can share it because they live together).
            bool isPhoneNumberExistingInDb = await _staffDataProcessor.CheckPhoneNumberAsync(phoneNumber);

            // If we found that Phone Number in Db we get Id and just store it inside relation StaffPhoneNumbers Table.
            if (isPhoneNumberExistingInDb)
            {
                List<PhoneNumberModel> phoneNumberList = await _staffDataProcessor.GetPhoneNumberAsync(phoneNumber);
                await _staffDataProcessor.SavePhoneNumberLinkAsync(staffId, phoneNumberList.First().Id);
            }
            // If its new Phone Number we add it first to PhoneNumbers Table and after we create relation.
            else
            {
                int phoneNumberId = await _staffDataProcessor.SavePhoneNumberAsync(phoneNumber);
                await _staffDataProcessor.SavePhoneNumberLinkAsync(staffId, phoneNumberId);
            }
        }
    }

    /// <summary>
    /// Update Staff in Db. Set isApproved to false because Admin must approve those changes.
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="staff"></param>
    /// <returns></returns>
    public async Task UpdateStaffAsync(string firstName, string lastName, int staffId)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("Staffs_Update.sql");
        await _db.SaveDataAsync(sql,
                                new { id = staffId, firstName, lastName, isApproved = false },
                                _connectionStringName);
    }

    /// <summary>
    /// Update Alias - PIN in Db.
    /// </summary>
    /// <param name="pIN"></param>
    /// <param name="staff"></param>
    /// <returns></returns>
    public async Task UpdateAliasAsync(string pIN, int aliasId)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("Aliases_Update.sql");
        await _db.SaveDataAsync(sql,
                                new { id = aliasId, pIN },
                                _connectionStringName);
    }

    /// <summary>
    /// Update Address in Db.
    /// </summary>
    /// <param name="address"></param>
    /// <param name="staff"></param>
    /// <returns></returns>
    public async Task UpdateAddressAsync(AddressModel address, int addressId)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("Addresses_Update.sql");
        await _db.SaveDataAsync(sql,
                                new
                                {
                                    id = addressId,
                                    street = address.Street,
                                    city = address.City,
                                    zip = address.Zip,
                                    state = address.State
                                },
                                _connectionStringName);
    }

    /// <summary>
    /// Get all Basic Staff Info from Db by Department Id and Approved status.
    /// </summary>
    /// <param name="departmentId"></param>
    /// <param name="approvedType"></param>
    /// <returns>Staff Basic Info.</returns>
    public async Task<List<StaffBasicModel>> GetAllBasicStaffByDepartmentAndApprovedAsync(int departmentId,
                                                                                 ApprovedType approvedType)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("Staffs_GetAllBasicByDepartmentAndApproved.sql");
        return await _db.LoadDataAsync<StaffBasicModel, dynamic>(sql,
                                                                 new { departmentId, isApproved = approvedType == ApprovedType.Approved ? true : false },
                                                                 _connectionStringName);
    }

    /// <summary>
    /// Get all Basic Staff Info from Db by Department Id.
    /// </summary>
    /// <param name="departmentId"></param>
    /// <returns>Staff Basic Info.</returns>
    public async Task<List<StaffBasicModel>> GetAllBasicStaffByDepartmentAsync(int departmentId)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("Staffs_GetAllBasicByDepartment.sql");
        return await _db.LoadDataAsync<StaffBasicModel, dynamic>(sql,
                                                                 new { departmentId },
                                                                 _connectionStringName);
    }

    /// <summary>
    /// Get Phone Numbers by Staff Id from Db.
    /// </summary>
    /// <param name="staffId"></param>
    /// <returns>Collection of Phone Number Info.</returns>
    public async Task<List<PhoneNumberModel>> GetPhoneNumbersByStaffIdAsync(int staffId)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("PhoneNumbers_GetByStaffId.sql");
        return await _db.LoadDataAsync<PhoneNumberModel, dynamic>(sql,
                                                                  new { staffId },
                                                                  _connectionStringName);
    }

    /// <summary>
    /// Get Address by Email from Db.
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <returns>Email Address Info.</returns>
    public async Task<AddressModel> GetAddressByEmailAsync(string emailAddress)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("Addresses_GetByEmail.sql");
        List<AddressModel> output = await _db.LoadDataAsync<AddressModel, dynamic>(sql,
                                                                                   new { emailAddress },
                                                                                   _connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Get Basic Staff Model by Email from Db.
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <returns>Staff Basic Info.</returns>
    public async Task<StaffFullModel> GetStaffByEmailAsync(string emailAddress)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("Staffs_GetBasicByEmail.sql");
        List<StaffFullModel> output = await _db.LoadDataAsync<StaffFullModel, dynamic>(sql,
                                                                                       new { emailAddress },
                                                                                       _connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Get Address by Id from Db.
    /// </summary>
    /// <param name="id">Staff's Id.</param>
    /// <returns>Address Info.</returns>
    public async Task<AddressModel> GetAddressByIdAsync(int id)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("Addresses_GetById.sql");
        List<AddressModel> output = await _db.LoadDataAsync<AddressModel, dynamic>(sql,
                                                                                   new { id },
                                                                                   _connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Get Staff by Id from Db.
    /// </summary>
    /// <param name="id">Staff's Id.</param>
    /// <returns>Staff Info.</returns>
    public async Task<StaffFullModel> GetStaffByIdAsync(int id)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("Staffs_GetById.sql");
        List<StaffFullModel> output = await _db.LoadDataAsync<StaffFullModel, dynamic>(sql,
                                                                                       new { id },
                                                                                       _connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Delete Alias from Db.
    /// </summary>
    /// <param name="staffModel"></param>
    /// <returns></returns>
    public async Task DeleteAliasAsync(int aliasId)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("Aliases_Delete.sql");
        await _db.SaveDataAsync(sql, new { id = aliasId }, _connectionStringName);
    }

    /// <summary>
    /// Delete Address from Db.
    /// </summary>
    /// <param name="staffModel"></param>
    /// <returns></returns>
    public async Task DeleteAddressAsync(int addressId)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("Addresses_Delete.sql");
        await _db.SaveDataAsync(sql, new { id = addressId }, _connectionStringName);
    }

    /// <summary>
    /// Delete Staff from Db.
    /// </summary>
    /// <param name="staffId"></param>
    /// <returns></returns>
    public async Task DeleteStaffAsync(int staffId)
    {
        string sql = await SqliteQueryHelper.LoadQueryAsync("Staffs_Delete.sql");
        await _db.SaveDataAsync(sql, new { staffId }, _connectionStringName);
    }

    /// <summary>
    /// Delete Phone Numbers from Db - PhoneNumbers Table and StaffPhoneNumbers Table.
    /// </summary>
    /// <param name="staffId">Staffs Id.</param>
    /// <param name="phoneNumbers">Phone Numbers.</param>
    /// <returns></returns>
    public async Task DeletePhoneNumbersAsync(int staffId, List<PhoneNumberModel> phoneNumbers)
    {
        foreach (PhoneNumberModel item in phoneNumbers)
        {
            int phoneNumberId = item.Id;

            // Get all links Staff-PhoneNumber for given PhoneNumber Id from relation StaffPhoneNumbers Table
            List<StaffPhoneNumberModel> staffPhoneNumberLinks = await _staffDataProcessor.GetPhoneNumberLinksAsync(phoneNumberId);

            // First we delete link Staff-PhoneNumber from relation StaffPhoneNumbers Table just for given Staff Id.
            await _staffDataProcessor.DeletePhoneNumberLinkAsync(staffId, phoneNumberId);

            // If there was only one link Staff-PhoneNumber than we can delete PhoneNumber from PhoneNumbers Table too.
            if (staffPhoneNumberLinks.Count == 1)
            {
                await _staffDataProcessor.DeletePhoneNumberAsync(phoneNumberId);
            }
        }
    }
}
