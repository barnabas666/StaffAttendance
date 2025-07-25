﻿namespace StaffAttLibrary.Data;

public interface ICheckInData
{
    Task<int> CheckInPerformAsync(int staffId);
    Task<int> CheckOutPerformAsync(int checkInId);
    Task DeleteCheckInAsync(int staffId);
}
