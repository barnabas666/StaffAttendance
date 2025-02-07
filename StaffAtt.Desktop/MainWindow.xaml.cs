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
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IDatabaseData _sqlData;

    public MainWindow(IDatabaseData sqlData)
    {
        InitializeComponent();
        _sqlData = sqlData;
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        AliasModel aliasModel = await _sqlData.AliasVerification(aliasText.Text, pINText.Text);

        if (aliasModel != null)
        {
            StaffBasicModel staffBasicModel = await _sqlData.GetBasicStaffByAliasId(aliasModel.Id);

            if (staffBasicModel.IsApproved)
            {       
                CheckInForm checkInForm = App.serviceProvider.GetRequiredService<CheckInForm>();

                // we populate instance of CheckInForm with data from our StaffBasicModel
                checkInForm.PopulateStaff(staffBasicModel);

                checkInForm.Show();

                aliasText.Text = "";
                pINText.Text = "";
            }
        }
        else
        {
            MessageBox.Show("You have entered incorrect login information.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}