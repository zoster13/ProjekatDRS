using EmployeeCommon.Data;
using ICommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiringCompany.DatabaseAccess
{
    public interface IHiringCompanyDB
    {
        bool AddNewEmployee(Employee employee);

        Employee GetEmployee(string username);

        bool AddNewPartnerCompany(PartnerCompany company);

        bool AddNewProject(Project project);

        List<Project> ProjectsInDevelopment();

        List<Project> ProjectsForSendingToOutsC();

        List<Project> ProjectsForCeoApproval();

        List<PartnerCompany> PartnerCompanies();

        List<Employee> AllEmployees();

        object AllEmployees_lock { get; set; }

        object Projects_lock { get; set; }

        object PartnerCompanies_lock { get; set; }

        bool EditEmployeeData(string username, string name, string surname, string email, string password);

        bool EditWorkingHours(string username, int beginH, int beginM, int endH, int endM);

        bool EditEmployeeType(string username, EmployeeType type);

        bool ProjectApprovedCEOFieldChange(Project p);

        bool CloseProjectFieldChange(string projectName);

        bool ResponseForProjectRequestFieldsChange(string outsourcingCompanyName, ProjectCommon p);

        bool SendUserStoriesToHiringCompanyFieldsChange(List<UserStory> tempUserStories, string projectName);

        bool SendClosedUserStoryFieldChange(string projectName, string title);

        bool ClearEmployeeNotifs(string username);

        List<string> GetPartnerCompaniesNames();
    }
}
