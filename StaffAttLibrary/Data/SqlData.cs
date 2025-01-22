using StaffAttLibrary.Db;
using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Data;
public class SqlData
{
    private readonly ISqlDataAccess _db;
    private const string connectionStringName = "SqlDb";

    public SqlData(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<List<DepartmentModel>> GetAllDepartments()
    {
        return await _db.LoadData<DepartmentModel, dynamic>("spDepartments_GetAll", new { }, connectionStringName);
    }

    public async void CreateStaff(int departmentId,
                            string street,
                            string city,
                            string zip,
                            string state,
                            string pIN,
                            string firstName,
                            string lastName,
                            string emailAddress)
    {
        int addressId = await _db.SaveDataGetId("spAddresses_Insert",
                                                new { street, city, zip, state },
                                                connectionStringName);

        int orderNumber = 0;
        string alias = "";
        int aliasId = 0;

        do
        {
            orderNumber++;
            alias = CreateAlias(firstName, lastName, orderNumber);
            aliasId = await _db.SaveDataGetId("spAliases_Insert",
                                              new { alias, pIN },
                                              connectionStringName);
        } while (aliasId == 0);

        int staffId = await _db.SaveDataGetId("spStaffs_Insert",
                                              new { departmentId, addressId, aliasId, firstName, lastName, emailAddress },
                                              connectionStringName);
    }

    /// <summary>
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
