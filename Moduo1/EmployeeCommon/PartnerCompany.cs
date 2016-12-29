using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommon
{
    [DataContract]
    public class PartnerCompany
    {
        private string name;

        public PartnerCompany() 
        {
        }

        [Key]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
