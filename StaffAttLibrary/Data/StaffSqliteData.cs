using StaffAttLibrary.Enums;
using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Data;
public class StaffSqliteData : IStaffData
{
    public Task<int> CreateAliasAsync(string pIN, string firstName, string lastName)
    {
        throw new NotImplementedException();
    }

    public Task CreatePhoneNumbersAsync(int staffId, List<PhoneNumberModel> phoneNumbers)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAddressAsync(int addressId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAliasAsync(int aliasId)
    {
        throw new NotImplementedException();
    }

    public Task DeletePhoneNumbersAsync(int staffId, List<PhoneNumberModel> phoneNumbers)
    {
        throw new NotImplementedException();
    }

    public Task DeleteStaffAsync(int staffId)
    {
        throw new NotImplementedException();
    }

    public Task<AddressModel> GetAddressByEmailAsync(string emailAddress)
    {
        throw new NotImplementedException();
    }

    public Task<AddressModel> GetAddressByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<StaffBasicModel>> GetAllBasicStaffByDepartmentAndApprovedAsync(int departmentId, ApprovedType approvedType)
    {
        throw new NotImplementedException();
    }

    public Task<List<StaffBasicModel>> GetAllBasicStaffByDepartmentAsync(int departmentId)
    {
        throw new NotImplementedException();
    }

    public Task<List<PhoneNumberModel>> GetPhoneNumbersByStaffIdAsync(int staffId)
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

    public Task<int> SaveAddressAsync(AddressModel address)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveStaffAsync(int departmentId, string firstName, string lastName, string emailAddress, int addressId, int aliasId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAddressAsync(AddressModel address, int addressId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAliasAsync(string pIN, int aliasId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateStaffAsync(string firstName, string lastName, int staffId)
    {
        throw new NotImplementedException();
    }
}
