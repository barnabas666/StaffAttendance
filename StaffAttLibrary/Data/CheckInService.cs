using StaffAttLibrary.Db;
using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Data
{
    /// <summary>
    /// Class servicing Check-Ins - CRUD actions.
    /// UIs (MVC, WPF) talk to this class. This class calls SqlDataAccess methods.
    /// </summary>
    public class CheckInService : ICheckInService
    {
        /// <summary>
        /// Servicing SQL database connection.
        /// </summary>
        private readonly ISqlDataAccess _db;
        private readonly ICheckInData _checkInData;

        /// <summary>
        /// Holds default connection string name.
        /// </summary>
        private const string connectionStringName = "Testing";

        /// <summary>
        /// Constructor, ISqlDataAccess comes from Dependency Injection from our frontend (UI).
        /// </summary>
        /// <param name="db">Servicing SQL database connection.</param>
        public CheckInService(ISqlDataAccess db, ICheckInData checkInData)
        {
            _db = db;
            _checkInData = checkInData;
        }

        /// <summary>
        /// Check if Staff is approved to do CheckIn/CheckOut
        /// </summary>
        /// <param name="aliasId">Staff's Alias info.</param>
        /// <returns>True if Staff is approved, false if not.</returns>
        public async Task<bool> CheckApproveStatus(int aliasId)
        {
            StaffBasicModel staffModel = await _checkInData.GetBasicStaffByAliasId(aliasId);           

            return staffModel.IsApproved;
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
            return await _db.SaveDataGetId("spCheckIns_InsertCheckIn",
                               new { staffId },
                               connectionStringName);             
        }

        /// <summary>
        /// Perform CheckOut for given CheckIn record with current date.
        /// </summary>
        /// <param name="checkInId">CheckIn's id.</param>
        /// <returns>CheckIn id.</returns>
        public async Task<int> CheckOutPerform(int checkInId)
        {
            return await _db.SaveDataGetId("spCheckIns_InsertCheckOut",
                               new { checkInId },
                               connectionStringName);             
        }

        /// <summary>
        /// Get all CheckIns from our database.
        /// </summary>
        /// <returns>Collection of CheckInModel.</returns>
        public async Task<List<CheckInFullModel>> GetAllCheckIns()
        {
            return await _db.LoadData<CheckInFullModel, dynamic>("spCheckIns_GetAll",
                                                                 new { },
                                                                 connectionStringName);
        }

        /// <summary>
        /// Get all CheckIns from our database by Date range.
        /// </summary>
        /// <param name="startDate">Start Date.</param>
        /// <param name="endDate">End Date.</param>
        /// <returns>Collection of CheckIn Info.</returns>
        public async Task<List<CheckInFullModel>> GetAllCheckInsByDate(DateTime startDate, DateTime endDate)
        {
            return await _db.LoadData<CheckInFullModel, dynamic>("spCheckIns_GetAllByDate",
                                                                                new { startDate, endDate },
                                                                                connectionStringName);
        }

        /// <summary>
        /// Get CheckIns by Email from Db.
        /// </summary>
        /// <param name="emailAddress">Staff's EmailAddress.</param>
        /// <returns>CheckIn info.</returns>
        public async Task<List<CheckInFullModel>> GetCheckInsByEmail(string emailAddress)
        {
            return await _db.LoadData<CheckInFullModel, dynamic>("spCheckIns_GetByEmail",
                                                                                new { emailAddress },
                                                                                connectionStringName);
        }

        /// <summary>
        /// Get CheckIns by Date and Email from Db.
        /// </summary>
        /// <param name="emailAddress">Staff's EmailAddress.</param>
        /// <param name="startDate">Start Date for CheckIns range.</param>
        /// <param name="endDate">End Date for CheckIns range.</param>
        /// <returns>CheckIn info.</returns>
        public async Task<List<CheckInFullModel>> GetCheckInsByDateAndEmail(string emailAddress,
                                                                            DateTime startDate,
                                                                            DateTime endDate)
        {
            return await _db.LoadData<CheckInFullModel, dynamic>("spCheckIns_GetByDateAndEmail",
                                                                                new { emailAddress, startDate, endDate },
                                                                                connectionStringName);
        }

        /// <summary>
        /// Get CheckIns by Date and Id from Db.
        /// </summary>
        /// <param name="id">Staff's Id.</param>
        /// <param name="startDate">Start Date for CheckIns range.</param>
        /// <param name="endDate">End Date for CheckIns range.</param>
        /// <returns>CheckIn info.</returns>
        public async Task<List<CheckInFullModel>> GetCheckInsByDateAndId(int id,
                                                                            DateTime startDate,
                                                                            DateTime endDate)
        {
            return await _db.LoadData<CheckInFullModel, dynamic>("spCheckIns_GetByDateAndId",
                                                                                new { id, startDate, endDate },
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
    }
}
