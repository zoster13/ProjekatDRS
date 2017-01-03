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
        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            SetUpConnection();

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
                usernameBox.Text = "";
                passwordBox.Password = "";
                logInWarningLabel.Content = "Employee with that username doesn't exist!";
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
            passBoxNewPass.IsEnabled = true;
            passBoxConfirmNewPass.IsEnabled = true;
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

                        textBoxEditUsername.IsEnabled = true;
                        textBoxEditUsername.Text = em.Username;

                        //takodje uraditi to i za radno vreme
                    }
                }
            }
            
        }

        private void saveChanges_Click(object sender, RoutedEventArgs e)
        {
            //Napraviti neku proveru koja su polja popunjena/promenjena i sacuvati izmene
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            passBoxOldPass.IsEnabled = false;
            passBoxNewPass.IsEnabled = false;
            passBoxConfirmNewPass.IsEnabled = false;

            textBoxEditName.IsEnabled = false;
            textBoxEditSurname.IsEnabled = false;
            textBoxEditEmail.IsEnabled = false;
            textBoxEditUsername.IsEnabled = false;

            //uraditi to i za radno vreme
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
            //System.Diagnostics.Debug.WriteLine("Neka metoda pocetak");
            if (clientDB.Employees.Count != 0)
            {
                clientDB.Employees.Clear();
            }
            clientDB.Employees.Clear();
            clientDB.Employees = new BindingList<Employee>(data.EmployeesData);
            //System.Diagnostics.Debug.WriteLine("Neka metoda kraj");
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
            string employeeSvcEndpoint = "net.tcp://10.1.212.113:9999/EmployeeService"; //10.1.212.113

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

        private void ApplyTypeChangeButton_Click(object sender, RoutedEventArgs e) //menja podatke u listi(u klijentskoj DB,serversku stranu i pravu bazu nisam dirala),ali ne menja podatke u tabelama koje su bindovane sa tom listom,
        {                                                                           //valjda treba da se upotrebljava IObserver ili sta vec,moram toga da se podsetim,kasnije cu da pogledam to
            string usName = ((Employee)dataGridCEO.SelectedItem).Username;          //treba ovde da se doda i poziv za servera da izmeni podatke u bazi,
                                                                                    //ovako sam radila samo za probu
            lock (clientDB.Employees_lock)
            {
                foreach (Employee em in clientDB.Employees)
                {
                    if (em.Username.Equals(usName))
                    {
                        em.Type = (EmployeeType)comboBoxNewPositionCEO.SelectedItem;
                        break;
                    }
                }
            }
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            Employee newEmployee;
            if ((textBoxNameCEO.Text != "") && (textBoxSurnameCEO.Text != "") && (textBoxEmailCEO.Text != "") && (usernameTextBoxCEO.Text != "") && (passwordBoxCEO.Password != ""))
            {
                newEmployee = new Employee(usernameTextBoxCEO.Text, passwordBoxCEO.Password, (EmployeeType)comboBoxPositionCEO.SelectedItem, textBoxNameCEO.Text, textBoxSurnameCEO.Text, textBoxEmailCEO.Text, 0, 0, 0, 0);
                clientDB.Employees.Add(newEmployee); //ne treba ovako,radjeno samo za proveru,
                //treba se pozvati serverska metoda da se doda u pravu bazu,da se sync sa klijentskom bazom,i na serverskoj strani se radi provera
                //postoji li vec zaposleni sa tim username-om.
                textBoxNameCEO.Text = "";
                textBoxSurnameCEO.Text = "";
                textBoxEmailCEO.Text = "";
                usernameTextBoxCEO.Text = "";
                passwordBoxCEO.Password = "";
                comboBoxPositionCEO.SelectedIndex = 0;
            }
        }
    }
}
