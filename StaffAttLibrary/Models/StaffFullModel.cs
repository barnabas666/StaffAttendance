using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Models;
public class StaffFullModel
{
    public int Id { get; set; }
    public int DepartmentId { get; set; }
    public int AddressId { get; set; }
    public int AliasId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public bool IsApproved { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string Zip { get; set; }
    public string State { get; set; }
    public string Alias { get; set; }
    public List<PhoneNumberModel> PhoneNumbers { get; set; } = new List<PhoneNumberModel>();
}
