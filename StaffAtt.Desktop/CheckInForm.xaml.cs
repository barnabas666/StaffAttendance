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
/// Interaction logic for CheckInForm.xaml
/// </summary>
public partial class CheckInForm : Window
{
    private readonly IDatabaseData _sqlData;
    private StaffBasicModel _basicStaffModel = null;
    private CheckInModel _checkInModel = null;

    public CheckInForm(IDatabaseData sqlData)
    {
        InitializeComponent();
        _sqlData = sqlData;
    }

    public async void PopulateStaff(StaffBasicModel basicStaffModel)
    {
        _basicStaffModel = basicStaffModel;
        firstNameText.Text = _basicStaffModel.FirstName;
        lastNameText.Text = _basicStaffModel.LastName;
        emailAddressText.Text = _basicStaffModel.EmailAddress;
        departmentTitleText.Text = _basicStaffModel.Title;

        _checkInModel = await _sqlData.GetLastCheckIn(_basicStaffModel.Id);

        // For first CheckIn ever we setup Button value to Check-In
        if(_checkInModel == null || _checkInModel.CheckOutDate != null)
        {
            checkInButton.Content = "Check-In";
        }
        // If last CheckIn has no CheckOut value than we setup Button value to Check-Out
        else if(_checkInModel.CheckOutDate == null)
        {
            checkInButton.Content = "Check-Out";
        }
    }

    private async void CheckInButton_Click(object sender, RoutedEventArgs e)
    {
        if (checkInButton.Content == "Check-In")
        {
            await _sqlData.CheckInPerform(_basicStaffModel.Id);
        }
        else
        {
            await _sqlData.CheckOutPerform(_checkInModel.Id);
        }

        this.Close();
    }
}
