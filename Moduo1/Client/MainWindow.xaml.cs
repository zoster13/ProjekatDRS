using System;
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
            WorkCompaniesDataGrid.DataContext = clientDB.Companies; //i ovo ce morati da se ponovi u nekoj SyncMetodi,kad se izmeni lista partnerskih kompanija
            dataGrid_NotPartnerCompanies.DataContext = clientDB.NamesOfCompanies;

            foreach(EmployeeType type in Enum.GetValues(typeof(EmployeeType)))
            {

                comboBoxNewPositionCEO.Items.Add(Extensions.TypeToString(type));
                comboBoxNewPositionCEO.SelectedIndex = 0;

                comboBoxPositionCEO.Items.Add(Extensions.TypeToString(type));
                comboBoxPositionCEO.SelectedIndex = 0;
            }


        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            SetUpConnection();

            signOutButton.Visibility = System.Windows.Visibility.Visible;
            bool success = proxy.SignIn(usernameBox.Text, passwordBox.Password);

            if(success)
            {
                logInWarningLabel.Content = "";
                tabControl.SelectedIndex = 1;

                //ovde treba da se napravi da su visible objekti koji
                //su vidjivi samo za odredjeni tip

                clientDB.Username = usernameBox.Text;
            }
            else
            {
                logInWarningLabel.Content = "Employee with username <" + usernameBox.Text.Trim() + "> doesn't exist!";
                usernameBox.Text = "";
                passwordBox.Password = "";
            }
        }

        private void signOutButton_Click(object sender, RoutedEventArgs e)
        {
            proxy.SignOut(clientDB.Username);

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
            lock(clientDB.Employees_lock)
            {
                foreach(Employee em in clientDB.Employees)
                {
                    if(em.Username == clientDB.Username)
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

        private void saveChanges_Click(object sender, RoutedEventArgs e)
        {
            proxy.ChangeEmployeeData(clientDB.Username, textBoxEditName.Text, textBoxEditSurname.Text, textBoxEditEmail.Text, passBoxNewPass.Password);
            FillHomeLabels();

            if(workBeginHour.Text != "" && workBeginMinute.Text != "" && workEndHour.Text != "" && workEndMinute.Text != "")
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

        private void usernameTextChanged(object sender, TextChangedEventArgs e)
        {
            logInWarningLabel.Visibility = Visibility.Hidden;
            if(usernameBox.Text != "" && passwordBox.Password != "")
            {
                logInButton.IsEnabled = true;
            }
            else
            {
                logInButton.IsEnabled = false;
            }
        }

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(usernameBox.Text != "" && passwordBox.Password != "")
            {
                logInButton.IsEnabled = true;
            }
            else
            {
                logInButton.IsEnabled = false;
            }
        }

        private void passBoxOldPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            passBoxNewPass.IsEnabled = true;
        }

        private void comboBoxProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Project p = (Project)comboBoxProjects.SelectedItem;
            BindingList<UserStory> userStories = new BindingList<UserStory>(p.UserStories);
            DataGridUserStories.DataContext = userStories;

            string str = "Id: " + p.Id + "\nName: " + p.Name + "\nDescription: " + p.Description + "\nStartDate: " + p.StartDate.ToString() + "\nDeadline: " + p.Deadline.ToString() + "\nOutsourcingCompany: " + p.OutsourcingCompany + "\nProductOwner: " + p.ProductOwner;
            textBoxProjects.Text = str;
        }

        public void syncClientDB(EmployeeCommon.CurrentData data)
        {
            lock(clientDB.Employees_lock)
            {
                if(clientDB.Employees.Count != 0)
                {
                    clientDB.Employees.Clear();
                }
                //clientDB.Employees.Clear();
                clientDB.Employees = new BindingList<Employee>(data.EmployeesData);
            }
            employeesDataGrid.DataContext = clientDB.Employees;

            lock(clientDB.AllEmployees_lock)
            {
                if(clientDB.AllEmployees.Count != 0)
                {
                    clientDB.AllEmployees.Clear();
                }
                clientDB.AllEmployees = new BindingList<Employee>(data.AllEmployeesData);
            }
            dataGridCEO.DataContext = clientDB.AllEmployees;

            lock(clientDB.ProjectsForApproval_lock)
            {
                if(clientDB.ProjectsForApproval.Count != 0)
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
            
            FillHomeLabels();
        }

        public void FillHomeLabels()
        {
            lock(clientDB.Employees_lock)
            {
                foreach(Employee em in clientDB.Employees)
                {
                    if(em.Username == clientDB.Username)
                    {
                        homeLabelName.Content = em.Name;
                        homeLabelSurname.Content = em.Surname;
                        TextBlock tb = new TextBlock();
                        tb.Text = em.Email;
                        homeLabelEmail.Content = tb;
                        homeLabelPosition.Content = Extensions.TypeToString(em.Type);

                        nameSurnameLabel.Content = em.Name + " " + em.Surname;

                        if(em.Type.Equals(EmployeeType.CEO))
                        {
                            workTabItem.Visibility = Visibility.Visible;
                            tabItemCompanies.Visibility = Visibility.Visible;
                            tabItemProjects.Visibility = Visibility.Visible;
                            PO_Work_TabItem.Visibility = Visibility.Hidden;
                        }
                        else if(em.Type.Equals(EmployeeType.HR))
                        {
                            workTabItem.Visibility = Visibility.Visible;
                            tabItemCompanies.Visibility = Visibility.Hidden;
                            tabItemProjects.Visibility = Visibility.Hidden;
                            PO_Work_TabItem.Visibility = Visibility.Hidden;
                        }
                        else if(em.Type.Equals(EmployeeType.PO))
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

        private void SetUpConnection()
        {
            string employeeSvcEndpoint = "net.tcp://10.1.212.113:9999/EmployeeService";
            //string employeeSvcEndpoint = "net.tcp://localhost:9999/EmployeeService"; // 10.1.212.113

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



        private void ApplyTypeChangeButton_Click(object sender, RoutedEventArgs e)
        {
            string usName = ((Employee)dataGridCEO.SelectedItem).Username;
            proxy.ChangeEmployeeType(usName, Extensions.StringToType((string)comboBoxNewPositionCEO.SelectedItem));
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            Employee newEmployee;
            if((textBoxNameCEO.Text != "") && (textBoxSurnameCEO.Text != "") && (textBoxEmailCEO.Text != "") && (usernameTextBoxCEO.Text != "") && (passwordBoxCEO.Password != ""))
            {
                newEmployee = new Employee(usernameTextBoxCEO.Text, passwordBoxCEO.Password, Extensions.StringToType((string)comboBoxPositionCEO.SelectedItem),
                    textBoxNameCEO.Text, textBoxSurnameCEO.Text, textBoxEmailCEO.Text, 0, 0, 0, 0);
                //Na serverskoj strani uraditi proveru
                //postoji li vec zaposleni sa tim username-om.
                proxy.AddNewEmployee(newEmployee);

                textBoxNameCEO.Text = "";
                textBoxSurnameCEO.Text = "";
                textBoxEmailCEO.Text = "";
                usernameTextBoxCEO.Text = "";
                passwordBoxCEO.Password = "";
                comboBoxPositionCEO.SelectedIndex = 0;
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            // uraditi close proxy-ija i sve to...ako treba pocistiti bazu i sta vec
            if(!clientDB.Username.Equals(string.Empty))
            {
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
            //u metodi na serveru treba da se promeni polje isAcceptedOutsCompany da bude true
        }

        private void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            Project p = new Project();
            p.Name = ProjectNameTextBox.Text;
            p.Description = ProjectDescriptionTextBox.Text;
            p.StartDate = Convert.ToDateTime(ProjectStartDateTextBox.Text);
            p.Deadline = Convert.ToDateTime(ProjectDeadlineTextBox.Text); // namestiti da se vrse provere da li je datum validan i da bude vizuelni feedback ako klijent ne ukuca kako treba
            p.ProductOwner = clientDB.Username;

            proxy.CreateNewProject(p);

            ProjectNameTextBox.Text = "";
            ProjectDescriptionTextBox.Text = "";
            ProjectStartDateTextBox.Text = "";
            ProjectDeadlineTextBox.Text = "";
        }

        private void ApproveProjectButton_CLick(object sender, RoutedEventArgs e)
        {
            Project proj = (Project)projectsForApprovalDataGrid.SelectedItem;

            lock(clientDB.ProjectsForApproval_lock)
            {
                proxy.ProjectApproved(proj); //u ProjectApproved metodi na serveru treba da se promeni polje isApprovedCEO da bude true
                
                //foreach (Project p in clientDB.ProjectsForApproval)
                //{
                //    if (p.Name.Equals(proj.Name))
                //    {
                //        //clientDB.ProjectsForApproval.Remove(p);
                //proxy.ProjectApproved(p);

                //break;
                //    }
                //}
            }
            //dodati liste projekata(verovatno ih ima vise) i u pravu serversku bazu,ne samo u memoriju
            //dodati proj u listu odobrenih projekata
            //(odluciti da li je to lista projects koja vec postoji ili treba napraviti listu odobrenih,pa kad outsComp prihvati,onda se projekat smesta u listu projects)
            //Poslati notification PO da je projekat odobren                     
        }

        // da li da cuvamo notifikacije u bazi?
        private void NotificationsButton_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            NotificationsButton.Background = (Brush)bc.ConvertFrom("#FFFAFBFE");
            notificationCounter.Content = "0";

            //if (notificationCounter.Visibility == Visibility.Visible)
            //{
            //    notificationCounter.Visibility = Visibility.Hidden;
            //}

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
            tb.Width = 128;

            Border myBorder = new Border();
            // myBorder.Background = Brushes.SkyBlue;
            BrushConverter bc = new BrushConverter();
            myBorder.Background = (Brush)bc.ConvertFrom("#FFFAFBFE");
            //myBorder.BorderBrush = Brushes.Black;  
            myBorder.BorderBrush = (Brush)bc.ConvertFrom("#FFACACAC");
            myBorder.BorderThickness = new Thickness(1);
            myBorder.Child = tb;

            notificationsStackPanel.Children.Insert(0, myBorder);
            //notificationsStackPanel.Children.Add(myBorder);
            int notifNum = Int32.Parse((string)notificationCounter.Content);
            notifNum++;
            notificationCounter.Content = notifNum.ToString();

            //if (notificationCounter.Visibility == Visibility.Hidden)
            //{
            //    notificationCounter.Visibility = Visibility.Visible;
            //}
            notificationCounter.Background = Brushes.Red;
        }
    }
}
