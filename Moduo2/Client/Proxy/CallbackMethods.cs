using ClientCommon;
using System;
using ClientCommon.Data;
using System.ServiceModel;
using System.Threading;
using ICommon;
using System.Collections.Generic;

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

        public void LogInCallback(Employee employee, bool loggedIn)
        {
             App.Current.Dispatcher.Invoke((Action)delegate
             {
                 mainWindow.LogInCallbackResult(employee, loggedIn);
             });
        }

        public void LogOutCallback(Employee employee)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.LogOutCallbackResult(employee);
            });
        }

        public void TeamAddedCallback(Team team)
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
        

        public void NotifyJustMe(Employee employee)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.TypeChangeCallbackResult(employee);
            });
        }

        public void ScrumMasterAddedCallback(Employee employee, Team team)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.ScrumMasterUpdatedCallbackResult(team);
            });
        }

        //IOutsourcingServiceCallbacks
        public void SendNotificationToCEO(Notification notification)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.SendNotificationToCEOResult(notification);
            });
        }

        public void ProjectTeamAssignCallback(Project project)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.ProjectTeamAssignCallbackResult(project);
            });
        }

        public void ReceiveUserStoriesCallback(List<UserStoryCommon> commUserStories, string projectName)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.ReceiveUserStoriesCallbackResult(commUserStories, projectName);
            });
        }

        public void ReleaseUserStoryCallback(List<Task> tasks)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.ReleaseUserStoryCallbackResult(tasks);
            });
        }

        public void TaskClaimedCallback(Task task)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.TaskClaimedCallbackResult(task);
            });
        }

        public void TaskCompletedCallback(Task task)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.TaskCompletedCallbackResult(task);
            });
        }

        public void AddTeamAndTLCallback(Team team, Employee teamLeader)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.AddTeamAndTLCallbackResult(team, teamLeader);
            });
        }

        public void ResponseToPartnershipRequestCallback(HiringCompany hiringCompany)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.ResponseToPartnershipRequestCallbackResult(hiringCompany);
            });
        }

        public void SendProjectToTeamMembers(Project project)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.SendProjectToTeamMembersResult(project);
            });
        }
    }
}