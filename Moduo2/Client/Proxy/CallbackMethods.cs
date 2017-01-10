using ClientCommon;
using System;
using ClientCommon.Data;
using System.ServiceModel;
using System.Threading;
using ICommon;

namespace Client
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class CallbackMethods : ICallbackMethods
    {
        MainWindow mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
        
        public void EditEmployeeCallback(Employee employee)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.EditEmployeeDataCalbackResult(employee);
            });
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
            
        }

        public void AddEmployeeCallback(Employee employee)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.AddEmployeeCallbackResult(employee);
            });
        }

        public void UpdateEmployeeFunctionAndTeamCallback(Employee employee)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.UpdateEmployeeFunctionAndTeamCallbackResult(employee);
            });
        }

        public void NotifyJustMe(Employee employee)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.TypeChangeCallbackResult(employee);
            });
        }

        public void ScrumMasterUpdatedCallback(Team team)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.ScrumMasterUpdatedCallbackResult(team);
            });
        }

        //IOutsourcingServiceCallbacks
        public void SendNotificationToCEO(Notification notification, IOutsourcingServiceCallback outsourcingCallbackChannel)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.SendNotificationToCEOResult(notification);
            });
        }
    }
}