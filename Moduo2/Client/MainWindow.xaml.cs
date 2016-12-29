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

            tabControl.SelectedIndex = 0;

            
        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            Employee employee = new Employee();

            using (EmployeeProxy proxy = new EmployeeProxy(binding, address))
            {
                 employee =  proxy.LogIn(emailBox.Text, passwordBox.Password);
            }

            if (employee != null)
            {
                tabControl.SelectedIndex = 1;
                LocalClientDatabase.Instance.CurrentEmployee = employee;
                displayName.Text = employee.Name + " " + employee.Surname;
                displayTeam.Text = "not implemented...";
                displayType.Text = employee.Type.ToString();
                displayEmail.Text = employee.Email;

                OnLoad();
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
            if (emailBox.Text != "" || passwordBox.Password != "")
            {
                //logInButton.IsEnabled = false;
            }
            else
            {
                logInButton.IsEnabled = true;
            }
        }

        private void passwordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (emailBox.Text != "" || passwordBox.Password != "")
            {
                //logInButton.IsEnabled = false;
            }
            else
            {
                logInButton.IsEnabled = true;
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

        private void logOutButton_Click(object sender, RoutedEventArgs e)
        {
            using (EmployeeProxy proxy = new EmployeeProxy(binding, address))
            {
                proxy.LogOut(LocalClientDatabase.Instance.CurrentEmployee.Email);
            }
            tabControl.SelectedIndex = 0;
        }
    }
}
