using ClientCommon.Data;
using System.Collections.Generic;
using System.ServiceModel;

namespace ClientCommon
{
    [ServiceContract]
    public interface IEmployeeService
    {
        [OperationContract]
        Employee LogIn(string email, string password);

        [OperationContract]
        bool LogOut(string email);

        [OperationContract]
        List<Employee> GetAllEmployees();
    }
}
