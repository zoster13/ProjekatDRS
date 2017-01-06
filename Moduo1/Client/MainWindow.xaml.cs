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
            dataGridCEO.DataContext = clientDB.Employees;

            foreach (var type in Enum.GetValues(typeof(EmployeeType)))
            {
                comboBoxNewPositionCEO.Items.Add(type);
                comboBoxNewPositionCEO.SelectedIndex = 0;
                comboBoxPositionCEO.Items.Add(type);
                comboBoxPositionCEO.SelectedIndex = 0;
            }

            SetUpConnection(); // izmestila da se poveze sa serverom cim se podigne aplikacija, a ne na log - in....da bi mogao da se ugasi regularno u close metodi
        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {           
            signOutButton.Visibility = System.Windows.Visibility.Visible;
            bool success=proxy.SignIn(usernameBox.Text,passwordBox.Password);
            if (success) 
            {
                logInWarningLabel.Content = "";
                tabControl.SelectedIndex = 1;

                //ovde treba da se napravi da su visible objekti koji
                //su vidjivi samo za odredjeni tip

                clientDB.Username = usernameBox.Text;
            }
            else
            {           
                logInWarningLabel.Content = "Employee with username <" + usernameBox.Text.Trim()+ "> doesn't exist!";
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

        private void saveChanges_Click(object sender, RoutedEventArgs e)
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

        private void usernameTextChanged(object sender, TextChangedEventArgs e)
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

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
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

        private void passBoxOldPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            passBoxNewPass.IsEnabled = true;
        }

        private void comboBoxProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Project p = (Project)comboBoxProjects.SelectedItem;
            BindingList<UserStory> userStories = new BindingList<UserStory>(p.UserStories);
            DataGridUserStories.DataContext = userStories;

            string str ="Id: " + p.Id + "\nName: " + p.Name + "\nDescription: " + p.Description + "\nStartDate: " + p.StartDate.ToString() + "\nDeadline: " + p.Deadline.ToString() + "\nOutsourcingCompany: " + p.OutsourcingCompany+"\nProductOwner: "+p.ProductOwner;
            textBoxProjects.Text = str;
        }

        public void syncClientDB(EmployeeCommon.CurrentData data) 
        {
            lock (clientDB.Employees_lock)
            {
                if (clientDB.Employees.Count != 0)
                {
                    clientDB.Employees.Clear();
                }
                clientDB.Employees.Clear();
                clientDB.Employees = new BindingList<Employee>(data.EmployeesData);
            }
            employeesDataGrid.DataContext = clientDB.Employees;
            dataGridCEO.DataContext = clientDB.Employees;

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
                        homeLabelEmail.Content = em.Email;
                        homeLabelPosition.Content = em.Type.ToString();
                        nameSurnameLabel.Content = em.Name + " " + em.Surname;

                        if (em.Type.Equals(EmployeeType.CEO))
                        {
                            workTabItem.Visibility = Visibility.Visible;
                            tabItemCompanies.Visibility = Visibility.Visible;
                        }
                        else if (em.Type.Equals(EmployeeType.HR))
                        {
                            workTabItem.Visibility = Visibility.Visible;
                            tabItemCompanies.Visibility = Visibility.Hidden;
                        }
                        else
                        {
                            workTabItem.Visibility = Visibility.Hidden;
                            tabItemCompanies.Visibility = Visibility.Hidden;
                        }

                        break;
                    }
                }
            }
        }

        private void SetUpConnection() 
        {
            string employeeSvcEndpoint = "net.tcp://localhost:9999/EmployeeService"; //10.1.212.113

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

            proxy.ChangeEmployeeType(usName, (EmployeeType)comboBoxNewPositionCEO.SelectedItem);
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            Employee newEmployee;
            if ((textBoxNameCEO.Text != "") && (textBoxSurnameCEO.Text != "") && (textBoxEmailCEO.Text != "") && (usernameTextBoxCEO.Text != "") && (passwordBoxCEO.Password != ""))
            {
                newEmployee = new Employee(usernameTextBoxCEO.Text, passwordBoxCEO.Password, (EmployeeType)comboBoxPositionCEO.SelectedItem, textBoxNameCEO.Text, textBoxSurnameCEO.Text, textBoxEmailCEO.Text, 0, 0, 0, 0);
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
                proxy.SignOut(clientDB.Username);
            proxy.Dispose();
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
