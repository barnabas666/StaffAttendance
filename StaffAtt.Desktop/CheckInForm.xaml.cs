using StaffAtt.Desktop.Models;
using StaffAtt.Desktop.Helpers;
using StaffAttShared.DTOs;
using System.Windows;

namespace StaffAtt.Desktop;

/// <summary>
/// Staff's CheckIn/Out form.
/// </summary>
public partial class CheckInForm : Window
{
    private readonly IDesktopApiClient _api;
    private StaffBasicDto _staff = null!;
    private CheckInDto? _lastCheckIn = null;

    public CheckInForm(IDesktopApiClient api)
    {
        InitializeComponent();
        _api = api;
    }

    /// <summary>
    /// Called when the user successfully logs in from MainWindow.
    /// </summary>
    public async Task PopulateStaff(StaffBasicDto staff)
    {
        _staff = staff;

        firstNameText.Text = staff.FirstName;
        lastNameText.Text = staff.LastName;
        emailAddressText.Text = staff.EmailAddress;
        departmentTitleText.Text = staff.Title;

        await LoadLastCheckInAsync();

        checkInButton.Content = IsNextCheckIn() ? "Check-In" : "Check-Out";
    }

    private async Task LoadLastCheckInAsync()
    {
        var result = await _api.GetLastCheckInAsync(_staff.Id);

        if (!result.IsSuccess)
        {
            // Silent fail: we don't bother the user with last check-in errors
            _lastCheckIn = null;
            return;
        }

        _lastCheckIn = result.Value;
    }

    /// <summary>
    /// Check if we are doing CheckIn or CheckOut.
    /// For first CheckIn ever or if last record is CheckOut we return true.
    /// If last CheckIn has no CheckOut value than we return false.
    /// </summary>
    /// <returns></returns>
    private bool IsNextCheckIn()
    {
        // If null => first check in; If CheckOutDate != null => next is check in
        return _lastCheckIn == null || _lastCheckIn.CheckOutDate != null;
    }

    /// <summary>
    /// Perform CheckIn/Out and close this CheckInForm Window.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void CheckInButton_Click(object sender, RoutedEventArgs e)
    {
        var result = await _api.DoCheckInAsync(_staff.Id);

        if (!result.IsSuccess)
        {
            MessageBox.Show(
                result.ErrorMessage ?? "Unable to perform Check-In/Out.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        string action = IsNextCheckIn() ? "Check-In" : "Check-Out";

        MessageBox.Show(
            $"{action} successful.",
            "Success",
            MessageBoxButton.OK,
            MessageBoxImage.Information);

        this.Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
