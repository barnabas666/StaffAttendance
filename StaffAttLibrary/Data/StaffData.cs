using StaffAttLibrary.Db;
using StaffAttLibrary.Enums;
using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Data;

/// <summary>
/// Class servicing Staffs - CRUD actions.
/// StaffService talks to this class. This class calls StaffDataProcessor class and SqlDataAccess methods.
/// </summary>
public class StaffData : IStaffData
{
    private readonly ISqlDataAccess _db;
    private readonly IStaffDataProcessor _staffDataProcessor;

    /// <summary>
    /// Holds default connection string name.
    /// </summary>
    private readonly string _connectionStringName;

    public StaffData(ISqlDataAccess db, IStaffDataProcessor staffDataProcessor,ConnectionStringData connectionString)
    {
        _db = db;
        _staffDataProcessor = staffDataProcessor;
        _connectionStringName = connectionString.SqlConnectionName;
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
    public async Task<int> SaveStaff(int departmentId,
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
                                              _connectionStringName);
    }

    /// <summary>
    /// Check if Alias exists in Db, if not than insert it and return Id.
    /// If same Alias exists just return Id with default passed value = 0.
    /// </summary>
    /// <param name="pIN"></param>
    /// <param name="alias"></param>
    /// <param name="aliasId"></param>
    /// <returns>Alias Id.</returns>
    public async Task<int> CheckAndInsertAlias(string pIN, string alias, int aliasId)
    {
        aliasId = await _db.SaveDataGetId("spAliases_CheckAndInsert",
                                          new { alias, pIN },
                                          _connectionStringName);
        return aliasId;
    }

    /// <summary>
    /// Save Address into Db and return Id.
    /// </summary>
    /// <param name="address">Address Info.</param>
    /// <returns>Address Id.</returns>
    public async Task<int> SaveAddress(AddressModel address)
    {
        return await _db.SaveDataGetId("spAddresses_Insert",
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
    public async Task CreatePhoneNumbers(int staffId, List<PhoneNumberModel> phoneNumbers)
    {
        int phoneNumberId = 0;
        foreach (PhoneNumberModel phoneNumber in phoneNumbers)
        {
            // Check if given Phone Number is already in Db (some Staff can share it because they live together).
            List<PhoneNumberModel> phoneNumberList = await _staffDataProcessor.GetByPhoneNumber(phoneNumber);

            // If we found that Phone Number in Db we just store it inside relation StaffPhoneNumbers Table.
            if (phoneNumberList.Count == 1)
                await _staffDataProcessor.CreatePhoneNumberLink(staffId, phoneNumberList.First().Id);
            // If its new Phone Number we add it first to PhoneNumbers Table and after we create relation.
            else
            {
                phoneNumberId = await _staffDataProcessor.CreatePhoneNumber(phoneNumberId, phoneNumber);
                await _staffDataProcessor.CreatePhoneNumberLink(staffId, phoneNumberId);
            }
        }
    }

    /// <summary>
    /// Update Staff in Db.
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="staff"></param>
    /// <returns></returns>
    public async Task UpdateStaff(string firstName, string lastName, StaffFullModel staff)
    {

        // Update Staff, set isApproved to false because Admin must approve those changes.
        await _db.SaveData("spStaffs_Update",
                           new { id = staff.Id, firstName, lastName, isApproved = false },
                           _connectionStringName);
    }

    /// <summary>
    /// Update Alias - PIN in Db.
    /// </summary>
    /// <param name="pIN"></param>
    /// <param name="staff"></param>
    /// <returns></returns>
    public async Task UpdateAlias(string pIN, StaffFullModel staff)
    {

        // Update Alias - PIN
        await _db.SaveData("spAliases_Update",
                           new { id = staff.AliasId, pIN },
                           _connectionStringName);
    }

    /// <summary>
    /// Update Address in Db.
    /// </summary>
    /// <param name="address"></param>
    /// <param name="staff"></param>
    /// <returns></returns>
    public async Task UpdateAddress(AddressModel address, StaffFullModel staff)
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
                           _connectionStringName);
    }

    /// <summary>
    /// Get all Basic Staff Info from Db by Department Id and Approved status.
    /// </summary>
    /// <param name="departmentId"></param>
    /// <param name="approvedType"></param>
    /// <returns>Staff Basic Info.</returns>
    public async Task<List<StaffBasicModel>> GetAllBasicStaffByDepartmentAndApproved(int departmentId,
                                                                                 ApprovedType approvedType)
    {
        return await _db.LoadData<StaffBasicModel, dynamic>("spStaffs_GetAllBasicByDepartmentAndApproved",
                                                              new { departmentId, isApproved = approvedType == ApprovedType.Approved ? true : false },
                                                              _connectionStringName);
    }

