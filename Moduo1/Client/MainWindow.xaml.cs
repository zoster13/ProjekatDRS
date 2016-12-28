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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //string employeeSvcEndpoint = "net.tcp://localhost:9999/EmployeeService";

        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 1;

            //pronaci u bazi podataka koji je to user da bi mogli da se ispisu svi podaci u home tab-u,
            //takodje i zbog provere tipa,ovde verovatno treba da se napravi da su visible objekti koji
            //su vidjivi samo za odredjeni tip
            //homeLabel1.Content=?;
            //homeLabel2.Content=?;
            //homeLabel3.Content=?;
            //homeLabel4.Content=?;
        }

        private void editPassword_Click(object sender, RoutedEventArgs e)
        {
            passBoxOldPass.IsEnabled = true;
            passBoxNewPass.IsEnabled = true;
            passBoxConfirmNewPass.IsEnabled = true;
        }

        private void editData_Click(object sender, RoutedEventArgs e)
        {

        }

        private void saveChanges_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void showOnlineEmployees_Click(object sender, RoutedEventArgs e)
        {

        }

        private void showPartnerCompanies_Click(object sender, RoutedEventArgs e)
        {

        }

        private void projectOverview_Click(object sender, RoutedEventArgs e)
        {

        }

        private void usernameTextChanged(object sender, TextChangedEventArgs e)
        {
            if (usernameBox.Text != "" && passwordBox.Password != "")
            {
                logInButton.IsEnabled = true;
            }
            else
            {
                logInButton.IsEnabled = false;
            }
        }

        private void passwordGotFocus(object sender, RoutedEventArgs e)
        {
            if (usernameBox.Text != "" && passwordBox.Password != "")
            {
                logInButton.IsEnabled = true;
            }
            else
            {
                logInButton.IsEnabled = false;
            }
        }
    }
}
