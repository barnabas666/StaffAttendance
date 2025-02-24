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
public class StaffData : IStaffData
{
    /// <summary>
    /// Servicing SQL database connection.
    /// </summary>
    private readonly ISqlDataAccess _db;

    /// <summary>
    /// Holds default connection string name.
    /// </summary>
    private const string connectionStringName = "Testing";

    /// <summary>
    /// Constructor, ISqlDataAccess comes from Dependency Injection from our frontend (UI).
    /// </summary>
    /// <param name="db">Servicing SQL database connection.</param>
    public StaffData(ISqlDataAccess db)
    {
        _db = db;
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
        int addressId = await SaveAddress(address);

        int orderNumber = 0;
        string alias = "";
        int aliasId = 0;
        // Repeat until we have available Alias
        // (SP checks if Alias is available and if yes creates new Alias and returns last inserted Id).
        do
        {
            orderNumber++;
            alias = CreateAlias(firstName, lastName, orderNumber);
            aliasId = await CheckAndInsertAlias(pIN, alias, aliasId);
        } while (aliasId == 0);

        int staffId = await SaveStaff(departmentId, firstName, lastName, emailAddress, addressId, aliasId);

        // Add Phone Numbers into Db.
        await CreatePhoneNumbers(staffId, phoneNumbers);
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
    private async Task<int> SaveStaff(int departmentId,
                                      string firstName,
                                      string lastName,
                                      string emailAddress,
                                      int addressId,
                                      int aliasId)
    {

        // Save Staff into Db and returns back Id.
        return await _db.SaveDataGetId("spStaffs_Insert",
                                              new
                                              {
                                                  departmentId,
                                                  addressId,
                                                  aliasId,
                                                  firstName,
                                                  lastName,
                                                  emailAddress
                                              },
                                              connectionStringName);
    }

    /// <summary>
    /// Check if Alias exists in Db, if not than insert it.
    /// </summary>
    /// <param name="pIN"></param>
    /// <param name="alias"></param>
    /// <param name="aliasId"></param>
    /// <returns>Alias Id.</returns>
    private async Task<int> CheckAndInsertAlias(string pIN, string alias, int aliasId)
    {
        aliasId = await _db.SaveDataGetId("spAliases_CheckAndInsert",
                                          new { alias, pIN },
                                          connectionStringName);
        return aliasId;
    }

    /// <summary>
    /// Save Address into Db and return Id.
    /// </summary>
    /// <param name="address">Address Info.</param>
    /// <returns>Address Id.</returns>
    private async Task<int> SaveAddress(AddressModel address)
    {
        return await _db.SaveDataGetId("spAddresses_Insert",
                                                        new
                                                        {
                                                            street = address.Street,
                                                            city = address.City,
                                                            zip = address.Zip,
                                                            state = address.State
                                                        },
                                                        connectionStringName);
    }

    /// <summary>
    /// Create Phone Numbers for given Staff and save it to Db
    /// </summary>
    /// <param name="staffId">Staffs Id.</param>
    /// <param name="phoneNumbers">Phone Numbers.</param>
    /// <returns></returns>
    private async Task CreatePhoneNumbers(int staffId, List<PhoneNumberModel> phoneNumbers)
    {
        int phoneNumberId = 0;
        foreach (PhoneNumberModel phoneNumber in phoneNumbers)
        {
            // Check if given Phone Number is already in Db (some Staff can share it because they live together).
            List<PhoneNumberModel> phoneNumberList = await _db.LoadData<PhoneNumberModel, dynamic>(
                "spPhoneNumbers_GetByPhoneNumber",
                new { phoneNumber = phoneNumber.PhoneNumber },
                connectionStringName);

            // If we found that Phone Number in Db we just store it inside relation StaffPhoneNumbers Table.
            if (phoneNumberList.Count == 1)
            {
                await _db.SaveData("spStaffPhoneNumbers_Insert",
                                   new { staffId, phoneNumberId = phoneNumberList.First().Id },
                                   connectionStringName);
            }
            // If its new Phone Number we add it to PhoneNumbers Table too.
            else
            {
                phoneNumberId = await _db.SaveDataGetId("spPhoneNumbers_Insert",
                                                        new { phoneNumber = phoneNumber.PhoneNumber },
                                                        connectionStringName);
                await _db.SaveData("spStaffPhoneNumbers_Insert",
                                   new { staffId, phoneNumberId },
                                   connectionStringName);
            }
        }
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
        await UpdateAddress(address, staff);
        await UpdateAlias(pIN, staff);
        await UpdateStaff(firstName, lastName, staff);

        // Check if Updated Phone Numbers and Phone Numbers from Db are the same than we do nothing.
        bool isSamePhoneNumber = staff.PhoneNumbers.All(phoneNumbers.Contains) && staff.PhoneNumbers.Count == phoneNumbers.Count;
        if (isSamePhoneNumber == false)
        {
            // Update Phone Numbers, we simply delete old and insert new ones.
            await DeletePhoneNumbers(staff.Id, staff.PhoneNumbers);
            await CreatePhoneNumbers(staff.Id, phoneNumbers);
        }
    }

    /// <summary>
    /// Update Staff in Db.
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="staff"></param>
    /// <returns></returns>
    private async Task UpdateStaff(string firstName, string lastName, StaffFullModel staff)
    {

        // Update Staff, set isApproved to false because Admin must approve those changes.
        await _db.SaveData("spStaffs_Update",
                           new { id = staff.Id, firstName, lastName, isApproved = false },
                           connectionStringName);
    }

    /// <summary>
    /// Update Alias in Db.
    /// </summary>
    /// <param name="pIN"></param>
    /// <param name="staff"></param>
    /// <returns></returns>
    private async Task UpdateAlias(string pIN, StaffFullModel staff)
    {

        // Update Alias
        await _db.SaveData("spAliases_Update",
                           new { id = staff.AliasId, pIN },
                           connectionStringName);
    }

    /// <summary>
    /// Update Address in Db.
    /// </summary>
    /// <param name="address"></param>
    /// <param name="staff"></param>
    /// <returns></returns>
    private async Task UpdateAddress(AddressModel address, StaffFullModel staff)
    {

        // Update Address
        await _db.SaveData("spAddresses_Update",
                           new
                           {
                               id = staff.AddressId,
                               street = address.Street,
                               city = address.City,
                               zip = address.Zip,
                               state = address.State
                           },
                           connectionStringName);
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
            return await GetAllBasicStaffByDepartment(departmentId);

        else
            return await GetAllBasicStaffByDepartmentAndApproved(departmentId, approvedType);        
    }

    /// <summary>
    /// Get all Basic Staff Info from Db by Department Id and Approved status.
    /// </summary>
    /// <param name="departmentId"></param>
    /// <param name="approvedType"></param>
    /// <returns>Staff Basic Info.</returns>
    private async Task<List<StaffBasicModel>> GetAllBasicStaffByDepartmentAndApproved(int departmentId,
                                                                                 ApprovedType approvedType)
    {
        return await _db.LoadData<StaffBasicModel, dynamic>("spStaffs_GetAllBasicByDepartmentAndApproved",
                                                              new { departmentId, isApproved = approvedType == ApprovedType.Approved ? true : false },
                                                              connectionStringName);
    }

    /// <summary>
    /// Get all Basic Staff Info from Db by Department Id.
    /// </summary>
    /// <param name="departmentId"></param>
    /// <returns>Staff Basic Info.</returns>
    private async Task<List<StaffBasicModel>> GetAllBasicStaffByDepartment(int departmentId)
    {
        return await _db.LoadData<StaffBasicModel, dynamic>(storedProcedure: "spStaffs_GetAllBasicByDepartment",
                                                              new { departmentId },
                                                              connectionStringName);
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
        StaffFullModel staffModel = await GetStaffByEmail(emailAddress);

        staffModel.Address = await GetAddressByEmail(emailAddress);

        staffModel.PhoneNumbers = await GetPhoneNumbersByStaffId(staffModel.Id);

        return staffModel;
    }

    /// <summary>
    /// Get Phone Numbers by Staff Id from Db.
    /// </summary>
    /// <param name="staffId"></param>
    /// <returns>Collection of Phone Number Info.</returns>
    private async Task<List<PhoneNumberModel>> GetPhoneNumbersByStaffId(int staffId)
    {
        return await _db.LoadData<PhoneNumberModel, dynamic>("spPhoneNumbers_GetByStaffId",
                                                                               new { staffId },
                                                                               connectionStringName);
    }

    /// <summary>
    /// Get Address by Email from Db.
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <returns>Email Address Info.</returns>
    private async Task<AddressModel> GetAddressByEmail(string emailAddress)
    {
        List<AddressModel> output = await _db.LoadData<AddressModel, dynamic>("spAddresses_GetByEmail",
                                                                              new { emailAddress },
                                                                              connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Get Basic Staff Model by Email from Db.
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <returns>Staff Basic Info.</returns>
    private async Task<StaffFullModel> GetStaffByEmail(string emailAddress)
    {
        List<StaffFullModel> output = await _db.LoadData<StaffFullModel, dynamic>("spStaffs_GetBasicByEmail",
                                                                            new { emailAddress },
                                                                            connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Get Staff by Id from Db.
    /// </summary>
    /// <param name="id">Staff's Id.</param>
    /// <returns>Staff info.</returns>
    public async Task<StaffFullModel> GetStaffByIdProcess(int id)
    {
        StaffFullModel staffModel = await GetStaffById(id);

        staffModel.Address = await GetAddressById(id);

        staffModel.PhoneNumbers = await GetPhoneNumbersByStaffId(staffModel.Id);

        return staffModel;
    }

    /// <summary>
    /// Get Address by Id from Db.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Address Info.</returns>
    private async Task<AddressModel> GetAddressById(int id)
    {
        List<AddressModel> output = await _db.LoadData<AddressModel, dynamic>("spAddresses_GetById",
                                                                      new { id },
                                                                      connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Get Staff by Id from Db.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Staff Info.</returns>
    private async Task<StaffFullModel> GetStaffById(int id)
    {
        List<StaffFullModel> output = await _db.LoadData<StaffFullModel, dynamic>("spStaffs_GetById",
                                                                            new { id },
                                                                            connectionStringName);
        return output.FirstOrDefault();
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
        StaffFullModel staffModel = await GetStaffById(staffId);
        staffModel.PhoneNumbers = await GetPhoneNumbersByStaffId(staffId);

        await DeletePhoneNumbers(staffId, staffModel.PhoneNumbers);
        await DeleteStaff(staffId);
        await DeleteAddress(staffModel);
        await DeleteAlias(staffModel);
    }

    /// <summary>
    /// Delete Alias from Db.
    /// </summary>
    /// <param name="staffModel"></param>
    /// <returns></returns>
    private async Task DeleteAlias(StaffFullModel staffModel)
    {
        await _db.SaveData("spAliases_Delete", new { id = staffModel.AliasId }, connectionStringName);
    }

    /// <summary>
    /// Delete Address from Db.
    /// </summary>
    /// <param name="staffModel"></param>
    /// <returns></returns>
    private async Task DeleteAddress(StaffFullModel staffModel)
    {        
        await _db.SaveData("spAddresses_Delete", new { id = staffModel.AddressId }, connectionStringName);
    }

    /// <summary>
    /// Delete Staff from Db.
    /// </summary>
    /// <param name="staffId"></param>
    /// <returns></returns>
    private async Task DeleteStaff(int staffId)
    {        
        await _db.SaveData("spStaffs_Delete", new { staffId }, connectionStringName);
    }

    /// <summary>
    /// Delete Phone Numbers from Db - PhoneNumbers Table and StaffPhoneNumbers Table.
    /// </summary>
    /// <param name="staffId">Staffs Id.</param>
    /// <param name="phoneNumbers">Phone Numbers.</param>
    /// <returns></returns>
    private async Task DeletePhoneNumbers(int staffId, List<PhoneNumberModel> phoneNumbers)
    {
        foreach (PhoneNumberModel item in phoneNumbers)
        {
            int phoneNumberId = item.Id;

            // Get all links Staff-PhoneNumber for given PhoneNumber Id from relation StaffPhoneNumbers Table
            List<StaffPhoneNumberModel> staffPhoneNumberList = await _db.LoadData<StaffPhoneNumberModel, dynamic>(
                "spStaffPhoneNumbers_GetByPhoneNumber",
                new { phoneNumberId },
                connectionStringName);

            // First we delete link Staff-PhoneNumber from relation StaffPhoneNumbers Table just for given Staff Id.
            await _db.SaveData("spStaffPhoneNumbers_Delete",
                               new { staffId, phoneNumberId },
                               connectionStringName);

            // If there was only one link Staff-PhoneNumber than we can delete PhoneNumber from PhoneNumbers Table too.
            if (staffPhoneNumberList.Count == 1)
            {
                await _db.SaveData("spPhoneNumbers_Delete",
                                   new { phoneNumberId },
                                   connectionStringName);
            }
        }
    }

    /// <summary>
    /// Should move into some helper class!
    /// Create Alias from First Name, Last Name and Order Number (just in case of same aliases)
    /// John Doe will have alias JDO1, Jason Doherty JDO2, Susan Storm SST1 etc. 
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="orderNumber"></param>
    /// <returns></returns>
    private string CreateAlias(string firstName, string lastName, int orderNumber)
    {
        string output = "";

        output += firstName.Substring(0, 1).ToUpper();
        output += lastName.Substring(0, 2).ToUpper();
        output += orderNumber.ToString();

        return output;
    }
}
