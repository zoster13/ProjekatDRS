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
        private BindingList<Notification> notifications;
        private BindingList<Project> allProjects;
        private BindingList<Project> myTeamProjects;

        private BindingList<UserStory> userStories;

        private BindingList<Task> allTasks;
        private BindingList<Task> myTasks;

        private Employee currentEmployee;
        private Notification currentNotification;
        private object locker;

        private Canvas notificationCanvas;

        private LocalClientDatabase()
        {
            //binding.OpenTimeout = new TimeSpan(1, 0, 0);
            //binding.CloseTimeout = new TimeSpan(1, 0, 0);
            //binding.SendTimeout = new TimeSpan(1, 0, 0);
            //binding.ReceiveTimeout = new TimeSpan(1, 0, 0);

            employees = new BindingList<Employee>();
            developers = new BindingList<Employee>();
            teams = new BindingList<Team>();
            hiringCompanies = new BindingList<HiringCompany>();
            notifications = new BindingList<Notification>();
            allProjects = new BindingList<Project>();
            myTeamProjects = new BindingList<Project>();

            userStories = new BindingList<UserStory>();

            allTasks = new BindingList<Task>();
            myTasks = new BindingList<Task>();

            currentEmployee = new Employee();
            currentNotification = new Notification();
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

        public BindingList<Notification> Notifications
        {
            get
            {
                return notifications;
            }
            set
            {
                notifications = value;
            }
        }

        public BindingList<UserStory> UserStories
        {
            get
            {
                return userStories;
            }
            set
            {
                userStories = value;
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

        public BindingList<Notification> DevNotificationselopers
        {
            get
            {
                return notifications;
            }
            set
            {
                notifications = value;
            }
        }

        public BindingList<Task> AllTasks
        {
            get
            {
                return allTasks;
            }
            set
            {
                allTasks = value;
            }
        }

        public BindingList<Task> MyTasks
        {
            get
            {
                return myTasks;
            }
            set
            {
                myTasks = value;
            }
        }

        public BindingList<Project> AllProjects
        {
            get
            {
                return allProjects;
            }
            set
            {
                allProjects = value;
            }
        }

        public BindingList<Project> MyTeamProjects
        {
            get
            {
                return myTeamProjects;
            }
            set
            {
                myTeamProjects = value;
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

        public Notification CurrentNotification
        {
            get
            {
                return currentNotification;
            }
            set
            {
                currentNotification = value;
            }
        }

        public object Locker
        {
            get { return this.locker; }
            set { this.locker = value; }
        }

    }
}
