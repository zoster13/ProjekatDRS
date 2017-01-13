using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using EmployeeCommon.Data;

namespace EmployeeCommon
{
    public interface IEmployeeServiceCallback
    {
        [OperationContract]
        void Notify(string message);

        [OperationContract]
        void SyncData(CurrentData data);

    }
}
