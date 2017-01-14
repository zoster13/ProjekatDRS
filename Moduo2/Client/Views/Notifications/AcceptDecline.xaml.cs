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
using ClientCommon.Data;

namespace Client.Views.Notifications
{
    /// <summary>
    /// Interaction logic for ChangeType.xaml
    /// </summary>
    public partial class AcceptDecline : UserControl
    {
        private MainWindow mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;

        public AcceptDecline()
        {
            InitializeComponent();
            mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
        }

        private void ButtonAccept_Click(object sender, RoutedEventArgs e)
        {
            switch (LocalClientDatabase.Instance.CurrentNotification.Type)
            {
                case NotificationType.REQUEST_FOR_PARTNERSHIP:
                    if (LocalClientDatabase.Instance.CurrentNotification.StatusAccept == NotificationAcceptStatus.PENDING)
                    {
                        HiringCompany hiringCompany = new HiringCompany();
                        hiringCompany.Name = LocalClientDatabase.Instance.CurrentNotification.HiringCompanyName;
                        //LocalClientDatabase.Instance.HiringCompanies.Add(hiringCompany);
                        LocalClientDatabase.Instance.CurrentNotification.StatusAccept = NotificationAcceptStatus.ACCEPTED;
                        mainWindow.dataGridNotifications.Items.Refresh();

                        this.Visibility = Visibility.Hidden;
                        buttonAccept.IsEnabled = false;
                        buttonDecline.IsEnabled = false;

                        LocalClientDatabase.Instance.Proxy.ResponseToPartnershipRequest(true, LocalClientDatabase.Instance.CurrentNotification.HiringCompanyName);
                        MessageBox.Show("Partnership with hiring company  " + LocalClientDatabase.Instance.CurrentNotification.HiringCompanyName + " has been accepted!\n It can be seen in the Companies pannel.");
                    }
                    else
                    {
                        MessageBox.Show("This request has already been " + LocalClientDatabase.Instance.CurrentNotification.StatusAccept + "!");
                    }
                    break;
                case NotificationType.PROJECT_REQUEST:
                    if (LocalClientDatabase.Instance.CurrentNotification.StatusAccept == NotificationAcceptStatus.PENDING)
                    {
                        Project project = new Project();
                        project.Name = LocalClientDatabase.Instance.CurrentNotification.ProjectName;
                        project.HiringCompanyName = LocalClientDatabase.Instance.CurrentNotification.HiringCompanyName;
                        project.Description = LocalClientDatabase.Instance.CurrentNotification.ProjectDescription;
                        LocalClientDatabase.Instance.AllProjects.Add(project);
                        LocalClientDatabase.Instance.CurrentNotification.StatusAccept = NotificationAcceptStatus.ACCEPTED;
                        mainWindow.dataGridNotifications.Items.Refresh();

                        this.Visibility = Visibility.Hidden;
                        buttonAccept.IsEnabled = false;
                        buttonDecline.IsEnabled = false;

                        LocalClientDatabase.Instance.Proxy.ResponseToProjectRequest(true, project);
                        
                        MessageBox.Show("Project named: " + LocalClientDatabase.Instance.CurrentNotification.ProjectName + " has been accepted!\n It can be seen in the Work pannel in Projects.");
                    }
                    else
                    {
                        MessageBox.Show("This request has already been " + LocalClientDatabase.Instance.CurrentNotification.StatusAccept + "!");
                    }
                    break;
            }
        }

        private void ButtonDecline_Click(object sender, RoutedEventArgs e)
        {
            switch (LocalClientDatabase.Instance.CurrentNotification.Type)
            {
                case NotificationType.REQUEST_FOR_PARTNERSHIP:
                    if (LocalClientDatabase.Instance.CurrentNotification.StatusAccept == NotificationAcceptStatus.PENDING)
                    {
                        LocalClientDatabase.Instance.CurrentNotification.StatusAccept = NotificationAcceptStatus.DECLINED;
                        mainWindow.dataGridNotifications.Items.Refresh();

                        this.Visibility = Visibility.Hidden;
                        buttonAccept.IsEnabled = false;
                        buttonDecline.IsEnabled = false;

                        LocalClientDatabase.Instance.Proxy.ResponseToPartnershipRequest(false, LocalClientDatabase.Instance.CurrentNotification.HiringCompanyName);

                        MessageBox.Show("Partnership with hiring company  " + LocalClientDatabase.Instance.CurrentNotification.HiringCompanyName + " has been declined!");
                    }
                    else
                    {
                        MessageBox.Show("This request has already been " + LocalClientDatabase.Instance.CurrentNotification.StatusAccept + "!");
                    }
                    break;
                case NotificationType.PROJECT_REQUEST:
                    if (LocalClientDatabase.Instance.CurrentNotification.StatusAccept == NotificationAcceptStatus.PENDING)
                    {
                        Project project = new Project();
                        project.Name = LocalClientDatabase.Instance.CurrentNotification.ProjectName;
                        project.HiringCompanyName = LocalClientDatabase.Instance.CurrentNotification.HiringCompanyName;
                        project.Description = LocalClientDatabase.Instance.CurrentNotification.ProjectDescription;

                        LocalClientDatabase.Instance.CurrentNotification.StatusAccept = NotificationAcceptStatus.DECLINED;
                        mainWindow.dataGridNotifications.Items.Refresh();

                        this.Visibility = Visibility.Hidden;
                        buttonAccept.IsEnabled = false;
                        buttonDecline.IsEnabled = false;

                        LocalClientDatabase.Instance.Proxy.ResponseToProjectRequest(false, project);

                        MessageBox.Show("Project named: " + LocalClientDatabase.Instance.CurrentNotification.ProjectName + " has been declined!");
                    }
                    else
                    {
                        MessageBox.Show("This request has already been " + LocalClientDatabase.Instance.CurrentNotification.StatusAccept + "!");
                    }
                    break;
            }
        }
    }
}
