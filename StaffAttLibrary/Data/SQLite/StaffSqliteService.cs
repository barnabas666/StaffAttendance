using StaffAttLibrary.Db.SQLite;
using StaffAttShared.Enums;
using StaffAttLibrary.Helpers;
using StaffAttLibrary.Models;

namespace StaffAttLibrary.Data.SQLite;
public class StaffSqliteService : IStaffService
{
    private readonly ISqliteDataAccess _db;
    private readonly IStaffData _staffData;
    private readonly ICheckInData _checkInData;

    /// <summary>
    /// Holds default connection string name.
    /// </summary>
    private readonly string _connectionStringName;

    public StaffSqliteService(ISqliteDataAccess db, IStaffData staffData, ICheckInData checkInData, IConnectionStringData connectionStringData)
    {
        _db = db;
        _staffData = staffData;
        _checkInData = checkInData;
        _connectionStringName = connectionStringData.SQLiteConnectionName;
    }

    /// <summary>
    /// Get all Departments from our database.
    /// </summary>
    /// <returns>Collection of DepartmentModel.</returns>
    public async Task<List<DepartmentModel>> GetAllDepartmentsAsync()
    {
        string sql = await QueryHelper.LoadSqliteQueryAsync("Departments_GetAll.sql");

        return await _db.LoadDataAsync<DepartmentModel, dynamic>(sql,
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
    /// Verify if entered credentials are correct. Both Alias and PIN are stored in uppercase.
    /// </summary>
    /// <param name="alias">Staff's Alias.</param>
    /// <param name="pIN">Staff's PIN.</param>
    /// <returns>Correct: Alias info. False: null</returns>
    public async Task<AliasModel> AliasVerificationAsync(string alias, string pIN)
    {
        string sql = await QueryHelper.LoadSqliteQueryAsync("Aliases_GetByAliasAndPIN.sql");

        List<AliasModel> output = await _db.LoadDataAsync<AliasModel, dynamic>(sql,
                                                                               new { alias = alias.ToUpper(), pIN = pIN.ToUpper() },
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
        string sql = await QueryHelper.LoadSqliteQueryAsync("Staffs_GetAllBasic.sql");

        return await _db.LoadDataAsync<StaffBasicModel, dynamic>(sql,
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
        string sql = await QueryHelper.LoadSqliteQueryAsync("Staffs_GetBasicById.sql");

        List<StaffBasicModel> output = await _db.LoadDataAsync<StaffBasicModel, dynamic>(sql,
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
        string sql = await QueryHelper.LoadSqliteQueryAsync("Staffs_GetBasicByAliasId.sql");

        List<StaffBasicModel> output = await _db.LoadDataAsync<StaffBasicModel, dynamic>(sql,
                                                                                         new { aliasId },
                                                                                         _connectionStringName);
        return output.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves the email address of a staff member based on their unique identifier.
    /// </summary>
    /// <remarks>This method queries the database asynchronously to fetch the email address associated with
    /// the specified staff ID. If no matching record is found, the method returns <see langword="null"/>.</remarks>
    /// <param name="id">The unique identifier of the staff member whose email address is to be retrieved. Must be a positive integer.</param>
    /// <returns>A <see cref="string"/> containing the email address of the staff member if found; otherwise, <see
    /// langword="null"/>.</returns>
    public async Task<string> GetStaffEmailByIdAsync(int id)
    {
        string sql = await QueryHelper.LoadSqliteQueryAsync("Staffs_GetEmailById.sql");

        List<string> output = await _db.LoadDataAsync<string, dynamic>(sql,
                                                                       new { id },
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
        string sql = await QueryHelper.LoadSqliteQueryAsync("Staffs_CheckByEmail.sql");

        List<bool> output = await _db.LoadDataAsync<bool, dynamic>(sql,
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
        string sql = await QueryHelper.LoadSqliteQueryAsync("Staffs_UpdateByAdmin.sql");

        await _db.SaveDataAsync(sql,
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
