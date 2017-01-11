using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ICommon
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IOutsourcingServiceCallback))]
    public interface IOutsourcingService
    {
        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void AskForPartnership(string hiringCompanyName);

        [OperationContract(IsOneWay=true)]
        void SendProjectToOutsourcingCompany(string hiringCompanyName, ProjectCommon p);
    }
}
