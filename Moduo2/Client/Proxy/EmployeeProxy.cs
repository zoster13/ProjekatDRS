﻿using System;
using System.Collections.Generic;
using System.ServiceModel;
using ClientCommon;
using ClientCommon.Data;
using ICommon;

namespace Client
{
    public class EmployeeProxy : DuplexChannelFactory<IEmployeeService>, IEmployeeService, IDisposable
    {
        private IEmployeeService factory;
        private CallbackMethods callbackMethods = null;

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

        public void AddTeamAndTL(Team team, Employee teamLeader)
        {
            try
            {
                factory.AddTeamAndTL(team, teamLeader);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to AddTeamAndTL: {0}", e.Message);
            }
        }

        public void AddTeamAndUpdateDeveloperToTL(Team team, Employee developer)
        {
            try
            {
                factory.AddTeamAndUpdateDeveloperToTL(team, developer);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to AddTeamAndUpdateDeveloperToTL: {0}", e.Message);
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

        //public void UpdateEmployeeFunctionAndTeam(Employee employee, string newTeamName)
        //{
        //    try
        //    {
        //        factory.UpdateEmployeeFunctionAndTeam(employee, newTeamName);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Error while trying to UpdateEmployeeFunctionAndTeam: {0}", e.Message);
        //    }
        //}

        public void AddUserStory(UserStory userStory, string projectName)
        {
                factory.AddUserStory(userStory, projectName);
            
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

        //Responses to Hiring Company
        public void ResponseToPartnershipRequest(bool accepted, string hiringCompanyName)
        {
            try
            {
                factory.ResponseToPartnershipRequest(accepted, hiringCompanyName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to ResponseToPartnershipRequest: {0}", e.Message);
            }
        }

        public void ResponseToProjectRequest(bool accepted, Project project)
        {
            try
            {
                factory.ResponseToProjectRequest(accepted, project);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to ResponseToProjectRequest: {0}", e.Message);
            }
        }

        public List<Project> GetAllProjects()
        {
            try
            {
                return factory.GetAllProjects();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to GetAllProjects: {0}", e.Message);
                return null;
            }
        }

        public List<UserStory> GetUserStories()
        {
            try
            {
                return factory.GetUserStories();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to GetUserStories: {0}", e.Message);
                return null;
            }
        }

        public List<Task> GetAllTasks()
        {
            try
            {
                return factory.GetAllTasks();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to GetAllTasks: {0}", e.Message);
                return null;
            }
        }
    }
}
