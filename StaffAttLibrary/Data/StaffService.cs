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
/// UIs (MVC, WPF) talk to this class. This class calls SqlDataAccess methods.
/// </summary>
public class StaffService : IStaffService
{
    /// <summary>
    /// Servicing SQL database connection.
    /// </summary>
    private readonly ISqlDataAccess _db;
    private readonly IStaffData _staffData;

    /// <summary>
    /// Holds default connection string name.
    /// </summary>
    private const string connectionStringName = "Testing";

    /// <summary>
    /// Constructor, ISqlDataAccess comes from Dependency Injection from our frontend (UI).
    /// </summary>
    /// <param name="db">Servicing SQL database connection.</param>
    public StaffService(ISqlDataAccess db, IStaffData staffData)
    {
        _db = db;
        _staffData = staffData;
    }

    /// <summary>
    /// Get all Departments from our database.
    /// </summary>
    /// <returns>Collection of DepartmentModel.</returns>
    public async Task<List<DepartmentModel>> GetAllDepartments()
    {
        return await _db.LoadData<DepartmentModel, dynamic>("spDepartments_GetAll",
                                                            new { },
                                                            connectionStringName);
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
    public async Task CreateStaff(int departmentId,
                                 AddressModel address,
                                 string pIN,
                                 string firstName,
                                 string lastName,
                                 string emailAddress,
                                 List<PhoneNumberModel> phoneNumbers)
    {
        // Save Address into Db and returns back Id.
        int addressId = await _staffData.SaveAddress(address);

        int orderNumber = 0;
        string alias = "";
        int aliasId = 0;
        // Repeat until we have available Alias
        // (SP checks if Alias is available and if yes creates new Alias and returns last inserted Id).
        do
        {
            orderNumber++;
            alias = _staffData.CreateAlias(firstName, lastName, orderNumber);
            aliasId = await _staffData.CheckAndInsertAlias(pIN, alias, aliasId);
        } while (aliasId == 0);

        int staffId = await _staffData.SaveStaff(departmentId, firstName, lastName, emailAddress, addressId, aliasId);

        // Add Phone Numbers into Db.
        await _staffData.CreatePhoneNumbers(staffId, phoneNumbers);
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
    public async Task UpdateStaffProcess(AddressModel address,
                                 string pIN,
                                 string firstName,
                                 string lastName,
                                 string emailAddress,
                                 List<PhoneNumberModel> phoneNumbers)
    {
        StaffFullModel staff = await GetStaffByEmailProcess(emailAddress);
        await _staffData.UpdateAddress(address, staff);
        await _staffData.UpdateAlias(pIN, staff);
        await _staffData.UpdateStaff(firstName, lastName, staff);

        // Check if Updated Phone Numbers and Phone Numbers from Db are the same than we do nothing.
        bool isSamePhoneNumber = staff.PhoneNumbers.All(phoneNumbers.Contains) && staff.PhoneNumbers.Count == phoneNumbers.Count;
        if (isSamePhoneNumber == false)
        {
            // Update Phone Numbers, we simply delete old and insert new ones.
            await _staffData.DeletePhoneNumbers(staff.Id, staff.PhoneNumbers);
            await _staffData.CreatePhoneNumbers(staff.Id, phoneNumbers);
        }
    }

    /// <summary>
    /// Verify if entered credentials are correct.
    /// </summary>
    /// <param name="alias">Staff's Alias.</param>
    /// <param name="pIN">Staff's PIN.</param>
    /// <returns>Correct: Alias info. False: null</returns>
    public async Task<AliasModel> AliasVerification(string alias, string pIN)
    {
        List<AliasModel> output = await _db.LoadData<AliasModel, dynamic>("spAliases_GetByAliasAndPIN",
                                                                          new { alias, pIN },
                                                                          connectionStringName);

        return output.FirstOrDefault();
    }

    /// <summary>
    /// Get all Basic Staff Info from Db. If DepartmentId is 0 than we get all Staffs.
    /// If ApprovedType is All than we get all Staffs, otherwise only those with or without Approved status.
    /// </summary>
    /// <param name="departmentId">Departments Id.</param>
    /// <param name="approvedType">Enum for Staff's Approved status.</param>
    /// <returns>Collection of Basic Staff Info.</returns>
    public async Task<List<StaffBasicModel>> GetAllBasicStaffFiltered(int departmentId,
                                                                      ApprovedType approvedType)
    {       
        if (approvedType == ApprovedType.All)
            return await _staffData.GetAllBasicStaffByDepartment(departmentId);

        else
            return await _staffData.GetAllBasicStaffByDepartmentAndApproved(departmentId, approvedType);        
    }   

    /// <summary>
    /// Get all Basic Staff Info from Db.
    /// </summary>
    /// <returns>Collection of Basic Staff Info.</returns>
    public async Task<List<StaffBasicModel>> GetAllBasicStaff()
    {
        return await _db.LoadData<StaffBasicModel, dynamic>("spStaffs_GetAllBasic",
                                                                                    new { },
                                                                                    connectionStringName);        
    }

    /// <summary>
    /// Get Staff by Email from Db.
    /// </summary>
    /// <param name="emailAddress">Staff's email.</param>
    /// <returns>Staff info.</returns>
    public async Task<StaffFullModel> GetStaffByEmailProcess(string emailAddress)
    {
        StaffFullModel staffModel = await _staffData.GetStaffByEmail(emailAddress);

        staffModel.Address = await _staffData.GetAddressByEmail(emailAddress);

        staffModel.PhoneNumbers = await _staffData.GetPhoneNumbersByStaffId(staffModel.Id);

        return staffModel;
    }

    /// <summary>
    /// Get Staff by Id from Db.
    /// </summary>
    /// <param name="id">Staff's Id.</param>
    /// <returns>Staff info.</returns>
    public async Task<StaffFullModel> GetStaffByIdProcess(int id)
    {
        StaffFullModel staffModel = await _staffData.GetStaffById(id);

        staffModel.Address = await _staffData.GetAddressById(id);

        staffModel.PhoneNumbers = await _staffData.GetPhoneNumbersByStaffId(staffModel.Id);

        return staffModel;
    }

    /// <summary>
    /// Get Basic Staff Model by Id from Db.
    /// </summary>
    /// <param name="id">Staff's id.</param>
    /// <returns>Basic Staff info.</returns>
    public async Task<StaffBasicModel> GetBasicStaffById(int id)
    {
        List<StaffBasicModel> output = await _db.LoadData<StaffBasicModel, dynamic>("spStaffs_GetBasicById",
                                                                            new { id },
                                                                            connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Get Basic Staff Model by AliasId from Db.
    /// </summary>
    /// <param name="aliasId">Alias id.</param>
    /// <returns>Basic Staff info.</returns>
    public async Task<StaffBasicModel> GetBasicStaffByAliasId(int aliasId)
    {
        List<StaffBasicModel> output = await _db.LoadData<StaffBasicModel, dynamic>("spStaffs_GetBasicByAliasId",
                                                                            new { aliasId },
                                                                            connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Check if Staff exists by Email from Db.
    /// </summary>
    /// <param name="emailAddress">Staff's email.</param>
    /// <returns>True if Staff exists.</returns>
    public async Task<bool> CheckStaffByEmail(string emailAddress)
    {
        List<bool> output = await _db.LoadData<bool, dynamic>("spStaffs_CheckByEmail",
                                                                            new { emailAddress },
                                                                            connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Update type of Department and Approved status by Admin.
    /// </summary>
    /// <param name="id">Staff's id.</param>
    /// <param name="departmentId">Department's id.</param>
    /// <param name="isApproved">is Approved status</param>
    public async Task UpdateStaffByAdmin(int id, int departmentId, bool isApproved)
    {
        await _db.SaveData("spStaffs_UpdateByAdmin",
                           new { id, departmentId, isApproved },
                           connectionStringName);
    }

    /// <summary>
    /// Delete Staff from Db. Delete all related Phone Numbers, Addresses and Aliases.
    /// First we delete Phone Numbers (can be shared) because they have Foreign Key into Staffs Table.
    /// Than we must delete Staff because it has Foreign Keys into Addresses and Aliases Tables.
    /// Finally we delete Address and Alias.
    /// </summary>
    /// <param name="staffId">Staff Id.</param>
    /// <returns></returns>
    public async Task DeleteStaffProcess(int staffId)
    {
        StaffFullModel staffModel = await _staffData.GetStaffById(staffId);
        staffModel.PhoneNumbers = await _staffData.GetPhoneNumbersByStaffId(staffId);

        await _staffData.DeletePhoneNumbers(staffId, staffModel.PhoneNumbers);
        await _staffData.DeleteStaff(staffId);
        await _staffData.DeleteAddress(staffModel);
        await _staffData.DeleteAlias(staffModel);
    }
}
