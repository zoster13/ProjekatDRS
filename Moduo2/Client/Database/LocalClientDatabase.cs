using ClientCommon.Data;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ServiceModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace Client
{
    public class LocalClientDatabase
    {
        private static string address = "net.tcp://localhost:9999/EmployeeService";
        private EndpointAddress epAddress = new EndpointAddress(new Uri(address));
        private NetTcpBinding binding = new NetTcpBinding();
        public EmployeeProxy proxy;

        private static LocalClientDatabase localDB;
        private BindingList<Employee> employees;
        private BindingList<Employee> developers;
        private BindingList<Team> teams;
        private BindingList<HiringCompany> hiringCompanies;

        private Employee currentEmployee;
        private object locker;

        private Canvas notificationCanvas;

        private LocalClientDatabase()
        {
            employees = new BindingList<Employee>();
            developers = new BindingList<Employee>();
            teams = new BindingList<Team>();
            hiringCompanies = new BindingList<HiringCompany>();
            currentEmployee = new Employee();
            locker = new object();

            notificationCanvas = new Canvas();
            notificationCanvas.Background = new SolidColorBrush(Colors.DarkOrange);

            TextBox notificationNumTB = new TextBox();
            notificationNumTB.Text = "1";
            notificationCanvas.Children.Add(notificationNumTB);

            proxy = new EmployeeProxy(binding, epAddress, new CallbackMethods());
        }

        public static LocalClientDatabase Instance
        {
            get
            {
                if (localDB == null)
                    localDB = new LocalClientDatabase();

                return localDB;
            }
            set
            {
                if (localDB == null)
                    localDB = value;
            }
        }

        public static void NullifyInstance()
        {
            localDB = null;
        }

        public BindingList<Employee> Employees
        {
            get
            {
                return employees;
            }
            set
            {
                employees = value;
            }
        }

        public BindingList<Team> Teams
        {
            get
            {
                return teams;
            }
            set
            {
                teams = value;
            }
        }

        public BindingList<HiringCompany> HiringCompanies
        {
            get
            {
                return hiringCompanies;
            }
            set
            {
                hiringCompanies = value;
            }
        }

        public BindingList<Employee> Developers
        {
            get
            {
                return developers;
            }
            set
            {
                developers = value;
            }
        }

        public Employee CurrentEmployee
        {
            get
            {
                return currentEmployee;
            }
            set
            {
                currentEmployee = value;
            }
        }

        public object Locker
        {
            get { return this.locker; }
            set { this.locker = value; }
        }

    }
}
