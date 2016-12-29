using ClientCommon.Data;
using System.Collections.Generic;
using System.ServiceModel;

namespace ClientCommon
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ICallbackMethods))]
    public interface IEmployeeService
    {
        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void LogIn(string email, string password);

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void LogOut(string email);

        [OperationContract]
        List<Employee> GetAllEmployees();
    }
}
