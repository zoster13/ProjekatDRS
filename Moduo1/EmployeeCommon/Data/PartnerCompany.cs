using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommon.Data
{
    [DataContract]
    public class PartnerCompany
    {
        private string name;

        public PartnerCompany() 
        {
        }

        public PartnerCompany(string name)
        {
            this.name = name;
        }

        [Key]
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
