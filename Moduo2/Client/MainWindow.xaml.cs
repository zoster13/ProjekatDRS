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
        EmployeeProxy proxy;

        public MainWindow()
        {
            InitializeComponent();
            //database = new LocalClientDatabase();
            DataContext = LocalClientDatabase.Instance;

            proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());
        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            proxy.LogIn(emailBox.Text, passwordBox.Password);
        }

        private void logOutButton_Click(object sender, RoutedEventArgs e)
        {
            proxy.LogOut(LocalClientDatabase.Instance.CurrentEmployee.Email);
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

        public void LoadCommonData()
        {
            LocalClientDatabase.Instance.Employees.Clear();
            LocalClientDatabase.Instance.Teams.Clear();
            LocalClientDatabase.Instance.HiringCompanies.Clear();

            var employees = proxy.GetAllEmployees();

            foreach (var employee in employees)
            {
                if(employee.Type == EmployeeType.DEVELOPER)
                {
                    LocalClientDatabase.Instance.Developers.Add(employee);
                }

                LocalClientDatabase.Instance.Employees.Add(employee);
            }

            var teams = proxy.GetAllTeams();

            foreach (var team in teams)
            {
                LocalClientDatabase.Instance.Teams.Add(team);
            }

            //var hiringCompanies = proxy.GetAllHiringCompanies();

            //foreach (var hiringCompany in hiringCompanies)
            //{
            //    LocalClientDatabase.Instance.HiringCompanies.Add(hiringCompany);
            //}
        }


        public void LogInCallbackResult(Employee employee)
        {
            if (employee != null)
            {
                lock (LocalClientDatabase.Instance.Locker)
                {
                    LocalClientDatabase.Instance.Employees.Add(employee);
                }

                tabControl.SelectedIndex = 1;

                if (LocalClientDatabase.Instance.CurrentEmployee.Email == null)
                {
                    LocalClientDatabase.Instance.CurrentEmployee = employee;

                    displayName.Text = employee.Name + " " + employee.Surname;
                    displayTeam.Text = "not implemented...";
                    displayType.Text = employee.Type.ToString();
                    displayEmail.Text = employee.Email;

                    switch (LocalClientDatabase.Instance.CurrentEmployee.Type)
                    {
                        case EmployeeType.CEO:
                            CEOWorkspace();
                            break;
                        case EmployeeType.DEVELOPER:
                            DeveloperWorkspace();
                            break;
                        case EmployeeType.TEAMLEADER:
                            TeamLeaderWorkspace();
                            break;
                        case EmployeeType.SCRUMMASTER:
                            ScrumMasterWorkspace();
                            break;
                        case EmployeeType.HR:
                            HRWorkspace();
                            break;
                    }
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

        private void CEOWorkspace()
        {
            workCeo.Visibility = Visibility.Visible;
            LoadCommonData();
        }

        private void DeveloperWorkspace()
        {
            LoadCommonData();
        }
        private void TeamLeaderWorkspace()
        {
            LoadCommonData();
        }
        private void ScrumMasterWorkspace()
        {
            LoadCommonData();
        }
        private void HRWorkspace()
        {
            LoadCommonData();
        }

        public void LogOutCallbackResult(Employee employee)
        {
            foreach (Employee e in LocalClientDatabase.Instance.Employees)
            {
                if (e.Email.Equals(employee.Email))
                {
                    lock (LocalClientDatabase.Instance.Locker)
                    {
                        LocalClientDatabase.Instance.Employees.Remove(e);
                        break;
                    }
                }
            }

            if (employee.Email.Equals(LocalClientDatabase.Instance.CurrentEmployee.Email))
            {
                tabControl.SelectedIndex = 0;
                proxy.Abort();
                LocalClientDatabase.Instance = null;
                DataContext = LocalClientDatabase.Instance;
                proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());
            }
        }
    }
}
