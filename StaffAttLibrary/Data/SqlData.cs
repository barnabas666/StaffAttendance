using StaffAttLibrary.Db;
using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Data;

/// <summary>
/// Class servicing Staffs - CRUD actions. Maybe should make class for Staffs and other for CheckIns
/// UIs (MVC, WPF) talk to this class. This class calls SqlDataAccess methods.
/// </summary>
public class SqlData : IDatabaseData
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
    public SqlData(ISqlDataAccess db)
    {
        _db = db;
    }

    /// <summary>
    /// Get all Departments from our database.
    /// </summary>
    /// <returns>Collection of DepartmentModel.</returns>
    public async Task<List<DepartmentModel>> GetAllDepartments()
    {
        return await _db.LoadData<DepartmentModel, dynamic>("spDepartments_GetAll", new { }, connectionStringName);
    }

    /// <summary>
    /// Create Staff and save it to our database, needs some refactoring into more methods.
    /// All parameters are pretty straightforward
    /// </summary>
    /// <param name="departmentId"></param>
    /// <param name="street"></param>
    /// <param name="city"></param>
    /// <param name="zip"></param>
    /// <param name="state"></param>
    /// <param name="pIN"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="emailAddress"></param>
    /// <param name="phoneNumbers"></param>
    public async void CreateStaff(int departmentId,
                            string street,
                            string city,
                            string zip,
                            string state,
                            string pIN,
                            string firstName,
                            string lastName,
                            string emailAddress,
                            List<string> phoneNumbers)
    {
        // Save Address into Db and returns back Id.
        int addressId = await _db.SaveDataGetId("spAddresses_Insert",
                                                new { street, city, zip, state },
                                                connectionStringName);

        int orderNumber = 0;
        string alias = "";
        int aliasId = 0;

        // Repeat until we have available Alias (SP returns last inserted Id).
        do
        {
            orderNumber++;
            alias = CreateAlias(firstName, lastName, orderNumber);
            aliasId = await _db.SaveDataGetId("spAliases_Insert",
                                              new { alias, pIN },
                                              connectionStringName);
        } while (aliasId == 0);

        // Save Alias into Db and returns back Id.
        int staffId = await _db.SaveDataGetId("spStaffs_Insert",
                                              new { departmentId, addressId, aliasId, firstName, lastName, emailAddress },
                                              connectionStringName);

        // Add Phone Numbers into Db.
        int phoneNumberId = 0;
        foreach (string phoneNumber in phoneNumbers)
        {
            // Check if given Phone Number is already in Db (some Staff can share it because they live together).
            List<PhoneNumberModel> phoneNumberList = await _db.LoadData<PhoneNumberModel, dynamic>(
                "spPhoneNumbers_GetByPhoneNumber",
                new { phoneNumber },
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
                                                        new { phoneNumber },
                                                        connectionStringName);
                await _db.SaveData("spStaffPhoneNumbers_Insert",
                                   new { staffId, phoneNumberId },
                                   connectionStringName);
            }
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
        List<AliasModel> output = await _db.LoadData<AliasModel, dynamic>("spAliases_Verification",
                                                                          new { alias, pIN },
                                                                          connectionStringName);

        return output.FirstOrDefault();
    }

    /// <summary>
    /// Check if Staff is approved to do CheckIn/CheckOut
    /// </summary>
    /// <param name="aliasId">Staff's Alias info.</param>
    /// <returns>True if Staff is approved, false if not. ArgumentException for invalid aliasId.</returns>
    /// <exception cref="ArgumentException">For invalid aliasId.</exception>
    public async Task<bool> CheckApproveStatus(int aliasId)
    {
        List<StaffFullModel> model = await _db.LoadData<StaffFullModel, dynamic>("spStaffs_GetByAlias",
                                                                                 new { aliasId },
                                                                                 connectionStringName);

        if (model.FirstOrDefault() == null)
            throw new ArgumentException("You passed in an invalid parameter", "aliasId");

        return model.First().IsApproved;
    }

    /// <summary>
    /// Get last record from CheckIn Table.
    /// </summary>
    /// <param name="staffId">Staff's Id.</param>
    /// <returns>Last CheckIn (CheckOut prop can be null) info or null for invalid staffId.</returns>
    public async Task<CheckInModel> GetLastCheckIn(int staffId)
    {
        List<CheckInModel> output = await _db.LoadData<CheckInModel, dynamic>("spCheckIns_GetLastRecord",
                                                                  new { staffId },
                                                                  connectionStringName);

        return output.FirstOrDefault();
    }

    /// <summary>
    /// Perform CheckIn for given Staff with current date.
    /// </summary>
    /// <param name="staffId">Staff's id.</param>
    /// <returns>CheckIn id.</returns>
    public async Task<int> CheckInPerform(int staffId)
    {
        int checkInId = await _db.SaveDataGetId("spCheckIns_InsertCheckIn",
                           new { staffId },
                           connectionStringName);

        return checkInId;
    }

    /// <summary>
    /// Perform CheckOut for given CheckIn record with current date.
    /// </summary>
    /// <param name="checkInId">CheckIn's id.</param>
    /// <returns>CheckIn id.</returns>
    public async Task<int> CheckOutPerform(int checkInId)
    {
        checkInId = await _db.SaveDataGetId("spCheckIns_InsertCheckOut",
                           new { checkInId },
                           connectionStringName);

        return checkInId;
    }

    /// <summary>
    /// Get all Staffs from Db by default. If optional parameter is false than get only those not approved.
    /// </summary>
    /// <param name="getAll">false returns only not approved Staff.</param>
    /// <returns>Collection of StaffFullModel</returns>
    public async Task<List<StaffFullModel>> GetAllStaff(bool getAll = true)
    {
        string sql = getAll ? "spStaffs_GetAll" : "spStaffs_GetAllNotApproved";

        List<StaffFullModel> output = await _db.LoadData<StaffFullModel, dynamic>(sql,
                                                                                  new { },
                                                                                  connectionStringName);

        foreach (StaffFullModel staff in output)
        {
            staff.PhoneNumbers = await _db.LoadData<PhoneNumberModel, dynamic>("spPhoneNumbers_GetByStaffId",
                                                                               new { staffId = staff.Id },
                                                                               connectionStringName);
        }

        return output;
    }

    /// <summary>
    /// Get Staff by Email from Db.
    /// </summary>
    /// <param name="emailAddress">Staff's email.</param>
    /// <returns>Staff info.</returns>
    public async Task<StaffFullModel> GetStaffByEmail(string emailAddress)
    {
        List<StaffFullModel> output = await _db.LoadData<StaffFullModel, dynamic>("spStaffs_GetByEmail",
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
    public async void UpdateStaffByAdmin(int id, int departmentId, bool isApproved)
    {
        await _db.SaveData("spStaffs_UpdateByAdmin",
                           new { id, departmentId, isApproved },
                           connectionStringName);
    }

    /// <summary>
    /// Should move into some helper class!
    /// Perform CheckOut if last record in CheckIns Table has CheckOut set to null,
    /// otherwise perform CheckIn.
    /// </summary>
    /// <param name="staffId">Staff's Id.</param>
    /// <exception cref="ArgumentException"></exception>
    private async void DoCheckInOrCheckOut(int staffId)
    {
        CheckInModel model = await GetLastCheckIn(staffId);

        if (model == null)
            throw new ArgumentException("You passed in an invalid parameter", "staffId");

        if (model.CheckOutDate == null)
            await CheckOutPerform(model.Id);
        else
            await CheckInPerform(staffId);
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
