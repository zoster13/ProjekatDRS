using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
using ClientCommon.Data;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string address = "net.tcp://localhost:9999/EmployeeService";
        private NetTcpBinding binding = new NetTcpBinding();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = LocalClientDatabase.Instance;

            LocalClientDatabase.Instance.Employees.Add(new Employee() { Name = "aleksandra", Surname = "misic", Type = EmployeeType.TEAMLEADER, Email = "...@...com"});
        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 1;

            using (EmployeeProxy proxy = new EmployeeProxy(binding, address))
            {
                proxy.LogIn(usernameBox.Text, passwordBox.Password);
            }
        }

        private void editPassword_Click(object sender, RoutedEventArgs e)
        {
            oldPasswordLabel.IsEnabled = true;
            newPasswordLabel.IsEnabled = true;
            confirmNewPasswordLabel.IsEnabled = true;
            passBoxOldPass.IsEnabled = true;
            passBoxNewPass.IsEnabled = true;
            passBoxConfirmNewPass.IsEnabled = true;
        }

        private void usernameBox_TextChanged(object sender, TextChangedEventArgs e)
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

        private void passwordBox_GotFocus(object sender, RoutedEventArgs e)
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //LocalClientDatabase.Instance.Employees = proxy
        }

    }
}
