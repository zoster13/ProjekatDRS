﻿using System;
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
using EmployeeCommon;
using System.ServiceModel;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using EmployeeCommon.Data;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClientDatabase clientDB = ClientDatabase.Instance();
        private ClientProxy proxy;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = clientDB;

            clientDB.Main = this;

            foreach (EmployeeType type in Enum.GetValues(typeof(EmployeeType)))
            {
                comboBoxNewPositionCEO.Items.Add(Extensions.TypeToString(type));
                comboBoxNewPositionCEO.SelectedIndex = 0;

                comboBoxPositionCEO.Items.Add(Extensions.TypeToString(type));
                comboBoxPositionCEO.SelectedIndex = 0;
            }

            string dateTimeLabel = new System.Globalization.CultureInfo(CultureInfo.CurrentCulture.Name).DateTimeFormat.ShortDatePattern;
            DateTimeFormatLabel.Content = dateTimeLabel;
        }

        private void SetUpConnection()
        {
            string employeeSvcEndpoint = "net.tcp://10.1.212.113:9999/EmployeeService";
            //string employeeSvcEndpoint = "net.tcp://localhost:9999/EmployeeService";

            NetTcpBinding binding = new NetTcpBinding();
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(employeeSvcEndpoint));
            IEmployeeServiceCallback callback = new ClientCallback();

            InstanceContext instanceContext = new InstanceContext(callback);

            proxy = new ClientProxy(instanceContext, binding, endpointAddress);
        }

        #region LogIn, LogOut Handlers

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            SetUpConnection();

            if (proxy.State == CommunicationState.Opened)           
            {
                bool success = proxy.SignIn(usernameBox.Text.Trim(), passwordBox.Password);

                if (success)
                {
                    warningLabel.Content = "";
                    tabControl.SelectedIndex = 1;
                    tabControl1.SelectedIndex = 0;
                    comboBoxProjects.SelectedItem = null;
                    WorkPOProjectsComboBox.SelectedItem = null;

                    signOutButton.Visibility = System.Windows.Visibility.Visible;
                    clientDB.Username = usernameBox.Text.Trim();
                    clientDB.Password = passwordBox.Password;
                }
                else
                {
                    warningLabel.Content = string.Format("Employee with username <{0}> does not exist, or password is incorrect!", usernameBox.Text);
                    usernameBox.Text = "";
                    passwordBox.Password = "";
                    warningLabel.Visibility = Visibility.Visible;
                }
            }
            else
            {
                warningLabel.Content = "<EmployeeService> is not available.";
                warningLabel.Visibility = Visibility.Visible;
            }

        }

        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            proxy.SignOut(clientDB.Username.Trim());

            usernameBox.Text = "";
            passwordBox.Password = "";
            nameSurnameLabel.Content = "";
            signOutButton.Visibility = System.Windows.Visibility.Hidden;
            tabControl.SelectedIndex = 0;
        }

        private void UsernameBox_GotFocus(object sender, RoutedEventArgs e)
        {

            //  warningLabel.Visibility = Visibility.Hidden;
        }

        private void UsernameTextChanged(object sender, TextChangedEventArgs e)
        {
            warningLabel.Visibility = Visibility.Hidden;
            if (usernameBox.Text != "" && passwordBox.Password != "")
            {
                logInButton.IsEnabled = true;
            }
            else
            {
                logInButton.IsEnabled = false;
            }
        }

        private void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
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

        #endregion

        #region Projects tab - Projects in development overview

        private void ComboBoxProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxProjects.SelectedItem != null)
            {
                Project p = (Project)comboBoxProjects.SelectedItem;
                BindingList<UserStory> userStories = new BindingList<UserStory>(p.UserStories);
                DataGridUserStories.DataContext = userStories;

                string str = "Name: " + p.Name + "\nDescription: " + p.Description + "\nStartDate: " + p.StartDate.ToString() + "\nDeadline: " + p.Deadline.ToString() + "\nOutsourcingCompany: " + p.OutsourcingCompany
                    + "\nProductOwner: " + p.ProductOwner + "\nScrumMaster: " + p.ScrumMaster + "\nIsApprovedCEO: " + p.IsAcceptedCEO + "\nIsAcceptedOutsCompany: " + p.IsAcceptedOutsCompany + "\nIsClosed: " + p.IsClosed;
                textBoxProjects.Text = str;
            }
        }

        #endregion

        #region Work tab, for CEO and HR 

        // employees tab
        private void TextBoxNameCeoTextChanged(object sender, TextChangedEventArgs e)
        {
            labelUsernameExists.Visibility = Visibility.Hidden;
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            Employee newEmployee;
            if ((textBoxNameCEO.Text != "") && (textBoxSurnameCEO.Text != "") && (textBoxEmailCEO.Text != "") && (usernameTextBoxCEO.Text != "") && (passwordBoxCEO.Password != ""))
            {
                newEmployee = new Employee(usernameTextBoxCEO.Text, passwordBoxCEO.Password, Extensions.StringToType((string)comboBoxPositionCEO.SelectedItem),
                    textBoxNameCEO.Text, textBoxSurnameCEO.Text, textBoxEmailCEO.Text, 0, 0, 0, 0);
                newEmployee.DatePasswordChanged = DateTime.Now;

                textBoxNameCEO.Text = "";
                textBoxSurnameCEO.Text = "";
                textBoxEmailCEO.Text = "";
                usernameTextBoxCEO.Text = "";
                passwordBoxCEO.Password = "";
                comboBoxPositionCEO.SelectedIndex = 0;

                if (!proxy.AddNewEmployee(newEmployee))
                {
                    labelUsernameExists.Visibility = Visibility.Visible;
                }
            }
        }

        private void ApplyTypeChangeButton_Click(object sender, RoutedEventArgs e)
        {
            string usName = ((Employee)dataGridCEO.SelectedItem).Username;
            proxy.ChangeEmployeeType(usName, Extensions.StringToType((string)comboBoxNewPositionCEO.SelectedItem));
        }

        // not partner companies tab
        private void PartnershipRequestCEOButton_Click(object sender, RoutedEventArgs e)
        {
            proxy.AskForPartnership((string)PossiblePartnerCompaniesDataGrid.SelectedItem);
        }

        // projects for approval, and for assinging to development company tab  
        private void ApproveProjectButton_CLick(object sender, RoutedEventArgs e)
        {
            if (projectsForApprovalDataGrid.SelectedItem != null)
            {
                Project proj = (Project)projectsForApprovalDataGrid.SelectedItem;

                proxy.ProjectApprovedByCeo(proj);
            }
        }

        private void ProjectRequestButton_CLick(object sender, RoutedEventArgs e)
        {
            if (approvedprojectsInDevelopmentDataGrid.SelectedItem != null)
            {
                proxy.SendProject(((PartnerCompany)comboBoxPartnerCompanies.SelectedItem).Name, (Project)approvedprojectsInDevelopmentDataGrid.SelectedItem);
            }
        }

        #endregion

        #region work tab, for PO

        private void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            Project p = new Project(ProjectNameTextBox.Text.Trim(), ProjectDescriptionTextBox.Text.Trim(), clientDB.Username, (string)ScrumMasterComboBox.SelectedItem);

            DateTime dateTimeOut;
            if (DateTime.TryParse(ProjectStartDateTextBox.Text, out dateTimeOut))
            {
                p.StartDate = dateTimeOut;
            }

            if (DateTime.TryParse(ProjectDeadlineTextBox.Text, out dateTimeOut))
            {
                p.Deadline = dateTimeOut;
            }

            ProjectNameTextBox.Text = "";
            ProjectDescriptionTextBox.Text = "";
            ProjectStartDateTextBox.Text = "";
            ProjectDeadlineTextBox.Text = "";

            proxy.CreateNewProject(p);       
        }

        private void ProjectsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WorkPOProjectsComboBox.SelectedItem != null)
            {
                Project p = (Project)WorkPOProjectsComboBox.SelectedItem;
                BindingList<UserStory> userStories = new BindingList<UserStory>(p.UserStories.FindAll(u => u.IsApprovedByPO == false));

                foreach (UserStory us in userStories)
                {
                    string tTip = String.Format("Description: " + us.Description + "\nAcceptanceCriteria: " + us.AcceptanceCriteria);
                    CheckBox userStory = new CheckBox()
                    {
                        Content = us.Title,
                        IsChecked = false,
                        ToolTip = tTip
                    };
                    UserStoriesForApprovalListBox.Items.Add(userStory);
                }
            }
        }

        private void ApproveUserStoriesButton_Click(object sender, RoutedEventArgs e)
        {
            Project p = (Project)WorkPOProjectsComboBox.SelectedItem;
            foreach (CheckBox cb in UserStoriesForApprovalListBox.Items)
            {
                if (cb.IsChecked == true)
                {
                    foreach (UserStory us in p.UserStories)
                    {
                        if (us.Title.Equals(cb.Content))
                        {
                            us.IsApprovedByPO = true;
                        }
                    }
                }
            }
            UserStoriesForApprovalListBox.Items.Clear();

            proxy.SendApprovedUserStories(p.Name, p.UserStories);
        }

        private void CloseProjectButton_Click(object sender, RoutedEventArgs e)
        {
            Project p = (Project)ProjectsForClosingComboBox.SelectedItem;
            proxy.CloseProject(p.Name);
        }

        #endregion

        #region settings tab

        private void EditData_Click(object sender, RoutedEventArgs e)
        {
            Employee em = clientDB.Employees.SingleOrDefault(employee => employee.Username.Equals(clientDB.Username));
            if (em != null)
            {
                textBoxEditName.IsEnabled = true;
                textBoxEditName.Text = em.Name;

                textBoxEditSurname.IsEnabled = true;
                textBoxEditSurname.Text = em.Surname;

                textBoxEditEmail.IsEnabled = true;
                textBoxEditEmail.Text = em.Email;

                workBeginHour.IsEnabled = true;
                workBeginHour.Text = em.StartHour.ToString();
                workBeginMinute.IsEnabled = true;
                workBeginMinute.Text = em.StartMinute.ToString();
                workEndHour.IsEnabled = true;
                workEndHour.Text = em.EndHour.ToString();
                workEndMinute.IsEnabled = true;
                workEndMinute.Text = em.EndMinute.ToString();
            }
        }

        private void EditPassword_Click(object sender, RoutedEventArgs e)
        {
            passBoxOldPass.IsEnabled = true;
            passBoxNewPass.IsEnabled = true;
        }

        
        private void PassBoxOldPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
        }

        private void PassBoxOldPass_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void PassBoxNewPass_GotFocus(object sender, RoutedEventArgs e)
        {
            if (passBoxOldPass.Password != String.Empty)
            {
                if (passBoxOldPass.Password.Equals(clientDB.Password))
                {
                    warningPasswordLabel.Content = "";
                }
                else
                {
                    warningPasswordLabel.Content = "Current password incorrect!";
                }
            }
            else
            {
                warningPasswordLabel.Content = "You must type in current password first!";
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            clientDB.Password = passBoxNewPass.Password;
            proxy.ChangeEmployeeData(clientDB.Username, textBoxEditName.Text, textBoxEditSurname.Text, textBoxEditEmail.Text, passBoxNewPass.Password);
            FillHomeLabels();

            if (workBeginHour.Text != "" && workBeginMinute.Text != "" && workEndHour.Text != "" && workEndMinute.Text != "")
            {
                proxy.SetWorkingHours(clientDB.Username, Int32.Parse(workBeginHour.Text), Int32.Parse(workBeginMinute.Text), Int32.Parse(workEndHour.Text), Int32.Parse(workEndMinute.Text));
            }

            passBoxOldPass.IsEnabled = false;
            passBoxOldPass.Password = "";
            passBoxNewPass.IsEnabled = false;
            passBoxNewPass.Password = "";

            textBoxEditName.IsEnabled = false;
            textBoxEditName.Text = "";
            textBoxEditSurname.IsEnabled = false;
            textBoxEditSurname.Text = "";
            textBoxEditEmail.IsEnabled = false;
            textBoxEditEmail.Text = "";

            workBeginHour.IsEnabled = false;
            workBeginHour.Text = "";
            workBeginMinute.IsEnabled = false;
            workBeginMinute.Text = "";
            workEndHour.IsEnabled = false;
            workEndHour.Text = "";
            workEndMinute.IsEnabled = false;
            workEndMinute.Text = "";
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            passBoxOldPass.IsEnabled = false;
            passBoxOldPass.Password = "";
            passBoxNewPass.IsEnabled = false;
            passBoxNewPass.Password = "";

            textBoxEditName.IsEnabled = false;
            textBoxEditName.Text = "";
            textBoxEditSurname.IsEnabled = false;
            textBoxEditSurname.Text = "";
            textBoxEditEmail.IsEnabled = false;
            textBoxEditEmail.Text = "";

            workBeginHour.IsEnabled = false;
            workBeginHour.Text = "";
            workBeginMinute.IsEnabled = false;
            workBeginMinute.Text = "";
            workEndHour.IsEnabled = false;
            workEndHour.Text = "";
            workEndMinute.IsEnabled = false;
            workEndMinute.Text = "";
        }

        #endregion

        private void NotificationsButton_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            NotificationsButton.Background = (Brush)bc.ConvertFrom("#FFFAFBFE");
            notificationCounter.Content = "0";
            notificationCounter.Background = (Brush)bc.ConvertFrom("#FFFAFBFE");

            scrollViewerNotifications.Visibility = scrollViewerNotifications.Visibility == Visibility.Collapsed
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public void NotifyEmployee(string message)
        {
            TextBlock tb = new TextBlock();
            tb.Text = message;
            tb.TextWrapping = TextWrapping.Wrap;

            tb.Height = double.NaN;
            tb.Width = double.NaN; // auto

            Border myBorder = new Border();
            BrushConverter bc = new BrushConverter();
            myBorder.Background = (Brush)bc.ConvertFrom("#FFFAFBFE");
            myBorder.BorderBrush = (Brush)bc.ConvertFrom("#FFACACAC");
            myBorder.BorderThickness = new Thickness(1);
            myBorder.Child = tb;

            notificationsStackPanel.Children.Insert(0, myBorder);
            int notifNum = Int32.Parse((string)notificationCounter.Content);
            notifNum++;
            notificationCounter.Content = notifNum.ToString();
            notificationCounter.Background = Brushes.Cyan;
        }

        public void FillHomeLabels()
        {
            Employee em = clientDB.Employees.SingleOrDefault(employee => employee.Username.Equals(clientDB.Username));
            if (em != null)
            {
                homeLabelName.Content = em.Name;
                homeLabelSurname.Content = em.Surname;
                TextBlock tb = new TextBlock();
                tb.Text = em.Email;
                homeLabelEmail.Content = tb;
                homeLabelPosition.Content = Extensions.TypeToString(em.Type);

                nameSurnameLabel.Content = em.Name + " " + em.Surname;

                if (em.Type.Equals(EmployeeType.CEO))
                {
                    workTabItem.Visibility = Visibility.Visible;
                    tabItemCompanies.Visibility = Visibility.Visible;
                    tabItemProjects.Visibility = Visibility.Visible;
                    PO_Work_TabItem.Visibility = Visibility.Hidden;
                    Projects_TabItem.Focusable = true;
                }
                else if (em.Type.Equals(EmployeeType.HR))
                {
                    workTabItem.Visibility = Visibility.Visible;
                    tabItemCompanies.Visibility = Visibility.Hidden;
                    tabItemProjects.Visibility = Visibility.Hidden;
                    PO_Work_TabItem.Visibility = Visibility.Hidden;
                    Projects_TabItem.Focusable = false;
                }
                else if (em.Type.Equals(EmployeeType.PO))
                {
                    workTabItem.Visibility = Visibility.Hidden;
                    tabItemCompanies.Visibility = Visibility.Hidden;
                    tabItemProjects.Visibility = Visibility.Hidden;
                    PO_Work_TabItem.Visibility = Visibility.Visible;
                    Projects_TabItem.Focusable = true;
                }
                else
                {
                    workTabItem.Visibility = Visibility.Hidden;
                    tabItemCompanies.Visibility = Visibility.Hidden;
                    tabItemProjects.Visibility = Visibility.Hidden;
                    PO_Work_TabItem.Visibility = Visibility.Hidden;
                    Projects_TabItem.Focusable = true;
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            if (!clientDB.Username.Equals(string.Empty))
            {
                Debug.WriteLine("Client sign out");
                proxy.SignOut(clientDB.Username);
                proxy.Dispose();
            }
            this.Close();
        }

        public void SyncClientDb(CurrentData data)
        {
            if (clientDB.Employees.Count != 0)
            {
                clientDB.Employees.Clear();
            }
            clientDB.Employees = new BindingList<Employee>(data.EmployeesData);
            
            employeesDataGrid.DataContext = clientDB.Employees;

            if (clientDB.AllEmployees.Count != 0)
            {
                clientDB.AllEmployees.Clear();
            }
            clientDB.AllEmployees = new BindingList<Employee>(data.AllEmployeesData);
            dataGridCEO.DataContext = clientDB.AllEmployees;

            if (clientDB.ProjectsForApproval.Count != 0)
            {
                clientDB.ProjectsForApproval.Clear();
            }
            clientDB.ProjectsForApproval = new BindingList<Project>(data.ProjectsForApprovalData);
            projectsForApprovalDataGrid.DataContext = clientDB.ProjectsForApproval;

            if (clientDB.NamesOfCompanies.Count != 0)
            {
                clientDB.NamesOfCompanies.Clear();
            }
            clientDB.NamesOfCompanies = new BindingList<string>(data.NamesOfCompaniesData);
            PossiblePartnerCompaniesDataGrid.DataContext = clientDB.NamesOfCompanies;

            if (clientDB.Companies.Count != 0)
            {
                clientDB.Companies.Clear();
            }
            clientDB.Companies = new BindingList<PartnerCompany>(data.CompaniesData);
            partnerCompaniesDataGrid.DataContext = clientDB.Companies;
            comboBoxPartnerCompanies.DataContext = clientDB.Companies;

            if (clientDB.ProjectsForSending.Count != 0)
            {
                clientDB.ProjectsForSending.Clear();
            }
            clientDB.ProjectsForSending = new BindingList<Project>(data.ProjectsForSendingData);
            approvedprojectsInDevelopmentDataGrid.DataContext = clientDB.ProjectsForSending;

            if (clientDB.ProjectsInDevelopment.Count != 0)
            {
                clientDB.ProjectsInDevelopment.Clear();
            }
            clientDB.ProjectsInDevelopment = new BindingList<Project>(data.ProjectsInDevelopmentData);
            comboBoxProjects.DataContext = clientDB.ProjectsInDevelopment; 
            WorkPOProjectsComboBox.DataContext = clientDB.ProjectsInDevelopment;

            clientDB.ProjectsForClosing.Clear();

            //projectsInDevelopment su svi projekti(i zatvoreni i oni koji se rade trenutno)
            foreach (Project p in clientDB.ProjectsInDevelopment) 
            {
                if (p.UserStories.Count != 0 && p.IsClosed == false)
                {
                    List<UserStory> us = p.UserStories.FindAll(u => u.IsClosed == false);
                    if (us.Count == 0)
                    {
                        clientDB.ProjectsForClosing.Add(p);
                    }
                }              
            }      
            ProjectsForClosingComboBox.DataContext = clientDB.ProjectsForClosing;

            FillHomeLabels();
            foreach (Employee em in clientDB.AllEmployees)
            {
                if (em.Type == EmployeeType.SM)
                {
                    if (!ScrumMasterComboBox.Items.Contains(em.Username))
                    {
                        ScrumMasterComboBox.Items.Add(em.Username);
                    }                                     
                }
            }                  
        }
    }
}
