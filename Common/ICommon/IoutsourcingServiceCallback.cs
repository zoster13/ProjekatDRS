using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ICommon
{
    public interface IOutsourcingServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void AskForPartnershipCallback(bool accepted);
    }
}