    /// <summary>
    /// Get all Basic Staff Info from Db by Department Id.
    /// </summary>
    /// <param name="departmentId"></param>
    /// <returns>Staff Basic Info.</returns>
    public async Task<List<StaffBasicModel>> GetAllBasicStaffByDepartment(int departmentId)
    {
        return await _db.LoadData<StaffBasicModel, dynamic>(storedProcedure: "spStaffs_GetAllBasicByDepartment",
                                                              new { departmentId },
                                                              _connectionStringName);
    }

    /// <summary>
    /// Get Phone Numbers by Staff Id from Db.
    /// </summary>
    /// <param name="staffId"></param>
    /// <returns>Collection of Phone Number Info.</returns>
    public async Task<List<PhoneNumberModel>> GetPhoneNumbersByStaffId(int staffId)
    {
        return await _db.LoadData<PhoneNumberModel, dynamic>("spPhoneNumbers_GetByStaffId",
                                                                               new { staffId },
                                                                               _connectionStringName);
    }

    /// <summary>
    /// Get Address by Email from Db.
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <returns>Email Address Info.</returns>
    public async Task<AddressModel> GetAddressByEmail(string emailAddress)
    {
        List<AddressModel> output = await _db.LoadData<AddressModel, dynamic>("spAddresses_GetByEmail",
                                                                              new { emailAddress },
                                                                              _connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Get Basic Staff Model by Email from Db.
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <returns>Staff Basic Info.</returns>
    public async Task<StaffFullModel> GetStaffByEmail(string emailAddress)
    {
        List<StaffFullModel> output = await _db.LoadData<StaffFullModel, dynamic>("spStaffs_GetBasicByEmail",
                                                                            new { emailAddress },
                                                                            _connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Get Address by Id from Db.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Address Info.</returns>
    public async Task<AddressModel> GetAddressById(int id)
    {
        List<AddressModel> output = await _db.LoadData<AddressModel, dynamic>("spAddresses_GetById",
                                                                      new { id },
                                                                      _connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Get Staff by Id from Db.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Staff Info.</returns>
    public async Task<StaffFullModel> GetStaffById(int id)
    {
        List<StaffFullModel> output = await _db.LoadData<StaffFullModel, dynamic>("spStaffs_GetById",
                                                                            new { id },
                                                                            _connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Delete Alias from Db.
    /// </summary>
    /// <param name="staffModel"></param>
    /// <returns></returns>
    public async Task DeleteAlias(StaffFullModel staffModel)
    {
        await _db.SaveData("spAliases_Delete", new { id = staffModel.AliasId }, _connectionStringName);
    }

    /// <summary>
    /// Delete Address from Db.
    /// </summary>
    /// <param name="staffModel"></param>
    /// <returns></returns>
    public async Task DeleteAddress(StaffFullModel staffModel)
    {
        await _db.SaveData("spAddresses_Delete", new { id = staffModel.AddressId }, _connectionStringName);
    }

    /// <summary>
    /// Delete Staff from Db.
    /// </summary>
    /// <param name="staffId"></param>
    /// <returns></returns>
    public async Task DeleteStaff(int staffId)
    {
        await _db.SaveData("spStaffs_Delete", new { staffId }, _connectionStringName);
    }

    /// <summary>
    /// Delete Phone Numbers from Db - PhoneNumbers Table and StaffPhoneNumbers Table.
    /// </summary>
    /// <param name="staffId">Staffs Id.</param>
    /// <param name="phoneNumbers">Phone Numbers.</param>
    /// <returns></returns>
    public async Task DeletePhoneNumbers(int staffId, List<PhoneNumberModel> phoneNumbers)
    {
        foreach (PhoneNumberModel item in phoneNumbers)
        {
            int phoneNumberId = item.Id;

            // Get all links Staff-PhoneNumber for given PhoneNumber Id from relation StaffPhoneNumbers Table
            List<StaffPhoneNumberModel> staffPhoneNumberLinks = await _staffDataProcessor.GetPhoneNumberLinks(phoneNumberId);

            // First we delete link Staff-PhoneNumber from relation StaffPhoneNumbers Table just for given Staff Id.
            await _staffDataProcessor.DeletePhoneNumberLink(staffId, phoneNumberId);

            // If there was only one link Staff-PhoneNumber than we can delete PhoneNumber from PhoneNumbers Table too.
            if (staffPhoneNumberLinks.Count == 1)
            {
                await _staffDataProcessor.DeletePhoneNumber(phoneNumberId);
            }
        }
    }

    /// <summary>
    /// Create Alias from First Name, Last Name and Order Number (just in case of same aliases)
    /// John Doe will have alias JDO1, Jason Doherty JDO2, Susan Storm SST1 etc. 
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
