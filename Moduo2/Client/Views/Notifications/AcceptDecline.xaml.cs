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
            Notification notification = LocalClientDatabase.Instance.CurrentNotification;

            switch (notification.Type)
            {
                case NotificationType.REQUEST_FOR_PARTNERSHIP:
                    HiringCompany hiringCompany = new HiringCompany();
                   // hiringCompany.Name = 
                    break;
            }
        }

        private void buttonDecline_Click(object sender, RoutedEventArgs e)
        {
            Notification notification = LocalClientDatabase.Instance.CurrentNotification;

            switch (notification.Type)
            {
                case NotificationType.REQUEST_FOR_PARTNERSHIP:

                    break;
            }
        }
    }
}
