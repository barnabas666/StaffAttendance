using StaffAttLibrary.Data;
using StaffAttLibrary.Models;
using System.Windows;

namespace StaffAtt.Desktop;

/// <summary>
/// Staff's CheckIn/Out form.
/// </summary>
public partial class CheckInForm : Window
{
    /// <summary>
    /// Instance of class servicing Staffs - CRUD actions.
    /// </summary>
    private readonly IStaffService _staffService;
    private readonly ICheckInService _checkInService;

    /// <summary>
    /// Instance of class which holds Basic Staff data.
    /// </summary>
    private StaffBasicModel _basicStaffModel = null;

    /// <summary>
    /// Instance of class which holds CheckIn data.
    /// </summary>
    private CheckInModel _checkInModel = null;

    /// <summary>
    /// Constructor, initialize instance of this class.
    /// </summary>
    /// <param name="staffService">Instance of class servicing Staffs - CRUD actions.</param>
    public CheckInForm(IStaffService staffService, ICheckInService checkInService)
    {
        InitializeComponent();
        _staffService = staffService;
        _checkInService = checkInService;
    }

    /// <summary>
    /// Populate our form with data we get from Db - StaffBasicModel.
    /// Just info for Staff to be sure its really his/her own CheckIn/Out.
    /// </summary>
    /// <param name="basicStaffModel">Holds Basic Staff data.</param>
    public async void PopulateStaff(StaffBasicModel basicStaffModel)
    {
        _basicStaffModel = basicStaffModel;
        firstNameText.Text = _basicStaffModel.FirstName;
        lastNameText.Text = _basicStaffModel.LastName;
        emailAddressText.Text = _basicStaffModel.EmailAddress;
        departmentTitleText.Text = _basicStaffModel.Title;

        try
        {
            _checkInModel = await _checkInService.GetLastCheckInAsync(_basicStaffModel.Id);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return;
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
            await _checkInService.DoCheckInOrCheckOutAsync(_basicStaffModel.Id);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
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
