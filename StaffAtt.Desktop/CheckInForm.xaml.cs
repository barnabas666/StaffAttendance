using StaffAttLibrary.Data;
using StaffAttLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StaffAtt.Desktop;

/// <summary>
/// Staff's CheckIn/Out form.
/// </summary>
public partial class CheckInForm : Window
{
    /// <summary>
    /// Instance of class servicing Staffs - CRUD actions.
    /// </summary>
    private readonly IStaffData _staffData;
    private readonly ICheckInData _checkInData;

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
    /// <param name="staffData">Instance of class servicing Staffs - CRUD actions.</param>
    public CheckInForm(IStaffData staffData, ICheckInData checkInData)
    {
        InitializeComponent();
        _staffData = staffData;
        _checkInData = checkInData;
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
            _checkInModel = await _checkInData.GetLastCheckIn(_basicStaffModel.Id);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        // For first CheckIn ever or if last record is CheckOut we setup Button's value for new record: Check-In
        if(_checkInModel == null || _checkInModel.CheckOutDate != null)
        {
            checkInButton.Content = "Check-In";
        }
        // If last CheckIn has no CheckOut value than we setup Button value to: Check-Out
        else
        {
            checkInButton.Content = "Check-Out";
        }
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
            // Button's value decide if we do Check-In or Check-Out
            if (checkInButton.Content == "Check-In")
            {
                await _checkInData.CheckInPerform(_basicStaffModel.Id);
            }
            else
            {
                await _checkInData.CheckOutPerform(_checkInModel.Id);
            }
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
