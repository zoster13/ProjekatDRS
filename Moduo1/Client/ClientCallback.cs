using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using EmployeeCommon;
using System.ServiceModel;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using EmployeeCommon.Data;

namespace Client
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class ClientCallback : EmployeeCommon.IEmployeeServiceCallback
    {
        private ClientDatabase clientDB = ClientDatabase.Instance();

        public void Notify(string message)
        {
            var disp = App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new ThreadStart(() =>
                    {
                        clientDB.Main.NotifyEmployee(message);
                    }));
        }

        public void SyncData(CurrentData data)
        {
            var disp = App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new ThreadStart(() =>
                    {
                        clientDB.Main.SyncClientDb(data);
                    }));            
        }
    }
}
