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

namespace Client
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class ClientCallback : EmployeeCommon.IEmployeeServiceCallback
    {
        ClientDatabase clientDB = ClientDatabase.Instance();

        public void Notify()
        {
           
        }


        public void SyncData(EmployeeCommon.CurrentData data)
        {
            lock (clientDB.Employees_lock)
            {
                //System.Diagnostics.Debug.WriteLine("Iznad invokeDispatcher");

                var disp=App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new ThreadStart(() =>
                    {
                        clientDB.Main.syncClientDB(data);
                    }
                    )
                        );

                //while (disp.Status != DispatcherOperationStatus.Completed) 
                //{
                //    Thread.Sleep(10);
                //    System.Diagnostics.Debug.WriteLine("Ceka da se zavrsi disp thread");
                //}

                //App.Current.Dispatcher.Invoke((Action)delegate
                //{
                //    System.Diagnostics.Debug.WriteLine("Uslo u invokeDispatcher");
                //    clientDB.Main.nekaMetoda(data);
                //    System.Diagnostics.Debug.WriteLine("Vratilo se iz main-a");
                //});  

                //BindingList<Employee> bTemp = new BindingList<Employee>(data.EmployeesData);
                //clientDB.Employees = bTemp;
                //clientDB.Employees = new BindingList<Employee>(data.EmployeesData);
                //clientDB.Main.nekaMetoda();
                
                //clientDB.Main.employeesDataGrid.DataContext = clientDB.Employees;
            }
        }
    }
}
