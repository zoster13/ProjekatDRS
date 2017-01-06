using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommon
{
    [DataContract]
    public enum EmployeeType
    {
        [EnumMember]
        CEO = 0,
        [EnumMember]
        PO,
        [EnumMember]
        HR,
        [EnumMember]
        SM

    }

    public static class Extensions
    {
        public static string TypeToString(this EmployeeType em)
        {
            string retVal = string.Empty;
            switch(em)
            {
                case EmployeeType.CEO:
                    retVal = "Chief Executive Officer";
                    break;
                case EmployeeType.HR:
                    retVal = "Human Resources";
                    break;
                case EmployeeType.PO:
                    retVal = "Product Owner";
                    break;
                case EmployeeType.SM:
                    retVal = "Scrum Master";
                    break;
            }
            return retVal;
        }

        public static EmployeeType StringToType(this string em)
        {
            switch(em)
            {
                case "Chief Executive Officer":
                    return EmployeeType.CEO;
                case "Human Resources":
                    return EmployeeType.HR;
                case "Product Owner":
                    return EmployeeType.PO;
                case "Scrum Master":
                    return EmployeeType.SM;

                default: return EmployeeType.CEO; // mozda promeniti
            }
        }

    }


}
