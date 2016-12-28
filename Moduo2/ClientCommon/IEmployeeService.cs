using ClientCommon.Data;

namespace ClientCommon
{
    public interface IEmployeeService
    {
        Employee LogIn(string username, string password);

    }
}
