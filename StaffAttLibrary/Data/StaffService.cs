using StaffAttLibrary.Db;
using StaffAttLibrary.Enums;
using StaffAttLibrary.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Data;

/// <summary>
/// Class servicing Staffs - CRUD actions.
/// UIs (MVC, WPF) talk to this class. This class calls StaffData class and SqlDataAccess methods.
/// </summary>
public class StaffService : IStaffService
{
    /// <summary>
    /// Servicing SQL database connection.
    /// </summary>
    private readonly ISqlDataAccess _db;
    private readonly IStaffData _staffData;
    private readonly ICheckInData _checkInData;

    /// <summary>
    /// Holds default connection string name.
    /// </summary>
    private readonly string _connectionStringName;

    /// <summary>
    /// Constructor, ISqlDataAccess comes from Dependency Injection from our frontend (UI).
    /// </summary>
    /// <param name="db">Servicing SQL database connection.</param>
    public StaffService(ISqlDataAccess db, IStaffData staffData, ICheckInData checkInData, IConnectionStringData connectionString)
    {
        _db = db;
        _staffData = staffData;
        _checkInData = checkInData;
        _connectionStringName = connectionString.SqlConnectionName;
    }

    /// <summary>
    /// Get all Departments from our database.
    /// </summary>
    /// <returns>Collection of DepartmentModel.</returns>
    public async Task<List<DepartmentModel>> GetAllDepartmentsAsync()
    {
        return await _db.LoadDataAsync<DepartmentModel, dynamic>("spDepartments_GetAll",
                                                            new { },
                                                            _connectionStringName);
    }

    /// <summary>
    /// Create Staff and save it to our database.
    /// </summary>
    /// <param name="departmentId"></param>
    /// <param name="address"></param>
    /// <param name="pIN"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="emailAddress"></param>
    /// <param name="phoneNumbers"></param>
    /// <returns></returns>
    public async Task CreateStaffAsync(int departmentId,
                                 AddressModel address,
                                 string pIN,
                                 string firstName,
                                 string lastName,
                                 string emailAddress,
                                 List<PhoneNumberModel> phoneNumbers)
    {        
        int addressId = await _staffData.SaveAddressAsync(address);
        int aliasId = await _staffData.CreateAliasAsync(pIN, firstName, lastName);
        int staffId = await _staffData.SaveStaffAsync(departmentId,
                                                      firstName,
                                                      lastName,
                                                      emailAddress,
                                                      addressId,
                                                      aliasId);                
        await _staffData.CreatePhoneNumbersAsync(staffId, phoneNumbers);
    }



    /// <summary>
    /// Update Staff and save it to our database.
    /// </summary>
    /// <param name="address"></param>
    /// <param name="pIN"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="emailAddress"></param>
    /// <param name="phoneNumbers"></param>
    /// <returns></returns>
    public async Task UpdateStaffAsync(AddressModel address,
                                 string pIN,
                                 string firstName,
                                 string lastName,
                                 string emailAddress,
                                 List<PhoneNumberModel> phoneNumbers)
    {
        StaffFullModel staff = await GetStaffByEmailAsync(emailAddress);
        await _staffData.UpdateAddressAsync(address, staff.AddressId);
        await _staffData.UpdateAliasAsync(pIN, staff.AliasId);
        await _staffData.UpdateStaffAsync(firstName, lastName, staff.Id);

        // Check if Updated Phone Numbers and Phone Numbers from Db are the same than we do nothing.
        bool isSamePhoneNumber = staff.PhoneNumbers.All(phoneNumbers.Contains) && staff.PhoneNumbers.Count == phoneNumbers.Count;
        if (isSamePhoneNumber == false)
        {
            // Update Phone Numbers, we simply delete old and insert new ones.
            await _staffData.DeletePhoneNumbersAsync(staff.Id, staff.PhoneNumbers);
            await _staffData.CreatePhoneNumbersAsync(staff.Id, phoneNumbers);
        }
    }

    /// <summary>
    /// Verify if entered credentials are correct.
    /// </summary>
    /// <param name="alias">Staff's Alias.</param>
    /// <param name="pIN">Staff's PIN.</param>
    /// <returns>Correct: Alias info. False: null</returns>
    public async Task<AliasModel> AliasVerificationAsync(string alias, string pIN)
    {
        List<AliasModel> output = await _db.LoadDataAsync<AliasModel, dynamic>("spAliases_GetByAliasAndPIN",
                                                                          new { alias, pIN },
                                                                          _connectionStringName);

        return output.FirstOrDefault();
    }

    /// <summary>
    /// Get all Basic Staff Info from Db. If DepartmentId is 0 than we get all Staffs.
    /// If ApprovedType is All than we get all Staffs, otherwise only those with or without Approved status.
    /// </summary>
    /// <param name="departmentId">Departments Id.</param>
    /// <param name="approvedType">Enum for Staff's Approved status.</param>
    /// <returns>Collection of Basic Staff Info.</returns>
    public async Task<List<StaffBasicModel>> GetAllBasicStaffFilteredAsync(int departmentId,
                                                                      ApprovedType approvedType)
    {
        if (approvedType == ApprovedType.All)
            return await _staffData.GetAllBasicStaffByDepartmentAsync(departmentId);

        else
            return await _staffData.GetAllBasicStaffByDepartmentAndApprovedAsync(departmentId, approvedType);
    }

