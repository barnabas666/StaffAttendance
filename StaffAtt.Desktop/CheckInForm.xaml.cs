using StaffAtt.Desktop.Models;
using StaffAttShared.DTOs;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;

namespace StaffAtt.Desktop;

/// <summary>
/// Staff's CheckIn/Out form.
/// </summary>
public partial class CheckInForm : Window
{
    /// <summary>
    /// Instance of class which holds JWT token.
    /// </summary>
    private readonly TokenModel _tokenModel;
    /// <summary>
    /// Instance of HttpClient to call our API.
    /// </summary>
    private HttpClient httpClient;

    /// <summary>
    /// Instance of class which holds Basic Staff data.
    /// </summary>
    private StaffBasicDto _basicStaffModel = null;
    /// <summary>
    /// Instance of class which holds CheckIn data.
    /// </summary>
    private CheckInDto _checkInModel = null;

    /// <summary>
    /// Constructor, initialize instance of this class.
    /// </summary>
    /// <param name="httpClientFactory"></param>
    /// <param name="tokenModel"></param>
    public CheckInForm(IHttpClientFactory httpClientFactory, TokenModel tokenModel)
    {
        InitializeComponent();
        _tokenModel = tokenModel;
        httpClient = httpClientFactory.CreateClient("api");
    }

    /// <summary>
    /// Populate our form with data we get from Db - StaffBasicModel.
    /// Just info for Staff to be sure its really his/her own CheckIn/Out.
    /// </summary>
    /// <param name="basicStaffModel">Holds Basic Staff data.</param>
    public async void PopulateStaff(StaffBasicDto basicStaffModel)
    {
        _basicStaffModel = basicStaffModel;
        firstNameText.Text = _basicStaffModel.FirstName;
        lastNameText.Text = _basicStaffModel.LastName;
        emailAddressText.Text = _basicStaffModel.EmailAddress;
        departmentTitleText.Text = _basicStaffModel.Title;

        try
        {            
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenModel.Token);

            var lastCheckInResponse = await httpClient.GetAsync($"checkin/last/{_basicStaffModel.Id}");
            if(!lastCheckInResponse.IsSuccessStatusCode)
            {
                var errorContent = await lastCheckInResponse.Content.ReadAsStringAsync();
                string message = $"Getting last CheckIn failed ({(int)lastCheckInResponse.StatusCode} {lastCheckInResponse.ReasonPhrase})";
                // Try to extract more info if the error is JSON
                if (lastCheckInResponse.Content.Headers.ContentType?.MediaType == "application/problem+json")
                {
                    message += $"\nDetails: {errorContent}";
                }
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            var content = await lastCheckInResponse.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content) || content == "null")
                _checkInModel = null;
            else
            {
                _checkInModel = JsonSerializer.Deserialize<CheckInDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
        catch (HttpRequestException ex)
        {
            MessageBox.Show("Network error: " + ex.Message);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        checkInButton.Content = IsNextCheckIn() ? "Check-In" : "Check-Out";
    }

    /// <summary>
    /// Check if we are doing CheckIn or CheckOut.
    /// For first CheckIn ever or if last record is CheckOut we return true.
    /// If last CheckIn has no CheckOut value than we return false.
    /// </summary>
    /// <returns>True for CheckIn, false for CheckOut.</returns>
    private bool IsNextCheckIn()
    {
        return _checkInModel == null || _checkInModel.CheckOutDate != null;
    }

    /// <summary>
    /// Perform CheckIn/Out and close this CheckInForm Window.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void CheckInButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var doCheckInResponse = await httpClient.PostAsync($"checkin/do/{_basicStaffModel.Id}", null);
            if (!doCheckInResponse.IsSuccessStatusCode)
            {
                var errorContent = await doCheckInResponse.Content.ReadAsStringAsync();
                string message = $"CheckIn/Out failed ({(int)doCheckInResponse.StatusCode} {doCheckInResponse.ReasonPhrase})";
                // Try to extract more info if the error is JSON
                if (doCheckInResponse.Content.Headers.ContentType?.MediaType == "application/problem+json")
                {
                    message += $"\nDetails: {errorContent}";
                }
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }            
        }
        catch (HttpRequestException ex)
        {
            MessageBox.Show("Network error: " + ex.Message);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        this.Close();
    }

    /// <summary>
    /// Close this CheckInForm Window.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
