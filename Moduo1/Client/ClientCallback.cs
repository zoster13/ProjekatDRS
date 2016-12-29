using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using EmployeeCommon;

namespace Client
{
    public class ClientCallback : EmployeeCommon.IEmployeeServiceCallback
    {
        ClientDatabase clientDB = ClientDatabase.Instance();

        public void Notify()
        {
           
        }


        public void SyncData(EmployeeCommon.CurrentData data)
        {
            clientDB.Employees.Clear();
            clientDB.Employees = new BindingList<Employee>(data.EmployeesData);
        }
    }
}
