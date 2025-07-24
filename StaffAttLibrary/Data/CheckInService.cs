using StaffAttLibrary.Db;
using StaffAttLibrary.Models;

namespace StaffAttLibrary.Data;

/// <summary>
/// Class servicing Check-Ins - CRUD actions.
/// UIs (MVC, WPF) talk to this class. This class calls CheckInData class and SqlDataAccess methods.
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
    private readonly string _connectionStringName;

    /// <summary>
    /// Constructor, ISqlDataAccess comes from Dependency Injection from our frontend (UI).
    /// </summary>
    /// <param name="db">Servicing SQL database connection.</param>
    public CheckInService(ISqlDataAccess db, ICheckInData checkInData, IConnectionStringData connectionStringData)
    {
        _db = db;
        _checkInData = checkInData;
        _connectionStringName = connectionStringData.SqlConnectionName;
    }

    /// <summary>
    /// Get last record from CheckIn Table.
    /// </summary>
    /// <param name="staffId">Staff's Id.</param>
    /// <returns>Last CheckIn (CheckOut prop can be null) info or null for invalid staffId.</returns>
    public async Task<CheckInModel> GetLastCheckInAsync(int staffId)
    {
        List<CheckInModel> output = await _db.LoadDataAsync<CheckInModel, dynamic>("spCheckIns_GetLastRecord",
                                                                  new { staffId },
                                                                  _connectionStringName);

        return output.FirstOrDefault();
    }

    /// <summary>
    /// Get all CheckIns from our database by Date range.
    /// </summary>
    /// <param name="startDate">Start Date.</param>
    /// <param name="endDate">End Date.</param>
    /// <returns>Collection of CheckIn Info.</returns>
    public async Task<List<CheckInFullModel>> GetAllCheckInsByDateAsync(DateTime startDate, DateTime endDate)
    {
        return await _db.LoadDataAsync<CheckInFullModel, dynamic>("spCheckIns_GetAllByDate",
                                                                            new { startDate, endDate },
                                                                            _connectionStringName);
    }

    /// <summary>
    /// Get CheckIns by Date and Email from Db.
    /// </summary>
    /// <param name="emailAddress">Staff's EmailAddress.</param>
    /// <param name="startDate">Start Date for CheckIns range.</param>
    /// <param name="endDate">End Date for CheckIns range.</param>
    /// <returns>CheckIn info.</returns>
    public async Task<List<CheckInFullModel>> GetCheckInsByDateAndEmailAsync(string emailAddress,
                                                                        DateTime startDate,
                                                                        DateTime endDate)
    {
        return await _db.LoadDataAsync<CheckInFullModel, dynamic>("spCheckIns_GetByDateAndEmail",
                                                                            new { emailAddress, startDate, endDate },
                                                                            _connectionStringName);
    }

    /// <summary>
    /// Get CheckIns by Date and Id from Db.
    /// </summary>
    /// <param name="id">Staff's Id.</param>
    /// <param name="startDate">Start Date for CheckIns range.</param>
    /// <param name="endDate">End Date for CheckIns range.</param>
    /// <returns>CheckIn info.</returns>
    public async Task<List<CheckInFullModel>> GetCheckInsByDateAndIdAsync(int id,
                                                                        DateTime startDate,
                                                                        DateTime endDate)
    {
        return await _db.LoadDataAsync<CheckInFullModel, dynamic>("spCheckIns_GetByDateAndId",
                                                                            new { id, startDate, endDate },
                                                                            _connectionStringName);
    }

    /// <summary>
    /// Perform CheckOut if last record in CheckIns Table has CheckOut set to null,
    /// otherwise perform CheckIn.
    /// </summary>
    /// <param name="staffId">Staff's Id.</param>
    /// <exception cref="ArgumentException"></exception>
    public async Task DoCheckInOrCheckOutAsync(int staffId)
    {
        CheckInModel model = await GetLastCheckInAsync(staffId);

        if (model != null && model.CheckOutDate == null)
            await _checkInData.CheckOutPerformAsync(model.Id);
        else
            await _checkInData.CheckInPerformAsync(staffId);
    }
}
