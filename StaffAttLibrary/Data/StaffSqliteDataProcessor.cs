using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Data;
public class StaffSqliteDataProcessor : IStaffDataProcessor
{
    public Task<bool> CheckAliasAsync(string alias)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckPhoneNumberAsync(PhoneNumberModel phoneNumber)
    {
        throw new NotImplementedException();
    }

    public string CreateAlias(string firstName, string lastName, int orderNumber)
    {
        throw new NotImplementedException();
    }

    public Task DeletePhoneNumberAsync(int phoneNumberId)
    {
        throw new NotImplementedException();
    }

    public Task DeletePhoneNumberLinkAsync(int staffId, int phoneNumberId)
    {
        throw new NotImplementedException();
    }

    public Task<List<PhoneNumberModel>> GetPhoneNumberAsync(PhoneNumberModel phoneNumber)
    {
        throw new NotImplementedException();
    }

    public Task<List<StaffPhoneNumberModel>> GetPhoneNumberLinksAsync(int phoneNumberId)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveAliasAsync(string alias, string pIN)
    {
        throw new NotImplementedException();
    }

    public Task<int> SavePhoneNumberAsync(PhoneNumberModel phoneNumber)
    {
        throw new NotImplementedException();
    }

    public Task SavePhoneNumberLinkAsync(int staffId, int phoneNumberId)
    {
        throw new NotImplementedException();
    }
}
