using EmployeeCommon.Data;
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

        object DbAccess_lock { get; set; }

        object AllEmployees_lock { get; set; }

        object Projects_lock { get; set; }

        object PartnerCompanies_lock { get; set; }
    }
}
