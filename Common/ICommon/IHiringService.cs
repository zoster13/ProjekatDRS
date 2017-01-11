using System.ServiceModel;

namespace ICommon
{
    [ServiceContract]
    public interface IHiringService
    {
        [OperationContract]
        void ResponseForPartnershipRequest(bool accepted, string outsourcingCompName);

        [OperationContract]
        void ResponseForProjectRequest(string outsourcingCompanyName, ProjectCommon p);
    }
}
