using StaffAttLibrary.Db;
using StaffAttLibrary.Enums;
using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Data;
public class StaffSqliteService : IStaffService
{
    private readonly ISqliteDataAccess _db;
    private readonly IStaffData _staffData;
    private readonly ICheckInData _checkInData;
    private readonly IConnectionStringData _connectionStringData;

    /// <summary>
    /// Holds default connection string name.
    /// </summary>
    private readonly string _connectionStringName;

    public StaffSqliteService(ISqliteDataAccess db, IStaffData staffData, ICheckInData checkInData, IConnectionStringData connectionStringData)
    {
        _db = db;
        _staffData = staffData;
        _checkInData = checkInData;
        _connectionStringData = connectionStringData;
        _connectionStringName = _connectionStringData.SQLiteConnectionName;
    }

    public Task<AliasModel> AliasVerificationAsync(string alias, string pIN)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckStaffByEmailAsync(string emailAddress)
    {
        throw new NotImplementedException();
    }

    public Task CreateStaffAsync(int departmentId, AddressModel address, string pIN, string firstName, string lastName, string emailAddress, List<PhoneNumberModel> phoneNumbers)
    {
        throw new NotImplementedException();
    }

    public Task DeleteStaffAsync(int staffId)
    {
        throw new NotImplementedException();
    }

    public Task<List<StaffBasicModel>> GetAllBasicStaffAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<StaffBasicModel>> GetAllBasicStaffFilteredAsync(int departmentId, ApprovedType approvedType)
    {
        throw new NotImplementedException();
    }

    public Task<List<DepartmentModel>> GetAllDepartmentsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<StaffBasicModel> GetBasicStaffByAliasIdAsync(int aliasId)
    {
        throw new NotImplementedException();
    }

    public Task<StaffBasicModel> GetBasicStaffByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<StaffFullModel> GetStaffByEmailAsync(string emailAddress)
    {
        throw new NotImplementedException();
    }

    public Task<StaffFullModel> GetStaffByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetStaffEmailByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateStaffAsync(AddressModel address, string pIN, string firstName, string lastName, string emailAddress, List<PhoneNumberModel> phoneNumbers)
    {
        throw new NotImplementedException();
    }

    public Task UpdateStaffByAdminAsync(int id, int departmentId, bool isApproved)
    {
        throw new NotImplementedException();
    }
}
