﻿using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using ClientCommon.Data;
using System;
using System.Windows.Media;
using ICommon;
using System.Collections.Generic;
using System.Linq;

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
            DataContext = LocalClientDatabase.Instance;

            workCeo.Visibility = Visibility.Hidden; // kao i svi ostali
            workLeader.Visibility = Visibility.Hidden;
            workDev.Visibility = Visibility.Hidden;
        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            LocalClientDatabase.Instance.proxy.LogIn(emailBox.Text, passwordBox.Password);
        }

        public void LogInCallbackResult(Employee employee, bool loggedIn)
        {
            if (loggedIn)
            {
                    lock (LocalClientDatabase.Instance.Locker)
                    {
                        LocalClientDatabase.Instance.Employees.Add(employee);
                    }

                    tabControl.SelectedIndex = 1;

                    if (LocalClientDatabase.Instance.CurrentEmployee.Email.Equals(string.Empty))
                    {
                        LocalClientDatabase.Instance.CurrentEmployee = employee;

                        foreach (var notif in employee.Notifications)
                        {
                            LocalClientDatabase.Instance.Notifications.Add(notif);
                        }

                        logInButton.IsEnabled = false;
                        emailBox.Text = "";
                        passwordBox.Password = "";

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

                logInButton.IsEnabled = false;
                emailBox.Text = "";
                passwordBox.Password = "";
            }
        }

        public void CEOWorkspace()
        {
            workCeo.Visibility = Visibility.Visible;
            tabControl1.SelectedIndex = 0;
            LoadCommonData();
            SetSettings();
            ResetWork();
            SetHome();
            MakeNotifInvisible();
            CountNewNotifications();
        }

        public void HRWorkspace()
        {
            tabControl1.SelectedIndex = 0;
            workCeo.Visibility = Visibility.Visible;
            workCeo.tabItemProjects.Visibility = Visibility.Hidden;
            LoadCommonData();
            SetSettings();
            ResetWork();
            SetHome();
            MakeNotifInvisible();
            CountNewNotifications();
        }


        public void DeveloperWorkspace()
        {
            tabControl1.SelectedIndex = 0;
            workDev.Visibility = Visibility.Visible;
            LoadCommonData();
            SetSettings();
            ResetWork();
            SetHome();
            MakeNotifInvisible();
            CountNewNotifications();
        }
        public void TeamLeaderWorkspace()
        {
            tabControl1.SelectedIndex = 0;
            workLeader.Visibility = Visibility.Visible;
            LoadCommonData();
            SetSettings();
            ResetWork();
            SetHome();
            MakeNotifInvisible();
            CountNewNotifications();
        }

        public void ScrumMasterWorkspace()
        {
            tabControl1.SelectedIndex = 0;
            LoadCommonData();
            SetSettings();
            ResetWork();
            SetHome();
            MakeNotifInvisible();
            CountNewNotifications();
        }
        public void LoadCommonData()
        {
            LocalClientDatabase.Instance.Employees.Clear();
            LocalClientDatabase.Instance.Teams.Clear();
            LocalClientDatabase.Instance.HiringCompanies.Clear();
            LocalClientDatabase.Instance.AllProjects.Clear();
            LocalClientDatabase.Instance.UserStories.Clear();
            LocalClientDatabase.Instance.AllTasks.Clear();
            LocalClientDatabase.Instance.MyTasks.Clear();
            LocalClientDatabase.Instance.Developers.Clear();

            var onlineEmployees = LocalClientDatabase.Instance.proxy.GetAllOnlineEmployees();
            var allEmployees = LocalClientDatabase.Instance.proxy.GetAllEmployees();
            var teams = LocalClientDatabase.Instance.proxy.GetAllTeams();
            var hiringCompanies = LocalClientDatabase.Instance.proxy.GetAllHiringCompanies();
            var allProjects = LocalClientDatabase.Instance.proxy.GetAllProjects();

            var myUserStories = LocalClientDatabase.Instance.proxy.GetUserStories(); ;
            var allTasks = LocalClientDatabase.Instance.proxy.GetAllTasks();

            foreach (var employee in allEmployees)
            {
                if (employee.Type == EmployeeType.DEVELOPER)
                {
                    LocalClientDatabase.Instance.Developers.Add(employee);
                }
            }

            foreach (var employee in onlineEmployees)
            {
                LocalClientDatabase.Instance.Employees.Add(employee);
            }

            foreach (var team in teams)
            {
                LocalClientDatabase.Instance.Teams.Add(team);
            }

            foreach (var hiringCompany in hiringCompanies)
            {
                LocalClientDatabase.Instance.HiringCompanies.Add(hiringCompany);
            }

            foreach (var proj in allProjects)
            {
                LocalClientDatabase.Instance.AllProjects.Add(proj);

                if (proj.Team.Name.Equals(LocalClientDatabase.Instance.CurrentEmployee.Team.Name))
                {
                    if (LocalClientDatabase.Instance.CurrentEmployee.Type == EmployeeType.DEVELOPER)
                    {
                        if (proj.ProgressStatus == ProgressStatus.STARTED)
                        {
                            LocalClientDatabase.Instance.MyTeamProjects.Add(proj);
                        }
                    }
                    else
                    {
                        LocalClientDatabase.Instance.MyTeamProjects.Add(proj);
                    }
                }
            }

            foreach (var us in myUserStories)
            {
                var proj = LocalClientDatabase.Instance.MyTeamProjects.FirstOrDefault(p => p.Name.Equals(us.Project.Name));

                if(proj != null)
                {
                    if (LocalClientDatabase.Instance.CurrentEmployee.Type == EmployeeType.DEVELOPER)
                    {
                        if (us.ProgressStatus == ProgressStatus.STARTED || us.ProgressStatus == ProgressStatus.COMPLETED)
                        {
                            LocalClientDatabase.Instance.UserStories.Add(us);
                            if (proj.UserStories == null)
                            {
                                proj.UserStories = new List<UserStory>();
                            }
                            proj.UserStories.Add(us);
                        }
                    }
                    else
                    {
                        LocalClientDatabase.Instance.UserStories.Add(us);
                        if (proj.UserStories == null)
                        {
                            proj.UserStories = new List<UserStory>();
                        }
                        proj.UserStories.Add(us);
                    }
                }
            }

            foreach (var task in allTasks)
            {
                var userStory = LocalClientDatabase.Instance.UserStories.FirstOrDefault(us => us.Title.Equals(task.UserStory.Title));

                if (userStory != null)
                {
                    if (LocalClientDatabase.Instance.CurrentEmployee.Type == EmployeeType.DEVELOPER)
                    {
                        if (task.ProgressStatus == ProgressStatus.STARTED || task.ProgressStatus == ProgressStatus.COMPLETED)
                        {
                            LocalClientDatabase.Instance.AllTasks.Add(task);

                            if (userStory.Tasks == null)
                            {
                                userStory.Tasks = new List<Task>();
                            }
                            userStory.Tasks.Add(task);

                            if (task.AssignStatus == AssignStatus.ASSIGNED && task.EmployeeName == LocalClientDatabase.Instance.CurrentEmployee.Name)
                            {
                                LocalClientDatabase.Instance.MyTasks.Add(task);
                            }
                        }
                    }
                    else
                    {
                        LocalClientDatabase.Instance.AllTasks.Add(task);

                        if (userStory.Tasks == null)
                        {
                            userStory.Tasks = new List<Task>();
                        }
                        userStory.Tasks.Add(task);

                        if (task.AssignStatus == AssignStatus.ASSIGNED && task.EmployeeName == LocalClientDatabase.Instance.CurrentEmployee.Name)
                        {
                            LocalClientDatabase.Instance.MyTasks.Add(task);
                        }
                    }
                }
            }
        }

        public void SetHome()
        {
            displayName.Text = LocalClientDatabase.Instance.CurrentEmployee.Name + " " + LocalClientDatabase.Instance.CurrentEmployee.Surname;
            if (LocalClientDatabase.Instance.CurrentEmployee.Team != null)
                displayTeam.Text = LocalClientDatabase.Instance.CurrentEmployee.Team.Name;
            displayType.Text = LocalClientDatabase.Instance.CurrentEmployee.Type.ToString();
            displayEmail.Text = LocalClientDatabase.Instance.CurrentEmployee.Email;
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
            //CEO ***********************
            //Add team
            workCeo.textBoxTeamName.Text = "";

            workCeo.textBoxLeaderName.Text = "";
            workCeo.textBoxLeaderSurname.Text = "";
            workCeo.textBoxLeaderEmail.Text = "";
            workCeo.textBoxTeamName.Text = "";
            workCeo.passwordBoxLeader.Password = "";

            workCeo.tabControlNewTeam.SelectedIndex = 0;

            //Add employee

            workCeo.textBoxName.Text = "";
            workCeo.textBoxSurname.Text = "";
            workCeo.textBoxEmail.Text = "";
            workCeo.passwordBoxNew.Password = "";
            workCeo.comboBoxType.SelectedItem = null;
            workCeo.comboBoxTeamDeveloper.SelectedItem = null;
            workCeo.comboBoxTeamScrum.SelectedItem = null;

            workCeo.addEmployeeTabControl.SelectedIndex = 0;

            //Assign project

            workCeo.comboBoxProjects.SelectedItem = null;
            workCeo.comboBoxTeams.SelectedItem = null;

            //************************

            // Team Leader *************************
            
            // user stories
            workLeader.comboBoxProjects.SelectedItem = null;
            workLeader.textBoxUserStoryTitle.Text = "";
            workLeader.textBoxUserStoryContent.Text = "";
            workLeader.textBoxUserStoryDifficulty.Text = "2";

            workLeader.textProjectDescription.Text = "";

            //make tasks
            workLeader.textBoxTaskTitle.Text = "";
            workLeader.textBoxTaskContent.Text = "";
            workLeader.comboBoxStories.SelectedItem = null;

            workLeader.textUserStoryDescription.Text = "";

            //send user story

            //my tasks
            workLeader.textMyTaskDescription.Text = "";

            //tasks

            workLeader.textTaskDescription.Text = "";

            //DEVELOPER

            //project
            workDev.textProjectDescription.Text = "";

            //userstory
            workDev.textUserStoryDescription.Text = "";

            //my tasks
            workDev.textMyTaskDescription.Text = "";

            //tasks

            workDev.textTaskDescription.Text = "";
        }

        private void editPassword_Click(object sender, RoutedEventArgs e)
        {
            oldPasswordLabel.IsEnabled = true;
            newPasswordLabel.IsEnabled = true;
            passBoxOldPass.IsEnabled = true;
            passBoxNewPass.IsEnabled = true;
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
                workCeo.Visibility = Visibility.Hidden; // kao i svi ostali
                workLeader.Visibility = Visibility.Hidden;
                workDev.Visibility = Visibility.Hidden;
            }
        }

        public void TeamAddedCallbackResult(Team team)
        {
            if(team != null)
            {
                LocalClientDatabase.Instance.Teams.Add(team);
                MessageBox.Show("A new team has been added!\n It can be seen in the Teams pannel.");
            }
            else
            {
                MessageBox.Show("The team could not be added because of an internal server error.");
            }
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

            if (passBoxNewPass.Password != "" && passBoxOldPass.Password != "")
            {
                LocalClientDatabase.Instance.CurrentEmployee.Password = passBoxNewPass.Password;
                LocalClientDatabase.Instance.CurrentEmployee.PasswordTimeStamp = DateTime.Now;
            }

            SetSettings();

            displayName.Text = LocalClientDatabase.Instance.CurrentEmployee.Name + " " + LocalClientDatabase.Instance.CurrentEmployee.Surname;
 
            LocalClientDatabase.Instance.proxy.EditEmployeeData(LocalClientDatabase.Instance.CurrentEmployee);
        }

        private void buttonNotifDetail_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridNotifications.SelectedItem != null)
            {
                MakeNotifInvisible();

                Notification notification = (Notification)dataGridNotifications.SelectedItem;
                notification.StatusNew = NotificationNewStatus.SEEN;
                LocalClientDatabase.Instance.CurrentNotification = notification;
                dataGridNotifications.Items.Refresh();

                switch (notification.Type)
                {
                    case NotificationType.PROJECT_REQUEST:
                    case NotificationType.REQUEST_FOR_PARTNERSHIP:
                        
                        acceptDeclineCanvas.Visibility = Visibility.Visible;

                        if (notification.StatusAccept != NotificationAcceptStatus.PENDING)
                        {
                            acceptDeclineCanvas.buttonAccept.IsEnabled = false;
                            acceptDeclineCanvas.buttonDecline.IsEnabled = false;
                        }
                        else
                        {
                            acceptDeclineCanvas.buttonAccept.IsEnabled = true;
                            acceptDeclineCanvas.buttonDecline.IsEnabled = true;
                        }

                        acceptDeclineCanvas.notificationText.Text = notification.Message;
                        break;

                    case NotificationType.NEW_PROJECT_TL:

                        messageBoxCanvas.notificationText.Text = notification.Message;
                        break;
                }

                CountNewNotifications();
            }
        }

        private void MakeNotifInvisible()
        {
            acceptDeclineCanvas.Visibility = Visibility.Hidden;
            acceptDeclineCanvas.buttonAccept.IsEnabled = false;
            acceptDeclineCanvas.buttonDecline.IsEnabled = false;
            messageBoxCanvas.Visibility = Visibility.Hidden;
        }

        public void TypeChangeCallbackResult(Employee employee)
        {
            LocalClientDatabase.Instance.CurrentEmployee = employee;

            string message = string.Format("Your superior has changed your position to {0}. Your team is {1}.", LocalClientDatabase.Instance.CurrentEmployee.Type, employee.Team.Name);            
            MessageBox.Show(message, "Type Change", MessageBoxButton.OK);

            switch (employee.Type)
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
            bool flag = false;

            foreach (var emp in LocalClientDatabase.Instance.Employees)
            {
                if (emp.Email == employee.Email)
                {
                    LocalClientDatabase.Instance.Employees.Remove(emp);
                    flag = true;
                    break;
                }
            }

            if (flag)
            {
                LocalClientDatabase.Instance.Employees.Add(employee);
            }

            if (employee.Email.Equals(LocalClientDatabase.Instance.CurrentEmployee.Email))
            {
                MessageBox.Show("Your changes have been saved!");
                tabControl1.SelectedIndex = 0;
            }
        }

        public void AddEmployeeCallbackResult(Employee employee)
        {
            if (employee.Type == EmployeeType.DEVELOPER)
            {
                LocalClientDatabase.Instance.Developers.Add(employee);
            }
        }

        /*
        public void UpdateEmployeeFunctionAndTeamCallbackResult(Employee employee)
        {
            bool flag = false;

            foreach(var emp in LocalClientDatabase.Instance.Employees)
            {
                if(emp.Email == employee.Email)
                {
                    LocalClientDatabase.Instance.Employees.Remove(emp);
                    flag = true;
                    break;
                }
            }

            if (flag)
            {
                LocalClientDatabase.Instance.Employees.Add(employee);
            }

            foreach (var emp in LocalClientDatabase.Instance.Developers)
            {
                if (emp.Email == employee.Email)
                {
                    LocalClientDatabase.Instance.Developers.Remove(emp);
                    break;
                }
            }
        }
        */

        public int CountNewNotifications()
        {
            int countNew = 0;

            foreach (var notif in LocalClientDatabase.Instance.CurrentEmployee.Notifications)
            {
                if (notif.StatusNew == NotificationNewStatus.NEW)
                {
                    countNew++;
                }
            }

            textBoxNotifNum.Text = countNew.ToString();

            if(countNew > 0)
            {
                textBoxNotifNum.Background = new SolidColorBrush(Colors.DarkOrange);
                textBoxNotifNum.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                textBoxNotifNum.Background = new SolidColorBrush(Colors.Transparent);
                textBoxNotifNum.Foreground = new SolidColorBrush(Colors.Black);
            }

            return countNew;
        }

        public void SendNotificationToCEOResult(Notification notification)
        {
            LocalClientDatabase.Instance.CurrentEmployee.Notifications.Add(notification);
            LocalClientDatabase.Instance.Notifications.Add(notification);
            CountNewNotifications();
        }

        public void ScrumMasterUpdatedCallbackResult(Team team)
        {
            foreach (var team1 in LocalClientDatabase.Instance.Teams)
            {
                if (team1.Name.Equals(team.Name))
                {
                    team1.ScrumMasterEmail = team.ScrumMasterEmail;
                    break;
                }
            }

            workCeo.comboBoxTeamScrum.Items.Refresh();
        }

        public void ProjectTeamAssignCallbackResult(Project project)
        {
            LocalClientDatabase.Instance.MyTeamProjects.Add(project);
            MessageBox.Show("CEO has assigned a new project to your team. It can be seen in the Work pannel in Projects.");
        }

        public void ReleaseUserStoryCallbackResult(List<Task> tasks)
        {
            if(LocalClientDatabase.Instance.CurrentEmployee.Type == EmployeeType.DEVELOPER || LocalClientDatabase.Instance.CurrentEmployee.Type == EmployeeType.TEAMLEADER)
            {
                LocalClientDatabase.Instance.UserStories.Add(tasks[0].UserStory);
                foreach (var task in tasks)
                {
                    LocalClientDatabase.Instance.AllTasks.Add(task);
                }

                MessageBox.Show("A user story for project " + tasks[0].UserStory.Project.Name + " has arived.\n You can claim tasks.");
            }
        }

        public void ReceiveUserStoriesCallbackResult(List<UserStoryCommon> commStories, string projName)
        {
            var project = LocalClientDatabase.Instance.MyTeamProjects.FirstOrDefault(p => p.Name.Equals(projName));
            project.ProgressStatus = ProgressStatus.STARTED;

            if (project != null)
            {
                foreach (var comStory in commStories)
                {
                    UserStory userStory = project.UserStories.FirstOrDefault(us => us.Title.Equals(comStory.Title));

                    if (userStory != null)
                    {
                        if (comStory.IsAccepted == true)
                        {
                            userStory.AcceptStatus = AcceptStatus.ACCEPTED;
                        }
                        else
                        {
                            userStory.AcceptStatus = AcceptStatus.DECLINED;
                        }
                    }
                }

                workLeader.dataGridUserStories.Items.Refresh();
                workLeader.dataGridUserStories1.Items.Refresh();
                workLeader.dataGridProjects.Items.Refresh();
                workLeader.dataGridProjects1.Items.Refresh();

                MessageBox.Show("Feedbak for users stories has been received from a hiring company!");
            }
        }

        public void TaskClaimedCallbackResult(Task task)
        {
            foreach(var task1 in LocalClientDatabase.Instance.AllTasks)
            {
                if(task1.Title == task.Title)
                {
                    task1.AssignStatus = AssignStatus.ASSIGNED;
                    task1.EmployeeName = task.EmployeeName;
                    task1.ProgressStatus = ProgressStatus.STARTED;
                }
            }

            workLeader.dataGridTasks.Items.Refresh();
            workDev.dataGridTasks.Items.Refresh();
        }

        public void TaskCompletedCallbackResult(Task task)
        {
            foreach (var task1 in LocalClientDatabase.Instance.AllTasks)
            {
                if (task1.Title == task.Title)
                {
                    task1.ProgressStatus = ProgressStatus.COMPLETED;
                }
            }

            var ustory = LocalClientDatabase.Instance.UserStories.FirstOrDefault(us => us.Title.Equals(task.UserStory.Title));

            bool flag = true;
            if(ustory != null)
            {
                foreach(var task1 in ustory.Tasks)
                {
                    if(task1.ProgressStatus != ProgressStatus.COMPLETED)
                    {
                        flag = false;
                    }
                }
            }

            if (flag)
            {
                ustory.ProgressStatus = ProgressStatus.COMPLETED;

                workDev.dataGridUserStories.Items.Refresh();
                workLeader.dataGridUserStories.Items.Refresh();
                workLeader.dataGridUserStories1.Items.Refresh();
            }

            workLeader.dataGridTasks.Items.Refresh();
            workDev.dataGridTasks.Items.Refresh();
            workLeader.dataGridMyTasks.Items.Refresh();
            workDev.dataGridMyTasks.Items.Refresh();
        }

        private void logOutButton_Click(object sender, RoutedEventArgs e)
        {
            LocalClientDatabase.Instance.proxy.LogOut(LocalClientDatabase.Instance.CurrentEmployee);
        }

        private void emailBox_GotFocus(object sender, RoutedEventArgs e)
        {
            errorLogInBox.Text = "";
        }

        private void passwordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            errorLogInBox.Text = "";
        }

        public void AddTeamAndTLCallbackResult(Team team, Employee leader)
        {
            LocalClientDatabase.Instance.Teams.Add(team);

            Employee emp = LocalClientDatabase.Instance.Developers.FirstOrDefault(e => e.Email.Equals(leader.Email));
            

            if (emp != null)
            {
                LocalClientDatabase.Instance.Developers.Remove(emp);
            }

            emp = LocalClientDatabase.Instance.Employees.FirstOrDefault(e => e.Email.Equals(leader.Email));

            if(emp != null)
            {
                emp.Type = EmployeeType.TEAMLEADER;
                dataGridEmployees.Items.Refresh();
            }

            MessageBox.Show("A new team has been added!\n It can be seen in the Teams pannel.");
        }

        public void SendProjectToTeamMembersResult(Project project)
        {
            LocalClientDatabase.Instance.MyTeamProjects.Add(project);
            MessageBox.Show("A new project has started!\n User stories are being prepared.");
        }

        public void ResponseToPartnershipRequestCallbackResult(HiringCompany hiringCompany)
        {
            LocalClientDatabase.Instance.HiringCompanies.Add(hiringCompany);
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
    }
}