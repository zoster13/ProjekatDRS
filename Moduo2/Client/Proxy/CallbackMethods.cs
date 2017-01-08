using ClientCommon;
using System;
using ClientCommon.Data;
using System.ServiceModel;
using System.Threading;

namespace Client
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class CallbackMethods : ICallbackMethods
    {
        MainWindow mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;

        public void EditEmployeeCallback(Employee employee)
        {
            throw new NotImplementedException();
        }

        public void LogInCallback(Employee employee)
        {
             App.Current.Dispatcher.Invoke((Action)delegate
             {
                 mainWindow.LogInCallbackResult(employee);
             });
        }

        public void LogOutCallback(Employee employee)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.LogOutCallbackResult(employee);
            });
        }

        public void TeamAddedCallback(Team team, bool flag)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.TeamAddedCallbackResult(team);
            });
        }

        public void TypeChangeCallback(Team team, EmployeeType newType)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.TypeChangeCallbackResult(team, newType);
            });
        }
    }
}