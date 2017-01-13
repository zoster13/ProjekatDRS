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
            //lock (clientDB.Employees_lock) //mislim da nam ovde ne treba lock jer se u Main.SyncClientDb(data) metodi svakako koristi lock
            //{
            //System.Diagnostics.Debug.WriteLine("Iznad invokeDispatcher");

            var disp = App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new ThreadStart(() =>
                    {
                        clientDB.Main.SyncClientDb(data);
                    }));            
        }
    }
}
