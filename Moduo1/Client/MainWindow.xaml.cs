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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ClientDatabase clientDB = ClientDatabase.Instance();
        ClientProxy proxy;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = clientDB;

            clientDB.Main = this;

            comboBoxProjects.DataContext = clientDB.Projects;

            employeesDataGrid.DataContext = clientDB.Employees;
            companiesDataGrid.DataContext = clientDB.Companies; //verovatno ce i ovo morati opet da se nakuca u SyncClientDB metodi-ako budemo slali celu listu,isto kao za Employees
            dataGridCEO.DataContext = clientDB.AllEmployees;
            projectsForApprovalDataGrid.DataContext = clientDB.ProjectsForApproval;
            //WorkCompaniesDataGrid.DataContext = clientDB.Companies; //i ovo ce morati da se ponovi u nekoj SyncMetodi,kad se izmeni lista partnerskih kompanija
            comboBoxCompanies.DataContext = clientDB.Companies;
            dataGrid_NotPartnerCompanies.DataContext = clientDB.NamesOfCompanies;
            approvedprojectsInDevelopmentDataGrid.DataContext = clientDB.ProjectsForSending;

            

            foreach (EmployeeType type in Enum.GetValues(typeof(EmployeeType)))
            {
                comboBoxNewPositionCEO.Items.Add(Extensions.TypeToString(type));
                comboBoxNewPositionCEO.SelectedIndex = 0;

                comboBoxPositionCEO.Items.Add(Extensions.TypeToString(type));
                comboBoxPositionCEO.SelectedIndex = 0;
            }

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

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            SetUpConnection();

           // Debug.WriteLine(proxy.InnerChannel.State);
            //Debug.WriteLine(proxy.State);

            if (proxy.State == CommunicationState.Opened)
            {

               // Debug.WriteLine(proxy.InnerChannel.State);
               // Debug.WriteLine(proxy.State);

                bool success = proxy.SignIn(usernameBox.Text.Trim(), passwordBox.Password);

                if (success)
                {
                    warningLabel.Content = "";
                    tabControl.SelectedIndex = 1;

                    signOutButton.Visibility = System.Windows.Visibility.Visible;
                    clientDB.Username = usernameBox.Text.Trim();
                }
                else
                {
                    warningLabel.Content = string.Format("Employee with username <{0}> does not exist!",
                        usernameBox.Text);
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

        private void signOutButton_Click(object sender, RoutedEventArgs e)
        {
            proxy.SignOut(clientDB.Username.Trim());

            usernameBox.Text = "";
            passwordBox.Password = "";
            nameSurnameLabel.Content = "";
            signOutButton.Visibility = System.Windows.Visibility.Hidden;
            tabControl.SelectedIndex = 0;
        }

        private void editPassword_Click(object sender, RoutedEventArgs e)
        {
            passBoxOldPass.IsEnabled = true;
            //passBoxNewPass.IsEnabled = true;
            //passBoxConfirmNewPass.IsEnabled = true;
        }

        private void editData_Click(object sender, RoutedEventArgs e)
        {
            lock (clientDB.Employees_lock)
            {
                foreach (Employee em in clientDB.Employees)
                {
                    if (em.Username == clientDB.Username)
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
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
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
            //passBoxConfirmNewPass.IsEnabled = false;
            //passBoxConfirmNewPass.Password = "";

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

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            passBoxOldPass.IsEnabled = false;
            passBoxOldPass.Password = "";
            passBoxNewPass.IsEnabled = false;
            passBoxNewPass.Password = "";
            //passBoxConfirmNewPass.IsEnabled = false;
            //passBoxConfirmNewPass.Password = "";

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

        private void PassBoxOldPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            passBoxNewPass.IsEnabled = true;
        }

        private void ComboBoxProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxProjects.SelectedItem != null) 
            {
                Project p = (Project)comboBoxProjects.SelectedItem;
                BindingList<UserStory> userStories = new BindingList<UserStory>(p.UserStories);
                DataGridUserStories.DataContext = userStories;

                string str = "Name: " + p.Name + "\nDescription: " + p.Description + "\nStartDate: " + p.StartDate.ToString() + "\nDeadline: " + p.Deadline.ToString() + "\nOutsourcingCompany: " + p.OutsourcingCompany + "\nProductOwner: " + p.ProductOwner;
                textBoxProjects.Text = str;
            }          
        }

        public void SyncClientDb(EmployeeCommon.CurrentData data)
        {
            lock (clientDB.Employees_lock)
            {
                if (clientDB.Employees.Count != 0)
                {
                    clientDB.Employees.Clear();
                }
                clientDB.Employees = new BindingList<Employee>(data.EmployeesData);
            }
            employeesDataGrid.DataContext = clientDB.Employees;

            lock (clientDB.AllEmployees_lock)
            {
                if (clientDB.AllEmployees.Count != 0)
                {
                    clientDB.AllEmployees.Clear();
                }
                clientDB.AllEmployees = new BindingList<Employee>(data.AllEmployeesData);
            }
            dataGridCEO.DataContext = clientDB.AllEmployees;


            lock (clientDB.ProjectsForApproval_lock)
            {
                if (clientDB.ProjectsForApproval.Count != 0)
                {
                    clientDB.ProjectsForApproval.Clear();
                }
                clientDB.ProjectsForApproval = new BindingList<Project>(data.ProjectsForApprovalData);
            }
            projectsForApprovalDataGrid.DataContext = clientDB.ProjectsForApproval;

            lock (clientDB.NamesOfCompanies_lock)
            {
                if (clientDB.NamesOfCompanies.Count != 0)
                {
                    clientDB.NamesOfCompanies.Clear();
                }
                clientDB.NamesOfCompanies = new BindingList<string>(data.NamesOfCompaniesData);
            }
            dataGrid_NotPartnerCompanies.DataContext = clientDB.NamesOfCompanies;

            lock (clientDB.Companies_lock)
            {
                if (clientDB.Companies.Count != 0)
                {
                    clientDB.Companies.Clear();
                }
                clientDB.Companies = new BindingList<PartnerCompany>(data.CompaniesData);
            }
            companiesDataGrid.DataContext = clientDB.Companies;         
            //WorkCompaniesDataGrid.DataContext = clientDB.Companies;
            comboBoxCompanies.DataContext = clientDB.Companies;

            lock (clientDB.ProjectsForSending_lock)
            {
                if (clientDB.ProjectsForSending.Count != 0)
                {
                    clientDB.ProjectsForSending.Clear();
                }
                clientDB.ProjectsForSending = new BindingList<Project>(data.ProjectsForSendingData);
            }
            approvedprojectsInDevelopmentDataGrid.DataContext = clientDB.ProjectsForSending;

            lock (clientDB.Projects_lock) 
            {
                if (clientDB.Projects.Count != 0) 
                {
                    clientDB.Projects.Clear();
                }
                clientDB.Projects = new BindingList<Project>(data.ProjectsInDevelopmentData);
            }
            comboBoxProjects.DataContext = clientDB.Projects; // koje projekte prikazujemo ovde? 
          

            FillHomeLabels();
        }

        public void FillHomeLabels()
        {
            lock (clientDB.Employees_lock)
            {
                foreach (Employee em in clientDB.Employees)
                {
                    if (em.Username == clientDB.Username)
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
                        }
                        else if (em.Type.Equals(EmployeeType.HR))
                        {
                            workTabItem.Visibility = Visibility.Visible;
                            tabItemCompanies.Visibility = Visibility.Hidden;
                            tabItemProjects.Visibility = Visibility.Hidden;
                            PO_Work_TabItem.Visibility = Visibility.Hidden;
                        }
                        else if (em.Type.Equals(EmployeeType.PO))
                        {
                            workTabItem.Visibility = Visibility.Hidden;
                            tabItemCompanies.Visibility = Visibility.Hidden;
                            tabItemProjects.Visibility = Visibility.Hidden;
                            PO_Work_TabItem.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            workTabItem.Visibility = Visibility.Hidden;
                            tabItemCompanies.Visibility = Visibility.Hidden;
                            tabItemProjects.Visibility = Visibility.Hidden;
                            PO_Work_TabItem.Visibility = Visibility.Hidden;
                        }

                        break;
                    }
                }
            }
        }


        private void ApplyTypeChangeButton_Click(object sender, RoutedEventArgs e)
        {
            string usName = ((Employee)dataGridCEO.SelectedItem).Username;
            proxy.ChangeEmployeeType(usName, Extensions.StringToType((string)comboBoxNewPositionCEO.SelectedItem));
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            Employee newEmployee;
            if ((textBoxNameCEO.Text != "") && (textBoxSurnameCEO.Text != "") && (textBoxEmailCEO.Text != "") && (usernameTextBoxCEO.Text != "") && (passwordBoxCEO.Password != ""))
            {
                newEmployee = new Employee(usernameTextBoxCEO.Text, passwordBoxCEO.Password, Extensions.StringToType((string)comboBoxPositionCEO.SelectedItem),
                    textBoxNameCEO.Text, textBoxSurnameCEO.Text, textBoxEmailCEO.Text, 0, 0, 0, 0);

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

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            // uraditi close proxy-ija i sve to...ako treba pocistiti bazu i sta vec
            if (!clientDB.Username.Equals(string.Empty))
            {
                Debug.WriteLine("Client sign out");
                proxy.SignOut(clientDB.Username);
                proxy.Dispose();
            }

            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void PartnershipRequestCEOButton_Click(object sender, RoutedEventArgs e)
        {
            proxy.AskForPartnership((string)dataGrid_NotPartnerCompanies.SelectedItem);
        }

        private void ProjectRequestButton_CLick(object sender, RoutedEventArgs e)
        {
            if (approvedprojectsInDevelopmentDataGrid.SelectedItem != null)
            {
                proxy.SendProject(( (PartnerCompany)comboBoxCompanies.SelectedItem ).Name, (Project)approvedprojectsInDevelopmentDataGrid.SelectedItem);
                //proxy.SendProject((string)WorkCompaniesDataGrid.SelectedItem, (Project)approvedprojectsInDevelopmentDataGrid.SelectedItem);
            }


        }

        private void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            Project p = new Project(ProjectNameTextBox.Text.Trim(), ProjectDescriptionTextBox.Text.Trim(), clientDB.Username);

            Debug.WriteLine("Attempting to parse strings using {0} culture.",
                        CultureInfo.CurrentCulture.Name);

            // namestiti da se vrse provere da li je datum validan 
            // i da bude vizuelni feedback, npr crvene ivice, ako klijent ne ukuca kako treba
            //p.StartDate = Convert.ToDateTime(ProjectStartDateTextBox.Text);
            DateTime dateTimeOut;
            if (DateTime.TryParse(ProjectStartDateTextBox.Text, out dateTimeOut)) //tako nesto raditi kada se text promeni ili slicno...
            {
                p.StartDate = dateTimeOut;
            }

            if (DateTime.TryParse(ProjectDeadlineTextBox.Text, out dateTimeOut)) //tako nesto raditi kada se text promeni ili slicno...
            {
                p.Deadline = dateTimeOut;
            }
            //p.Deadline = Convert.ToDateTime(ProjectDeadlineTextBox.Text);

            proxy.CreateNewProject(p);

            ProjectNameTextBox.Text = "";
            ProjectDescriptionTextBox.Text = "";
            ProjectStartDateTextBox.Text = "";
            ProjectDeadlineTextBox.Text = "";
        }

        private void ApproveProjectButton_CLick(object sender, RoutedEventArgs e)
        {
            if (projectsForApprovalDataGrid.SelectedItem != null)
            {
                Project proj = (Project)projectsForApprovalDataGrid.SelectedItem;

                lock (clientDB.ProjectsForApproval_lock)
                {
                    proxy.ProjectApprovedByCeo(proj);
                }
            }              
        }

        // da li da cuvamo notifikacije u bazi?
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

            tb.Height = 45;
            //tb.Width = 128;
            tb.Width = double.NaN; // auto?

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

        private void UsernameBox_GotFocus(object sender, RoutedEventArgs e)
        {

            //  warningLabel.Visibility = Visibility.Hidden;
        }

        private void TextBoxNameCeoTextChanged(object sender, TextChangedEventArgs e)
        {
            labelUsernameExists.Visibility = Visibility.Hidden;
        }

        private void ApproveUserStoriesButton_Click(object sender, RoutedEventArgs e)
        {
            Project p = (Project)comboBoxProjects.SelectedItem;
            //napraviti listu userStorija za odobravanje koji ce se slati
            foreach (CheckBox cb in UserStoriesForApprovalListBox.Items)
            {
                if (cb.IsChecked == true)
                {
                    foreach (UserStory us in p.UserStories)
                    {
                        if (us.Title.Equals(cb.Content))
                        {
                            //dodati u listu za odobravanje userStory(promeniti mu polje isAccepted ili kako vec)
                        }
                    }
                }
                //poslati listu odobrenih
            }

            UserStoriesForApprovalListBox.Items.Clear();
        }

        private void ProjectsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxProjects.SelectedItem != null) 
            {
                Project p = (Project)comboBoxProjects.SelectedItem;
                BindingList<UserStory> userStories = new BindingList<UserStory>(p.UserStories);

                foreach (UserStory us in p.UserStories)//mozda i nekako drugacije,ne znam kako i gde cemo smestati userStory-je koje nam posalju,videcemo
                {
                    //string s = String.Format("Name: {0}", us.Title);
                    CheckBox property = new CheckBox()
                    {

                        Content = us.Title, //dodati i ostatak
                        IsChecked = false
                    };
                    UserStoriesForApprovalListBox.Items.Add(property);
                }
            }
            
        }
    }
}
