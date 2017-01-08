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

        public MainWindow()
        {
            InitializeComponent();
            //database = new LocalClientDatabase();
            DataContext = LocalClientDatabase.Instance;
        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            LocalClientDatabase.Instance.proxy.LogIn(emailBox.Text, passwordBox.Password);
        }

        private void logOutButton_Click(object sender, RoutedEventArgs e)
        {
            LocalClientDatabase.Instance.proxy.LogOut(LocalClientDatabase.Instance.CurrentEmployee.Email);
        }

        private void editPassword_Click(object sender, RoutedEventArgs e)
        {
            oldPasswordLabel.IsEnabled = true;
            newPasswordLabel.IsEnabled = true;
            passBoxOldPass.IsEnabled = true;
            passBoxNewPass.IsEnabled = true;
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

            var employees = LocalClientDatabase.Instance.proxy.GetAllEmployees();

            foreach (var employee in employees)
            {
                if (employee.Type == EmployeeType.DEVELOPER)
                {
                    LocalClientDatabase.Instance.Developers.Add(employee);
                }

                LocalClientDatabase.Instance.Employees.Add(employee);
            }

            var teams = LocalClientDatabase.Instance.proxy.GetAllTeams();

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
                    displayTeam.Text = employee.TeamName;
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

        public void CEOWorkspace()
        {
            workCeo.Visibility = Visibility.Visible;
            tabControl1.SelectedIndex = 0;
            LoadCommonData();
            SetSettings();
            ResetWork();
        }

        public void HRWorkspace()
        {
            tabControl1.SelectedIndex = 0;
            workCeo.Visibility = Visibility.Visible;
            workCeo.tabItemProjects.Visibility = Visibility.Hidden;
            LoadCommonData();
            SetSettings();
            ResetWork();
        }


        public void DeveloperWorkspace()
        {
            tabControl1.SelectedIndex = 0;
            workDevLeader.Visibility = Visibility.Visible;
            workDevLeader.gridAddUserStory.Visibility = Visibility.Hidden;
            workDevLeader.gridAddTask.Visibility = Visibility.Hidden;
            LoadCommonData();
            SetSettings();
            ResetWork();
        }
        public void TeamLeaderWorkspace()
        {
            tabControl1.SelectedIndex = 0;
            workDevLeader.Visibility = Visibility.Visible;
            workDevLeader.gridAddUserStory.Visibility = Visibility.Visible;
            workDevLeader.gridAddTask.Visibility = Visibility.Visible;
            LoadCommonData();
            SetSettings();
            ResetWork();
        }

        public void ScrumMasterWorkspace()
        {
            tabControl1.SelectedIndex = 0;
            LoadCommonData();
            SetSettings();
            ResetWork();
        }

        private void SetSettings()
        {
            textBoxEditName.Text = LocalClientDatabase.Instance.CurrentEmployee.Name;
            textBoxEditSurname.Text = LocalClientDatabase.Instance.CurrentEmployee.Surname;
            textBoxEditFromTimeHours.Text = LocalClientDatabase.Instance.CurrentEmployee.WorkingHoursStart.Hour.ToString();
            textBoxEditFromTimeMinutes.Text = LocalClientDatabase.Instance.CurrentEmployee.WorkingHoursStart.Minute.ToString();
            textBoxEditToTimeHours.Text = LocalClientDatabase.Instance.CurrentEmployee.WorkingHoursEnd.Hour.ToString();
            textBoxEditToTimeMinutes.Text = LocalClientDatabase.Instance.CurrentEmployee.WorkingHoursEnd.Minute.ToString();

            passBoxOldPass.Password = "";
            passBoxNewPass.Password = "";
            passBoxOldPass.IsEnabled = false;
            passBoxNewPass.IsEnabled = false;
        }

        private void ResetWork()
        {
            //Add team
            workCeo.textBoxTeamName.Text = "";

            workCeo.textBoxLeaderName.Text = "";
            workCeo.textBoxLeaderSurname.Text = "";
            workCeo.textBoxLeaderEmail.Text = "";
            workCeo.textBoxTeamName.Text = "";
            workCeo.passwordBoxLeader.Password = "";

            //Add employee


            //Other Employee work
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
                LocalClientDatabase.Instance.proxy.Abort();
                LocalClientDatabase.NullifyInstance();
                DataContext = LocalClientDatabase.Instance;

                // sve se postavlja da bude nevidljivo
                workCeo.Visibility = Visibility.Visible; // kao i svi ostali
                workDevLeader.Visibility = Visibility.Hidden;
                workDevLeader.gridAddUserStory.Visibility = Visibility.Hidden;
                workDevLeader.gridAddTask.Visibility = Visibility.Hidden;
            }
        }

        public void TeamAddedCallbackResult(Team team)
        {
            LocalClientDatabase.Instance.Teams.Add(team);
        }

        private void cancelChanges_Click(object sender, RoutedEventArgs e)
        {
            SetSettings();
        }

        private void saveChanges_Click(object sender, RoutedEventArgs e)
        {
            LocalClientDatabase.Instance.CurrentEmployee.Name = textBoxEditName.Text;
            LocalClientDatabase.Instance.CurrentEmployee.Surname = textBoxEditSurname.Text;

            LocalClientDatabase.Instance.CurrentEmployee.WorkingHoursStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(textBoxEditFromTimeHours.Text), int.Parse(textBoxEditFromTimeMinutes.Text), DateTime.Now.Second);
            LocalClientDatabase.Instance.CurrentEmployee.WorkingHoursEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(textBoxEditToTimeHours.Text), int.Parse(textBoxEditToTimeMinutes.Text), DateTime.Now.Second);

            if (passBoxNewPass.Password != "" && passBoxOldPass.Password != "")       //mora se i stari unijeti
            {
                LocalClientDatabase.Instance.CurrentEmployee.Password = passBoxNewPass.Password;
                LocalClientDatabase.Instance.CurrentEmployee.PasswordTimeStamp = DateTime.Now;
            }

            SetSettings();

            LocalClientDatabase.Instance.proxy.EditEmployeeData(LocalClientDatabase.Instance.CurrentEmployee);
        }

        private void buttonNotifDetail_Click(object sender, RoutedEventArgs e)
        {
            if(dataGridNotifications.SelectedItem != null)
            {
                MakeNotifInvisible();

                Notification notification = (Notification)dataGridNotifications.SelectedItem;
                LocalClientDatabase.Instance.CurrentNotification = notification;

                switch(notification.Type)
                {
                    
                }
            }
        }

        private void MakeNotifInvisible()
        {
            acceptCanvas.Visibility = Visibility.Hidden;
        }

        public void TypeChangeCallbackResult(Team team, EmployeeType newType)
        {
            string message = string.Format("Your superior has changed your position from {0} to {1}. Your team is {2}.", LocalClientDatabase.Instance.CurrentEmployee.Type, newType, team.Name);

            LocalClientDatabase.Instance.Teams.Clear();
            LocalClientDatabase.Instance.Teams.Add(team);
            LocalClientDatabase.Instance.CurrentEmployee.TeamName = team.Name;

            MessageBox.Show(message, "Type Change", MessageBoxButton.OK);

            switch (newType)
            {
                case EmployeeType.DEVELOPER:
                    DeveloperWorkspace();
                    break;
                case EmployeeType.TEAMLEADER:
                    TeamLeaderWorkspace();
                    break;
                case EmployeeType.HR:
                    HRWorkspace();
                    break;
                case EmployeeType.CEO:
                    CEOWorkspace();
                    break;
                case EmployeeType.SCRUMMASTER:
                    ScrumMasterWorkspace();
                    break;
            }
        }

        public void EditEmployeeDataCalbackResult(Employee employee)
        {
            foreach (var emp in LocalClientDatabase.Instance.Employees)
            {
                if(emp.Email == employee.Email)
                {
                    LocalClientDatabase.Instance.Employees.Remove(emp);
                    break;
                }
            }

            LocalClientDatabase.Instance.Employees.Add(employee);
        }
    }
}
