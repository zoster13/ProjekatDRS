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
                System.Diagnostics.Debug.WriteLine("Iznad invokeDispatcher");

                App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Input,
                    new ThreadStart(() => 
                    {
                        clientDB.Main.nekaMetoda(data);
                    }
                    )
                        );

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
