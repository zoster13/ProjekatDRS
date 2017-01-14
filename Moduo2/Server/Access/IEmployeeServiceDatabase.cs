using ClientCommon.Data;
using ClientCommon.TempStructure;
using ICommon;
using System.Collections.Generic;

namespace Server.Access
{
    public interface IEmployeeServiceDatabase
    {
        bool AddEmployee(Employee employee);
        
        Employee GetEmployee(string email);

        bool AddTeam(Team team);

        Team GetTeam(string email);

        bool UpdateEmployeeFunctionAndTeam(Employee employee, string newTeamName);

        bool UpdateEmployee(Employee employee);

        void AddNotification(Notification notification);

        //izmjestanje
        Team UpdateTeamScrumMaster(string teamName, string scrumMasterEmail);

        List<Employee> GetAllEmployees();

        List<Team> GetAllTeams();

        List<HiringCompany> GetAllHiringCompanies();

        List<Project> GetAllProjects();

        Team UpdateProjectsTeam(string projectName, string teamName);

        bool AddUserStory(UserStory userStory, string projectName);

        bool AddTask(Task task);

        UserStory ReleaseUserStory(UserStory userStory);

        Task TaskClaimed(Task task);

        TaskAndUserStoryCompletedFlag TaskCompleted(Task task);

        bool UpdateProjectStatus(string projectName);

        void AddHiringCompany(HiringCompany hiringCompany);

        void AddProject(Project project);

        List<UserStory> GetUserStories();

        List<Task> GetAllTasks();

        Project UpdateUserStoriesStatus(List<UserStoryCommon> commUserStories, string projectName);

    }
}
