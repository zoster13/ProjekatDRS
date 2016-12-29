using ClientCommon;
using System;
using ClientCommon.Data;

namespace Client
{
    public class CallbackMethods : ICallbackMethods
    {
        public void LogInCallback(Employee employee)
        {
            LocalClientDatabase.Instance.Employees.Add(employee);
        }

        public void LogOutCallback(Employee employee)
        {
            foreach(var em in LocalClientDatabase.Instance.Employees)
            {
                if(em.Email == employee.Email)
                {
                    LocalClientDatabase.Instance.Employees.Remove(employee);
                }
            }
        }
    }
}
