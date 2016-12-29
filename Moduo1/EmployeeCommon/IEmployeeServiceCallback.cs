using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommon
{
    public interface IEmployeeServiceCallback
    {
        [OperationContract]
        void Notify();

        [OperationContract]
        void SyncData(CurrentData data);
    }
}
