﻿using StaffAttLibrary.Enums;
using StaffAttLibrary.Models;

namespace StaffAttLibrary.Data;
public interface IDatabaseData
{
    Task<AliasModel> AliasVerification(string alias, string pIN);
    Task<bool> CheckApproveStatus(int aliasId);
    Task<int> CheckInPerform(int staffId);
    Task<int> CheckOutPerform(int checkInId);
    Task CreateStaff(int departmentId,
                     AddressModel address,
                     string pIN,
                     string firstName,
                     string lastName,
                     string emailAddress,
                     List<PhoneNumberModel> phoneNumbers);
    Task UpdateStaff(AddressModel address,
                     string pIN,
                     string firstName,
                     string lastName,
                     string emailAddress,
                     List<PhoneNumberModel> phoneNumbers);
    Task<List<DepartmentModel>> GetAllDepartments();
    Task<List<StaffFullModel>> GetAllStaff(bool getAll = true);
    Task<List<StaffBasicModel>> GetAllBasicStaffByDepartmentAndApproved(int departmentId, ApprovedType approvedType);
    Task<List<StaffBasicModel>> GetAllBasicStaff();
    Task<CheckInModel> GetLastCheckIn(int staffId);
    Task<StaffFullModel> GetStaffByEmail(string emailAddress);
    Task<StaffFullModel> GetStaffById(int id);
    Task<StaffBasicModel> GetBasicStaffById(int id);
    Task<StaffBasicModel> GetBasicStaffByAliasId(int aliasId);
    Task<bool> CheckStaffByEmail(string emailAddress);
    Task UpdateStaffByAdmin(int id, int departmentId, bool isApproved);
    Task DeleteStaff(int staffId);
    Task<List<CheckInFullModel>> GetAllCheckIns();
    Task<List<CheckInFullModel>> GetAllCheckInsByDate(DateTime startDate, DateTime endDate);
    Task<List<CheckInFullModel>> GetCheckInsByEmail(string emailAddress);
    Task<List<CheckInFullModel>> GetCheckInsByDateAndEmail(string emailAddress, DateTime startDate, DateTime endDate);
    Task<List<CheckInFullModel>> GetCheckInsByDateAndId(int id, DateTime startDate, DateTime endDate);
}