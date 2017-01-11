using System.ServiceModel;

namespace ICommon
{
    [ServiceContract]
    public interface IOutsourcingService
    {
        [OperationContract]
        void AskForPartnership(string hiringCompanyName);

        [OperationContract]
        void SendProjectToOutsourcingCompany(string hiringCompanyName, ProjectCommon p);
    }
}
