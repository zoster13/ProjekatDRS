using System;
using System.Collections.Generic;
using System.ServiceModel;
using ClientCommon;
using ClientCommon.Data;
using ICommon;

namespace Client
{
    public class EmployeeProxy : DuplexChannelFactory<IEmployeeService>, IEmployeeService, IDisposable
    {
        IEmployeeService factory;
        CallbackMethods callbackMethods = null;

        public EmployeeProxy(NetTcpBinding binding, EndpointAddress epAddress, CallbackMethods callbackMethods) : base(callbackMethods, binding, epAddress)
        {
            this.callbackMethods = callbackMethods;
            factory = this.CreateChannel();
        }

        public void LogIn(string email, string password)
        {
            try
            {
                factory.LogIn(email, password);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to LogIn: {0}", e.Message);
            }
        }

        public void LogOut(Employee employee)
        {
            try
            {
                factory.LogOut(employee);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to LogOut: {0}", e.Message);
            }
        }

        public List<Employee> GetAllOnlineEmployees()
        {
            try
            {
                return factory.GetAllOnlineEmployees();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to GetAllEmployees: {0}", e.Message);
                return null;
            }
        }

        public List<Employee> GetAllEmployees()
        {
            try
            {
                return factory.GetAllEmployees();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to GetAllEmployees: {0}", e.Message);
                return null;
            }
        }

        public List<Team> GetAllTeams()
        {
            try
            {
                return factory.GetAllTeams();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to GetAllTeams: {0}", e.Message);
                return null;
            }
        }

        public List<HiringCompany> GetAllHiringCompanies()
        {
            try
            {
                return factory.GetAllHiringCompanies();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to AddTeam: {0}", e.Message);
                return null;
            }
        }

        public void AddTeam(Team team)
        {
            try
            {
                factory.AddTeam(team);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to AddTeam: {0}", e.Message);
            }
        }

        public void EditEmployeeData(Employee employee)
        {
            try
            {
                factory.EditEmployeeData(employee);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to EditEmployeeData: {0}", e.Message);
            }
        }

        public void ProjectTeamAssign(Project project)
        {
            try
            {
                factory.ProjectTeamAssign(project);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to ProjectTeamAssign: {0}", e.Message);
            }
        }

        public void AddEmployee(Employee employee)
        {
            try
            {
                factory.AddEmployee(employee);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to AddEmployee: {0}", e.Message);
            }
        }

        public void UpdateEmployeeFunctionAndTeam(Employee employee, string newTeamName)
        {
            try
            {
                factory.UpdateEmployeeFunctionAndTeam(employee, newTeamName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to UpdateEmployeeFunctionAndTeam: {0}", e.Message);
            }
        }

        public void AddUserStory(UserStory userStory)
        {
            try
            {
                factory.AddUserStory(userStory);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to AddUserStory: {0}", e.Message);
            }
        }

        //IOutsorucingService
        public void ResponseToPartnershipRequest(bool accepted, string companyName)
        {
            try
            {
                factory.ResponseToPartnershipRequest(accepted, companyName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to ResponseToPartnershipRequest: {0}", e.Message);
            }
        }

        public void SendUserStories(List<UserStoryCommon> userStories, string projectName)
        {
            try
            {
                factory.SendUserStories(userStories, projectName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to SendUserStories: {0}", e.Message);
            }
        }

        public void AddTask(Task task)
        {
            try
            {
                factory.AddTask(task);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to AddTask: {0}", e.Message);
            }
        }

        public void ReleaseUserStory(UserStory userStory)
        {
            try
            {
                factory.ReleaseUserStory(userStory);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to ReleaseUserStory: {0}", e.Message);
            }
        }

        public void TaskClaimed(Task task)
        {
            try
            {
                factory.TaskClaimed(task);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to TaskClaimed: {0}", e.Message);
            }
        }

        public void TaskCompleted(Task task)
        {
            try
            {
                factory.TaskCompleted(task);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to TaskCompleted: {0}", e.Message);
            }
        }


        public void ResponseToProjectRequest(bool accepted, string projectName, string hiringCompanyName)
        {
            try
            {
                factory.ResponseToProjectRequest(accepted,projectName,hiringCompanyName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to ResponseToProjectRequest: {0}", e.Message);
            }
        }
    }
}
