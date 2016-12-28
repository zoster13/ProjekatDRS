using ClientCommon.Data;
using System.ServiceModel;

namespace ClientCommon
{
    [ServiceContract]
    public interface IEmployeeService
    {
        [OperationContract]
        Employee LogIn(string username, string password);
    }
}
