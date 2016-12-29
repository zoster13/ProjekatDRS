using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
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
                DataContext = LocalClientDatabase.Instance;

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

            logInButton.IsEnabled = true;
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
            LocalClientDatabase.Instance.Employees.Clear();

            using (EmployeeProxy proxy = new EmployeeProxy(binding, address))
            {
                var employees = proxy.GetAllEmployees();

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
