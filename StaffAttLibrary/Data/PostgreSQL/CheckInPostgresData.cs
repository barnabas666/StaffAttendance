using StaffAttLibrary.Db.PostgreSQL;
using StaffAttLibrary.Helpers;

namespace StaffAttLibrary.Data.PostgreSQL;
public class CheckInPostgresData : ICheckInData
{
    private readonly IPostgresDataAccess _db;
    /// <summary>
    /// Holds default connection string name.
    /// </summary>
    private readonly string _connectionStringName;

    public CheckInPostgresData(IPostgresDataAccess db, IConnectionStringData connectionStringData)
    {
        _db = db;
        _connectionStringName = connectionStringData.PostgresConnectionName;
    }

    /// <summary>
    /// Perform CheckIn for given Staff with current date.
    /// </summary>
    /// <param name="staffId">Staff's id.</param>
    /// <returns>CheckIn id.</returns>
    public async Task<int> CheckInPerformAsync(int staffId)
    {
        string sql = await QueryHelper.LoadPostgresQueryAsync("CheckIns_InsertCheckIn.sql");
        return await _db.SaveDataGetIdAsync(sql,
                                            new { staffId },
                                            _connectionStringName);
    }

    /// <summary>
    /// Perform CheckOut for given CheckIn record with current date.
    /// </summary>
    /// <param name="checkInId">CheckIn's id.</param>
    /// <returns>CheckOut id.</returns>
    public async Task<int> CheckOutPerformAsync(int checkInId)
    {
        string sql = await QueryHelper.LoadPostgresQueryAsync("CheckIns_InsertCheckOut.sql");
        return await _db.SaveDataGetIdAsync(sql,
                                            new { checkInId },
                                            _connectionStringName);
    }

    /// <summary>
    /// Deletes a check-in record associated with the specified staff ID.
    /// </summary>
    /// <remarks>This method performs an asynchronous operation to delete a check-in record from the database.
    /// Ensure that the provided <paramref name="staffId"/> corresponds to an existing record.</remarks>
    /// <param name="staffId">The unique identifier of the staff whose check-in record is to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DeleteCheckInAsync(int staffId)
    {
        string sql = await QueryHelper.LoadPostgresQueryAsync("CheckIns_Delete.sql");
        await _db.SaveDataAsync(sql, new { staffId }, _connectionStringName);
    }
}
