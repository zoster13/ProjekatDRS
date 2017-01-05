using ClientCommon.Data;
using System.ServiceModel;

namespace ClientCommon
{
    [ServiceContract]
    public interface ICallbackMethods
    {
        [OperationContract(IsOneWay = true)]
        [ServiceKnownType(typeof(Employee))]
        void LogInCallback(Employee employee);

        [OperationContract(IsOneWay = true)]
        void LogOutCallback(Employee employee);

        [OperationContract(IsOneWay = true)]
        void TeamAddedCallback(Team team, bool flag);
    }
}
