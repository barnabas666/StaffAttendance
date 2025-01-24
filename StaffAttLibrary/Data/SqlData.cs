using StaffAttLibrary.Db;
using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Data;

/// <summary>
/// Class servicing Staffs - CRUD actions.
/// UIs (MVC, WPF) talk to this class. This class calls SqlDataAccess methods.
/// </summary>
public class SqlData
{
    /// <summary>
    /// Servicing SQL database connection.
    /// </summary>
    private readonly ISqlDataAccess _db;

    /// <summary>
    /// Holds default connection string name.
    /// </summary>
    private const string connectionStringName = "SqlDb";

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
