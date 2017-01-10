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
        MainWindow mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;

        public AcceptDecline()
        {
            InitializeComponent();
            mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
        }

        private void buttonAccept_Click(object sender, RoutedEventArgs e)
        {
            switch (LocalClientDatabase.Instance.CurrentNotification.Type)
            {
                case NotificationType.REQUEST_FOR_PARTNERSHIP:
                    if (LocalClientDatabase.Instance.CurrentNotification.StatusAccept == NotificationAcceptStatus.PENDING)
                    {
                        HiringCompany hiringCompany = new HiringCompany();
                        hiringCompany.Name = LocalClientDatabase.Instance.CurrentNotification.HiringCompanyName;
                        LocalClientDatabase.Instance.HiringCompanies.Add(hiringCompany);
                        LocalClientDatabase.Instance.CurrentNotification.StatusAccept = NotificationAcceptStatus.ACCEPTED;
                        mainWindow.dataGridNotifications.Items.Refresh();

                        this.Visibility = Visibility.Hidden;
                        buttonAccept.IsEnabled = false;
                        buttonDecline.IsEnabled = false;

                        LocalClientDatabase.Instance.proxy.ResponseToPartnershipRequest(true, LocalClientDatabase.Instance.CurrentNotification.HiringCompanyName);
                        MessageBox.Show("Partnership with hiring company  " + LocalClientDatabase.Instance.CurrentNotification.HiringCompanyName + " has been accepted!");
                    }
                    else
                    {
                        MessageBox.Show("This request has already been " + LocalClientDatabase.Instance.CurrentNotification.StatusAccept + "!");
                    }
                    break;
            }
        }

        private void buttonDecline_Click(object sender, RoutedEventArgs e)
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

                        LocalClientDatabase.Instance.proxy.ResponseToPartnershipRequest(false, LocalClientDatabase.Instance.CurrentNotification.HiringCompanyName);

                        MessageBox.Show("Partnership with hiring company  " + LocalClientDatabase.Instance.CurrentNotification.HiringCompanyName + " has been declined!");
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
