using ClientCommon;
using System;
using ClientCommon.Data;
using System.ServiceModel;
using System.Threading;
using ICommon;
using System.Collections.Generic;
using NUnit.Framework;

namespace Client
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    
    public class CallbackMethods : ICallbackMethods
    {
        public MainWindow mainWindow;

        public CallbackMethods()
        {
            if (System.Windows.Application.Current != null)
            {
                mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            }
            else
            {
                mainWindow = new MainWindow();
            }
        }
        
        public void EditEmployeeCallback(Employee employee)
        {
            if (App.Current != null)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    mainWindow.EditEmployeeDataCalbackResult(employee);
                });
            }
            else
            {
                mainWindow.EditEmployeeDataCalbackResult(employee);
            }
        }

        public void LogInCallback(Employee employee, bool loggedIn)
        {
            if (App.Current != null)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    mainWindow.LogInCallbackResult(employee, loggedIn);
                });
            }
            else
            {
                mainWindow.EditEmployeeDataCalbackResult(employee);
            }
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

        public void NotifySMForUserStoryProgressCallback(string scrumMasterEmail, string userStoryName)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.NotifySMForUserStoryProgressCallbackResult(userStoryName);
            });
        }
    }
}