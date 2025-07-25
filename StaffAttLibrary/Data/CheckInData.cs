﻿using StaffAttLibrary.Db;

namespace StaffAttLibrary.Data;

/// <summary>
/// Class servicing Check-Ins - CRUD actions.
/// CheckInService talks to this class. This class calls SqlDataAccess methods.
/// </summary>
public class CheckInData : ICheckInData
{
    private readonly ISqlDataAccess _db;
    /// <summary>
    /// Holds default connection string name.
    /// </summary>
    private readonly string _connectionStringName;

    public CheckInData(ISqlDataAccess db, IConnectionStringData connectionStringData)
    {
        _db = db;
        _connectionStringName = connectionStringData.SqlConnectionName;
    }

    /// <summary>
    /// Perform CheckIn for given Staff with current date.
    /// </summary>
    /// <param name="staffId">Staff's id.</param>
    /// <returns>CheckIn id.</returns>
    public async Task<int> CheckInPerformAsync(int staffId)
    {
        return await _db.SaveDataGetIdAsync("spCheckIns_InsertCheckIn",
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
        return await _db.SaveDataGetIdAsync("spCheckIns_InsertCheckOut",
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
        await _db.SaveDataAsync("spCheckIns_Delete", new { staffId }, _connectionStringName);
    }
}
