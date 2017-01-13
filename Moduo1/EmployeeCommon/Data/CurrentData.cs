using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommon.Data
{
    [DataContract]
    public class CurrentData
    {
        // imacemo vise listi posto sa List<object> nece raditi..
        // fakticki znaci da cemo svim klijentima slati sve podatke sa svakim notify-emm O.o
        private List<Employee> employeesData;
        private List<Employee> allEmployeesData;
        private List<Project> projectsForApprovalData;
        private List<Project> projectsForSendingData;
        private List<Project> projectsInDevelopmentData;
        private List<String> namesOfCompaniesData;
        private List<PartnerCompany> companiesData;
        
        public CurrentData()
        {
            employeesData = new List<Employee>();
            allEmployeesData = new List<Employee>();
            projectsForApprovalData = new List<Project>();
            projectsForSendingData = new List<Project>();
            projectsInDevelopmentData = new List<Project>();
            namesOfCompaniesData = new List<string>();
            companiesData = new List<PartnerCompany>();
        }

       
        [DataMember]
        public List<Employee> EmployeesData
        {
            get
            {
                return employeesData;
            }
            set
            {
                employeesData = value;
            }
        }

        [DataMember]
        public List<Employee> AllEmployeesData
        {
            get
            {
                return allEmployeesData;
            }
            set
            {
                allEmployeesData = value;
            }
        }

        [DataMember]
        public List<Project> ProjectsForApprovalData
        {
            get { return projectsForApprovalData; }
            set { projectsForApprovalData = value; }
        }

        [DataMember]
        public List<Project> ProjectsForSendingData 
        {
            get { return projectsForSendingData; }
            set { projectsForSendingData = value; }
        }

        [DataMember]
        public List<Project> ProjectsInDevelopmentData 
        {
            get { return projectsInDevelopmentData; }
            set { projectsInDevelopmentData = value; }
        }

        [DataMember]
        public List<String> NamesOfCompaniesData 
        {
            get { return namesOfCompaniesData; }
            set { namesOfCompaniesData = value; }
        }

        [DataMember]
        public List<PartnerCompany> CompaniesData 
        {
            get { return companiesData; }
            set { companiesData = value; }
        }

    }
}

