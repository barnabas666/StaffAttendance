using Microsoft.Extensions.DependencyInjection;
using StaffAtt.Desktop.Models;
using StaffAttLibrary.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Windows;

namespace StaffAtt.Desktop;

/// <summary>
/// Main Staff's Login form.
/// </summary>
public partial class MainWindow : Window
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
    /// Constructor, initialize instance of this class.
    /// </summary>
    /// <param name="httpClientFactory"></param>
    /// <param name="tokenModel"></param>
    public MainWindow(IHttpClientFactory httpClientFactory, TokenModel tokenModel)
    {
        InitializeComponent();        
        _tokenModel = tokenModel;
        httpClient = httpClientFactory.CreateClient("api");
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
        AuthenticationModel login = new();
        StaffBasicModel staff = new();        

        if (string.IsNullOrWhiteSpace(aliasText.Text) || string.IsNullOrWhiteSpace(pINText.Text))
        {
            MessageBox.Show("You must enter Alias and PIN.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        login.Alias = aliasText.Text;
        login.PIN = pINText.Text;

        try
        {
            // 1. Authenticate and get token
            var response = await httpClient.PostAsJsonAsync<AuthenticationModel>("AuthenticationDesktop/token", login);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                string message = $"Login failed ({(int)response.StatusCode} {response.ReasonPhrase})";

                // Try to extract more info if the error is JSON
                if (response.Content.Headers.ContentType?.MediaType == "application/problem+json")
                {
                    message += $"\nDetails: {errorContent}";
                }

                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _tokenModel.Token = await response.Content.ReadAsStringAsync();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenModel.Token);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(_tokenModel.Token);
            var aliasId = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            // 2. Get StaffBasicModel from API
            var staffResponse = await httpClient.GetAsync($"staff/basic/alias/{aliasId}");
            if (!staffResponse.IsSuccessStatusCode)
            {
                var errorContent = await staffResponse.Content.ReadAsStringAsync();
                string message = $"Login failed ({(int)staffResponse.StatusCode} {staffResponse.ReasonPhrase})";
                // Try to extract more info if the error is JSON
                if (staffResponse.Content.Headers.ContentType?.MediaType == "application/problem+json")
                {
                    message += $"\nDetails: {errorContent}";
                }
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            staff = await staffResponse.Content.ReadFromJsonAsync<StaffBasicModel>();
                        
            if (staff.IsApproved)
            {
                CheckInForm checkInForm = App.serviceProvider.GetRequiredService<CheckInForm>();

                // we populate instance of CheckInForm window with data from our StaffBasicModel
                checkInForm.PopulateStaff(staff);
                checkInForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not approved to do CheckIn/Out.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
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