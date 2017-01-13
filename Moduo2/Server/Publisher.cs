using ClientCommon;
using System;
using System.ServiceModel;
using ClientCommon.Data;
using System.Collections.Generic;
using Server.Database;
using System.Linq;
using Server.Access;
using ICommon;

namespace Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class Publisher : ICallbackMethods
    {
        private static Publisher instance;
        private static Dictionary<string, ICallbackMethods> employeeChannels;
        private List<Employee> allEmployees;
        
        public Publisher()
        {
            if (instance == null)
            {
                allEmployees = new List<Employee>();
                employeeChannels = new Dictionary<string, ICallbackMethods>();
                instance = this;
            }
        }

        public static Publisher Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Publisher();
                }
                return instance;
            }
            set
            {
                if (instance == null)
                {
                    instance = value;
                }
            }
        }

        #region ICallbackMethods
        public void LogInCallback(Employee employee, bool loggedIn)
        {
            if (loggedIn)
            {
                employee.Channel = OperationContext.Current.GetCallbackChannel<ICallbackMethods>();
                employeeChannels.Add(employee.Email, employee.Channel);
                PublishLogInChanges(employee, loggedIn);
            }
            else
            {
                ICallbackMethods callback = OperationContext.Current.GetCallbackChannel<ICallbackMethods>();

                try
                {
                    if (((IClientChannel)callback).State == CommunicationState.Opened)
                    {
                        callback.LogInCallback(employee, loggedIn);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        public void PublishLogInChanges(Employee employee, bool loggedIn)
        {
            foreach (ICallbackMethods sub in employeeChannels.Values)
            {
                try
                {
                    if (((IClientChannel)sub).State == CommunicationState.Opened)
                    {
                        sub.LogInCallback(employee, loggedIn);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        public void LogOutCallback(Employee employee)
        {
            foreach (ICallbackMethods sub in employeeChannels.Values)
            {
                try
                {
                    if (((IClientChannel)sub).State == CommunicationState.Opened)
                    {
                        sub.LogOutCallback(employee);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }

            employeeChannels.Remove(employee.Email);
        }

        public void TeamAddedCallback(Team team)
        {
            foreach (ICallbackMethods sub in employeeChannels.Values)
            {
                try
                {
                    if (((IClientChannel)sub).State == CommunicationState.Opened)
                    {
                        sub.TeamAddedCallback(team);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        public void AddTeamAndTLCallback(Team team, Employee teamLeader)
        {
            foreach (ICallbackMethods sub in employeeChannels.Values)
            {
                try
                {
                    if (((IClientChannel)sub).State == CommunicationState.Opened)
                    {
                        sub.AddTeamAndTLCallback(team, teamLeader);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        public void TypeChangeCallback(Team team, EmployeeType newType)
        {
            //try
            //{
            //    if (((IClientChannel)team.TeamLeader.Channel).State == CommunicationState.Opened)
            //    {
            //        team.TeamLeader.Channel.TypeChangeCallback(team,newType);
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Error: {0}", e.Message);
            //}
        }

        public void EditEmployeeCallback(Employee employee)
        {
            foreach (ICallbackMethods sub in employeeChannels.Values)
            {
                try
                {
                    if (((IClientChannel)sub).State == CommunicationState.Opened)
                    {
                        sub.EditEmployeeCallback(employee);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        public void AddEmployeeCallback(Employee employee)
        {
            foreach (ICallbackMethods sub in employeeChannels.Values)
            {
                try
                {
                    if (((IClientChannel)sub).State == CommunicationState.Opened)
                    {
                        sub.AddEmployeeCallback(employee);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        public void NotifyJustMe(Employee employee)
        {
            Employee onlineEmployee = InternalDatabase.Instance.OnlineEmployees.FirstOrDefault(e => e.Email.Equals(employee.Email));

            if (onlineEmployee != null)
            {
                try
                {
                    if (((IClientChannel)onlineEmployee.Channel).State == CommunicationState.Opened)
                    {
                        onlineEmployee.Channel.NotifyJustMe(employee);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        public void ScrumMasterAddedCallback(Employee employee, Team team)
        {
            foreach (ICallbackMethods sub in employeeChannels.Values)
            {
                try
                {
                    if (((IClientChannel)sub).State == CommunicationState.Opened)
                    {
                        sub.ScrumMasterAddedCallback(employee, team);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        public void ProjectTeamAssignCallback(Project project)
        {
            Employee teamLeader = InternalDatabase.Instance.OnlineEmployees.FirstOrDefault(e => e.Email.Equals(project.Team.TeamLeaderEmail));

            if (teamLeader != null)        
            {
                try
                {
                    if (((IClientChannel)teamLeader.Channel).State == CommunicationState.Opened)
                    {
                        teamLeader.Channel.ProjectTeamAssignCallback(project);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        public void ReleaseUserStoryCallback(UserStory userStory)
        {
            foreach (ICallbackMethods sub in employeeChannels.Values)
            {
                try
                {
                    if (((IClientChannel)sub).State == CommunicationState.Opened)
                    {
                        sub.ReleaseUserStoryCallback(userStory);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        public void TaskClaimedCallback(Task task)
        {
            foreach (ICallbackMethods sub in employeeChannels.Values)
            {
                try
                {
                    if (((IClientChannel)sub).State == CommunicationState.Opened)
                    {
                        sub.TaskClaimedCallback(task);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        public void TaskCompletedCallback(Task task)
        {
            foreach (ICallbackMethods sub in employeeChannels.Values)
            {
                try
                {
                    if (((IClientChannel)sub).State == CommunicationState.Opened)
                    {
                        sub.TaskCompletedCallback(task);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        //Delegiranje notifikacije od hiring kompanije
        public void SendNotificationToCEO(Notification notification)
        {
            using (var access = new AccessDB())
            {
                allEmployees = access.Employees.ToList();
            }
            
            //Sacauvaj notifikaciju u bazu
            foreach (Employee emp in allEmployees)
            {
                if (emp.Type.Equals(EmployeeType.CEO))
                {
                    Employee ceo = EmployeeServiceDatabase.Instance.GetEmployee(emp.Email);
                    notification.Emoloyee = ceo;
                    EmployeeServiceDatabase.Instance.AddNotification(notification);
                }
            }

            //Obavjesti CEO ako je online da je dobio notif
            foreach (Employee emp in InternalDatabase.Instance.OnlineEmployees)
            {
                if (emp.Type.Equals(EmployeeType.CEO))
                {
                    try
                    {
                        if (((IClientChannel)emp.Channel).State == CommunicationState.Opened)
                        {
                            emp.Channel.SendNotificationToCEO(notification);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: {0}", e.Message);
                    }

                    break;
                }
            }
        }

        public void ReceiveUserStoriesCallback(List<UserStoryCommon> commUserStories, string projectName)
        {
            Project proj;

            using (var access = new AccessDB())
            {
                proj = access.Projects
                    .Include("UserStories")
                    .Include("Team")
                    .FirstOrDefault(p => p.Name.Equals(projectName));

                foreach (var userStory in commUserStories)
                {
                    UserStory usInDB = proj.UserStories.FirstOrDefault(us => us.Title.Equals(userStory.Title));

                    if(usInDB != null)
                    {
                        if(userStory.IsAccepted)
                        {
                            usInDB.AcceptStatus = AcceptStatus.ACCEPTED;
                        }
                        else
                        {
                            usInDB.AcceptStatus = AcceptStatus.DECLINED;
                        }
                    }

                    access.SaveChanges();
                }
            }
            
            Employee teamLeader = InternalDatabase.Instance.OnlineEmployees.FirstOrDefault(e => e.Email.Equals(proj.Team.TeamLeaderEmail));

            if (teamLeader != null)
            {
                try
                {
                    if (((IClientChannel)teamLeader.Channel).State == CommunicationState.Opened)
                    {
                        teamLeader.Channel.ReceiveUserStoriesCallback(commUserStories, projectName);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        #endregion ICallbackMethods
    }
}
