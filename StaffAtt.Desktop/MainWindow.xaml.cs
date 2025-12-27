using Microsoft.Extensions.DependencyInjection;
using StaffAtt.Desktop.Helpers;
using System.Windows;

namespace StaffAtt.Desktop;

/// <summary>
/// Main Staff's Login form.
/// </summary>
public partial class MainWindow : Window
{
    private readonly IDesktopApiClient _api;

    public MainWindow(IDesktopApiClient apiClient)
    {
        InitializeComponent();
        _api = apiClient;
    }


    /// <summary>
    /// Login Staff and open CheckIn/Out form.
    /// If Staff enter correct Alias and PIN and if is approved to perform CheckIn/Out than new
    /// CheckIn/Out window will open.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(aliasText.Text) ||
            string.IsNullOrWhiteSpace(pINText.Text))
        {
            MessageBox.Show("You must enter Alias and PIN.", "Warning",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            // 1) Authenticate
            var loginResult = await _api.LoginAsync(aliasText.Text, pINText.Text);

            if (!loginResult.IsSuccess || loginResult.Value is null)
            {
                MessageBox.Show(
                    loginResult.ErrorMessage ?? "Invalid alias or PIN.",
                    "Login Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var loginData = loginResult.Value;

            // 2) Get staff info
            var staffResult = await _api.GetStaffByAliasIdAsync(loginData.AliasId);

            if (!staffResult.IsSuccess || staffResult.Value is null)
            {
                MessageBox.Show(
                    staffResult.ErrorMessage ?? "Unable to load staff information.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            var staff = staffResult.Value;

            if (!staff.IsApproved)
            {
                MessageBox.Show(
                    "You are not approved to do CheckIn/Out.",
                    "Access Denied",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                ClearBoxes();
                return;
            }

            // 3) Open CheckIn window
            var checkInForm = App.serviceProvider.GetRequiredService<CheckInForm>();
            // we populate instance of CheckInForm window with data from our StaffBasicDto
            await checkInForm.PopulateStaff(staff);
            checkInForm.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Unexpected error: " + ex.Message,
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        ClearBoxes();
    }

    /// <summary>
    /// Clear Alias and PIN boxes.
    /// </summary>
    private void ClearBoxes()
    {
        aliasText.Text = "";
        pINText.Text = "";
    }
}