    /// <summary>
    /// Get all Basic Staff Info from Db.
    /// </summary>
    /// <returns>Collection of Basic Staff Info.</returns>
    public async Task<List<StaffBasicModel>> GetAllBasicStaffAsync()
    {
        return await _db.LoadDataAsync<StaffBasicModel, dynamic>("spStaffs_GetAllBasic",
                                                                                    new { },
                                                                                    _connectionStringName);
    }

    /// <summary>
    /// Get Staff by Email from Db.
    /// </summary>
    /// <param name="emailAddress">Staff's email.</param>
    /// <returns>Staff info.</returns>
    public async Task<StaffFullModel> GetStaffByEmailAsync(string emailAddress)
    {
        StaffFullModel staffModel = await _staffData.GetStaffByEmailAsync(emailAddress);
        staffModel.Address = await _staffData.GetAddressByEmailAsync(emailAddress);
        staffModel.PhoneNumbers = await _staffData.GetPhoneNumbersByStaffIdAsync(staffModel.Id);

        return staffModel;
    }

    /// <summary>
    /// Get Staff by Id from Db.
    /// </summary>
    /// <param name="id">Staff's Id.</param>
    /// <returns>Staff info.</returns>
    public async Task<StaffFullModel> GetStaffByIdAsync(int id)
    {
        StaffFullModel staffModel = await _staffData.GetStaffByIdAsync(id);
        staffModel.Address = await _staffData.GetAddressByIdAsync(id);
        staffModel.PhoneNumbers = await _staffData.GetPhoneNumbersByStaffIdAsync(staffModel.Id);

        return staffModel;
    }

    /// <summary>
    /// Get Basic Staff Model by Id from Db.
    /// </summary>
    /// <param name="id">Staff's id.</param>
    /// <returns>Basic Staff info.</returns>
    public async Task<StaffBasicModel> GetBasicStaffByIdAsync(int id)
    {
        List<StaffBasicModel> output = await _db.LoadDataAsync<StaffBasicModel, dynamic>("spStaffs_GetBasicById",
                                                                            new { id },
                                                                            _connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Get Basic Staff Model by AliasId from Db.
    /// </summary>
    /// <param name="aliasId">Alias id.</param>
    /// <returns>Basic Staff info.</returns>
    public async Task<StaffBasicModel> GetBasicStaffByAliasIdAsync(int aliasId)
    {
        List<StaffBasicModel> output = await _db.LoadDataAsync<StaffBasicModel, dynamic>("spStaffs_GetBasicByAliasId",
                                                                            new { aliasId },
                                                                            _connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Check if Staff exists by Email from Db.
    /// </summary>
    /// <param name="emailAddress">Staff's email.</param>
    /// <returns>True if Staff exists.</returns>
    public async Task<bool> CheckStaffByEmailAsync(string emailAddress)
    {
        List<bool> output = await _db.LoadDataAsync<bool, dynamic>("spStaffs_CheckByEmail",
                                                                            new { emailAddress },
                                                                            _connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Update type of Department and Approved status by Admin.
    /// </summary>
    /// <param name="id">Staff's id.</param>
    /// <param name="departmentId">Department's id.</param>
    /// <param name="isApproved">is Approved status</param>
    public async Task UpdateStaffByAdminAsync(int id, int departmentId, bool isApproved)
    {
        await _db.SaveDataAsync("spStaffs_UpdateByAdmin",
                           new { id, departmentId, isApproved },
                           _connectionStringName);
    }

    /// <summary>
    /// Delete Staff from Db. Delete all related Phone Numbers, Addresses and Aliases.
    /// First we delete Phone Numbers (can be shared) because they have Foreign Key into Staffs Table.
    /// Than we must delete Staff because it has Foreign Keys into Addresses and Aliases Tables.
    /// Finally we delete Address and Alias.
    /// </summary>
    /// <param name="staffId">Staff Id.</param>
    /// <returns></returns>
    public async Task DeleteStaffAsync(int staffId)
    {
        StaffFullModel staffModel = await _staffData.GetStaffByIdAsync(staffId);
        staffModel.PhoneNumbers = await _staffData.GetPhoneNumbersByStaffIdAsync(staffId);

        await _checkInData.DeleteCheckInAsync(staffId);
        await _staffData.DeletePhoneNumbersAsync(staffId, staffModel.PhoneNumbers);
        await _staffData.DeleteStaffAsync(staffId);
        await _staffData.DeleteAddressAsync(staffModel.AddressId);
        await _staffData.DeleteAliasAsync(staffModel.AliasId);
    }
}
