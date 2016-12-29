using ClientCommon.Data;
using System.ServiceModel;

namespace ClientCommon
{
    [ServiceContract]
    public interface ICallbackMethods
    {
        [OperationContract(IsOneWay = true)]
        void LogInCallback(Employee employee);

        [OperationContract(IsOneWay =true)]
        void LogOutCallback(Employee employee);
    }
}
