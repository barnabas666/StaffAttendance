using Microsoft.Extensions.DependencyInjection;
using StaffAttLibrary.Data;
using StaffAttLibrary.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StaffAtt.Desktop;

/// <summary>
/// Main Staff's Login form.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Instance of class servicing Staffs - CRUD actions.
    /// </summary>
    private readonly IStaffService _staffService;

    /// <summary>
    /// Constructor, initialize instance of this class.
    /// </summary>
    /// <param name="staffService">Instance of class servicing Staffs - CRUD actions.</param>
    public MainWindow(IStaffService staffService)
    {
        InitializeComponent();
        _staffService = staffService;
    }

    /// <summary>
    /// Login Staff and open CheckIn/Out form.
    /// If Staff enter correct Alias and PIN and he is approved to perform CheckIn/Out than new
    /// CheckIn/Out window will open.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        AliasModel aliasModel = null;

        try
        {
            // If Alias and PIN are correct AliasModel is returned
            aliasModel = await _staffService.AliasVerificationAsync(aliasText.Text, pINText.Text);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return;
        }

        if (aliasModel == null)
        {
            MessageBox.Show("You have entered incorrect login information.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        StaffBasicModel staffBasicModel = new StaffBasicModel();
        try
        {
            staffBasicModel = await _staffService.GetBasicStaffByAliasIdAsync(aliasModel.Id);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        if (staffBasicModel.IsApproved)
        {
            CheckInForm checkInForm = App.serviceProvider.GetRequiredService<CheckInForm>();

            // we populate instance of CheckInForm window with data from our StaffBasicModel
            checkInForm.PopulateStaff(staffBasicModel);

            checkInForm.ShowDialog();

            ClearBoxes();
        }
        else
        {
            MessageBox.Show("You are not approved to do CheckIn/Out.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
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