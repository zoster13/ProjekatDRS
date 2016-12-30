using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using EmployeeCommon;
using System.ServiceModel;

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


                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    clientDB.Main.nekaMetoda(data);
                });
                //BindingList<Employee> bTemp = new BindingList<Employee>(data.EmployeesData);
                //clientDB.Employees = bTemp;
                //clientDB.Employees = new BindingList<Employee>(data.EmployeesData);
                //clientDB.Main.nekaMetoda();
                
                //clientDB.Main.employeesDataGrid.DataContext = clientDB.Employees;
            }
        }
    }
}
