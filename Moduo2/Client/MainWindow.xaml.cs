using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using ClientCommon.Data;
using System;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string address = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress epAddress = new EndpointAddress(new Uri(address));
        private NetTcpBinding binding = new NetTcpBinding();
        public LocalClientDatabase database = null;
        EmployeeProxy proxy;

        public MainWindow()
        {
            InitializeComponent();
            database = new LocalClientDatabase();
            DataContext = database;

            proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());

            //workCeo.Visibility = Visibility.Hidden;
        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            proxy.LogIn(emailBox.Text, passwordBox.Password);
        }

        private void logOutButton_Click(object sender, RoutedEventArgs e)
        {
            proxy.LogOut(database.CurrentEmployee.Email);
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
            if (emailBox.Text == "" || passwordBox.Password == "")
            {
                logInButton.IsEnabled = false;
            }
            else
            {
                logInButton.IsEnabled = true;
            }
        }

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (emailBox.Text == "" || passwordBox.Password == "")
            {
                logInButton.IsEnabled = false;
            }
            else
            {
                logInButton.IsEnabled = true;
            }
        }

        public void OnLoad()
        {
            database.Employees.Clear();
            database.Teams.Clear();
            database.HiringCompanies.Clear();

            var employees = proxy.GetAllEmployees();

            foreach (var employee in employees)
            {
                database.Employees.Add(employee);
            }

            var teams = proxy.GetAllTeams();

            foreach (var team in teams)
            {
                database.Teams.Add(team);
            }

            var hiringCompanies = proxy.GetAllHiringCompanies();

            foreach (var hiringCompany in hiringCompanies)
            {
                database.HiringCompanies.Add(hiringCompany);
            }
        }


        public void LogInCallbackResult(Employee employee)
        {
            if (employee != null)
            {
                lock (database.Locker)
                {
                    database.Employees.Add(employee);
                }

                tabControl.SelectedIndex = 1;

                if (database.CurrentEmployee.Email == null)
                {
                    database.CurrentEmployee = employee;

                    displayName.Text = employee.Name + " " + employee.Surname;
                    displayTeam.Text = "not implemented...";
                    displayType.Text = employee.Type.ToString();
                    displayEmail.Text = employee.Email;

                    switch (database.CurrentEmployee.Type)
                    {
                        case EmployeeType.CEO:
                            workCeo.Visibility = Visibility.Visible;
                            break;
                    }

                    OnLoad();
                }
            }
            else
            {
                errorLogInBox.Text = "Wrong e-mail or password.";
            }

            logInButton.IsEnabled = false;
            emailBox.Text = "";
            passwordBox.Password = "";
        }

        public void LogOutCallbackResult(Employee employee)
        {
            foreach (Employee e in database.Employees)
            {
                if (e.Email.Equals(employee.Email))
                {
                    lock (database.Locker)
                    {
                        database.Employees.Remove(e);
                        break;
                    }
                }
            }

            if (employee.Email.Equals(database.CurrentEmployee.Email))
            {
                tabControl.SelectedIndex = 0;
                database = new LocalClientDatabase();
                DataContext = database;
                proxy.Abort();
                proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());
            }
        }
    }
}
