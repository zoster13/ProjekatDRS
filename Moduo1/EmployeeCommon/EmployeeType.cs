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
}
