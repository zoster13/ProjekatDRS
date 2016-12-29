using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientCallback : EmployeeCommon.IEmployeeServiceCallback
    {
        public void Notify()
        {
           
        }


        public void SyncData(EmployeeCommon.CurrentData data)
        {
            throw new NotImplementedException();
        }
    }
}
