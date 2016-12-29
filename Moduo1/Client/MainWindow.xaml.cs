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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ClientDatabase clientDB = ClientDatabase.Instance();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = clientDB;
            employeesDataGrid.DataContext = clientDB.Employees;

            //dodati companiesDataGrid.DataContext= clientDB.listaKompanija
            //dodati projectsDataGrid.DataContext=clientDB.listaProjekata

            string employeeSvcEndpoint = "net.tcp://localhost:9999/EmployeeService";

            NetTcpBinding binding = new NetTcpBinding();           
            binding.OpenTimeout = new TimeSpan(1, 0, 0);
            binding.CloseTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(employeeSvcEndpoint));
            IEmployeeServiceCallback callback = new ClientCallback();

            InstanceContext instanceContext = new InstanceContext(callback);

            using (ClientProxy proxy = new ClientProxy(instanceContext, binding, endpointAddress))
            {
                proxy.SignIn();

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);

                proxy.SignOut();
            };

        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 1;

            //smisliti kako da u listu clientDB.Employee na svaki odredjeni vremenski period
            //smestamo zaposlene koji se nalaze u listi online korisnika na serveru
            //takodje, ovde verovatno treba da se napravi da su visible objekti koji
            //su vidjivi samo za odredjeni tip

            //pozvati serversku metodu za logovanje,f-ja mora da ima povratnu vrednost da bi se znalo da li taj korisnik postoji u database,
            //ako postoji i loguje se,onda raditi ovo ispod

            clientDB.Username = usernameBox.Text;

            foreach (Employee em in clientDB.Employees)
            {
                if (em.Username == clientDB.Username)
                {
                    homeLabel1.Content=em.Name;
                    homeLabel2.Content=em.Surname;
                    homeLabel3.Content=em.Email;
                    homeLabel4.Content=em.Type.ToString(); //Ne znam da li mora ovo ToString
                    break;
                }
            }

            //srediti vidljivost objekata na osnovu tipa
        }

        private void editPassword_Click(object sender, RoutedEventArgs e)
        {
            passBoxOldPass.IsEnabled = true;
            passBoxNewPass.IsEnabled = true;
            passBoxConfirmNewPass.IsEnabled = true;
        }

        private void editData_Click(object sender, RoutedEventArgs e)
        {
            foreach (Employee em in clientDB.Employees)
            {
                if (em.Username == clientDB.Username)
                {
                    textBoxEditName.IsEnabled = true;
                    textBoxEditName.Text=em.Name;

                    textBoxEditSurname.IsEnabled = true;
                    textBoxEditSurname.Text=em.Surname;

                    textBoxEditEmail.IsEnabled = true;
                    textBoxEditEmail.Text=em.Email;

                    textBoxEditUsername.IsEnabled = true;
                    textBoxEditUsername.Text=em.Username;

                    //takodje uraditi to i za radno vreme
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

        private void showOnlineEmployees_Click(object sender, RoutedEventArgs e)
        {

        }

        private void showPartnerCompanies_Click(object sender, RoutedEventArgs e)
        {

        }

        private void projectOverview_Click(object sender, RoutedEventArgs e)
        {

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

        private void passwordGotFocus(object sender, RoutedEventArgs e)
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
    }
}
