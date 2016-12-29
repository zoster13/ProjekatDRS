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

            OnLoad();
        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 1;
            Employee employee = new Employee();

            using (EmployeeProxy proxy = new EmployeeProxy(binding, address))
            {
                 employee =  proxy.LogIn(emailBox.Text, passwordBox.Password);
            }

            if (employee != null)
            {
                tabControl.SelectedIndex = 1;
                LocalClientDatabase.Instance.CurrentEmployee = employee;
                displayName.Text = employee.Name;
                displayTeam.Text = "not implemented...";
                displayType.Text = employee.Type.ToString();
                displayEmail.Text = employee.Type.ToString();
            }
            else
            {
                errorLogInBox.Text = "Wrong e-mail or password.";
            }

            logInButton.IsEnabled = false;
            emailBox.Text = "";
            passwordBox.Password = "";
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
            if (emailBox.Text != "" && passwordBox.Password != "")
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
            if (emailBox.Text != "" && passwordBox.Password != "")
            {
                logInButton.IsEnabled = true;
            }
            else
            {
                logInButton.IsEnabled = false;
            }
        }

        private void OnLoad()
        {
            using (EmployeeProxy proxy = new EmployeeProxy(binding, address))
            {
                var employees = proxy.GetAllEmployees();

                //LocalClientDatabase.Instance.Employees = employees;
                foreach (var employee in employees)
                {
                    LocalClientDatabase.Instance.Employees.Add(employee);
                }
            }
        }
    }
}
