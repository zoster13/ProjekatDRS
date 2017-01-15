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
        }

        #region ICallbackMethods
        public void LogInCallback(Employee employee, bool loggedIn)
        {
            if (loggedIn)
            {
                if (OperationContext.Current != null)
                {
                    employee.Channel = OperationContext.Current.GetCallbackChannel<ICallbackMethods>();
                    employeeChannels.Add(employee.Email, employee.Channel);
                    PublishLogInChanges(employee, loggedIn);
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (OperationContext.Current != null)
                {
                    ICallbackMethods callback = OperationContext.Current.GetCallbackChannel<ICallbackMethods>();

                    if (callback != null)
                    {
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
                else
                {
                    return;
                }
            }
        }

        public void PublishLogInChanges(Employee employee, bool loggedIn)
        {
            if (employeeChannels.Values.Count > 0)
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
        }

        public void LogOutCallback(Employee employee)
        {
            if (employeeChannels.Values.Count > 0)
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
        }

        public void TeamAddedCallback(Team team)
        {
            if (employeeChannels.Values.Count > 0)
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
        }

        public void AddTeamAndTLCallback(Team team, Employee teamLeader)
        {
            if (employeeChannels.Values.Count > 0)
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
            else
            {
                return;
            }
        }

        public void TypeChangeCallback(Team team, EmployeeType newType)
        {
        }

        public void EditEmployeeCallback(Employee employee)
        {
            if (employeeChannels.Values.Count > 0)
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
        }

        public void AddEmployeeCallback(Employee employee)
        {
            if (employeeChannels.Values.Count > 0)
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
        }

        public void NotifyJustMe(Employee employee)
        {
            Employee onlineEmployee = InternalDatabase.Instance.OnlineEmployees.FirstOrDefault(e => e.Email.Equals(employee.Email));

            if (onlineEmployee != null)
            {
                if (onlineEmployee.Channel != null)
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
        }

        public void ScrumMasterAddedCallback(Employee employee, Team team)
        {
            if (employeeChannels.Values.Count > 0)
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
        }

        public void ProjectTeamAssignCallback(Project project)
        {
            Employee teamLeader = InternalDatabase.Instance.OnlineEmployees.FirstOrDefault(e => e.Email.Equals(project.Team.TeamLeaderEmail));

            if (teamLeader != null)        
            {
                if (teamLeader.Channel != null)
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
        }

        public void ReleaseUserStoryCallback(List<Task> tasks)
        {
            foreach (Employee employee in InternalDatabase.Instance.OnlineEmployees)
            {
                if (employee.Team.Name.Equals(tasks[0].UserStory.Project.Team.Name) && employee.Type == EmployeeType.DEVELOPER)
                {
                    if (employee.Channel != null)
                    {
                        try
                        {
                            if (((IClientChannel)employee.Channel).State == CommunicationState.Opened)
                            {
                                employee.Channel.ReleaseUserStoryCallback(tasks);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: {0}", e.Message);
                        }
                    }
                }
            }
        }

        public void TaskClaimedCallback(Task task)
        {
            if (employeeChannels.Values.Count > 0)
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
        }

        public void TaskCompletedCallback(Task task)
        {
            if (employeeChannels.Values.Count > 0)
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
        }

        //Delegiranje notifikacije od hiring kompanije
        public void SendNotificationToCEO(Notification notification)
        {
            //Sacauvaj notifikaciju u bazu
            EmployeeServiceDatabase.Instance.AddNotification(notification);

            //Obavjesti CEO ako je online da je dobio notif
            foreach (Employee emp in InternalDatabase.Instance.OnlineEmployees)
            {
                if (emp.Type.Equals(EmployeeType.CEO))
                {
                    if (emp.Channel != null)
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
                    }
                    break;
                }
            }
        }

        public void ReceiveUserStoriesCallback(List<UserStoryCommon> commUserStories, string projectName)
        {
            Project proj = EmployeeServiceDatabase.Instance.UpdateUserStoriesStatus(commUserStories, projectName);
            
            //Posalji UserStorije TL
            Employee teamLeader = InternalDatabase.Instance.OnlineEmployees.FirstOrDefault(e => e.Email.Equals(proj.Team.TeamLeaderEmail));

            if (teamLeader != null)
            {
                if (teamLeader.Channel != null)
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

            SendProjectToTeamMembers(proj);
        }

        public void ResponseToPartnershipRequestCallback(HiringCompany hiringCompany)
        {
            if (employeeChannels.Values.Count > 0)
            {
                foreach (ICallbackMethods sub in employeeChannels.Values)
                {
                    try
                    {
                        if (((IClientChannel)sub).State == CommunicationState.Opened)
                        {
                            sub.ResponseToPartnershipRequestCallback(hiringCompany);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: {0}", e.Message);
                    }
                }
            }
        }

        public void SendProjectToTeamMembers(Project project)
        {
            //Posalji online zaposlenima tog tima taj projekta
            foreach (Employee employee in InternalDatabase.Instance.OnlineEmployees)
            {
                if (employee.Team.Name.Equals(project.Team.Name) && employee.Type == EmployeeType.DEVELOPER)
                {
                    if (employee.Channel != null)
                    {
                        try
                        {
                            if (((IClientChannel)employee.Channel).State == CommunicationState.Opened)
                            {
                                employee.Channel.SendProjectToTeamMembers(project);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: {0}", e.Message);
                        }
                    }
                }
            }
        }
        
        public void NotifySMForUserStoryProgressCallback(string scrumMasterEmail, string userStoryName)
        {
            if(employeeChannels.Values.Count > 0)
            {
                try
                {
                    if (((IClientChannel)employeeChannels[scrumMasterEmail]).State == CommunicationState.Opened)
                    {
                        employeeChannels[scrumMasterEmail].NotifySMForUserStoryProgressCallback(scrumMasterEmail, userStoryName);
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
