using ClientCommon.Data;
using System.Collections.Generic;
using System.ServiceModel;

namespace ClientCommon
{
    [ServiceContract]
    public interface IEmployeeService
    {
        [OperationContract]
        Employee LogIn(string username, string password);

        [OperationContract]
        bool LogOut(string username, string password);

        [OperationContract]
        List<Employee> GetAllEmployees();

    }
}
