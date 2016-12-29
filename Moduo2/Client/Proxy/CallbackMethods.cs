using ClientCommon;
using System;
using ClientCommon.Data;

namespace Client
{
    public class CallbackMethods : ICallbackMethods
    {
        public void LogInCallback(Employee employee)
        {
            //obavjesti ostale da sam se prijavio
            throw new NotImplementedException();
        }

        public void LogOutCallback(Employee employee)
        {
            //obavjesti ostale da sam se odjavio
            throw new NotImplementedException();
        }
    }
}
