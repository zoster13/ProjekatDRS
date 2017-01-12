using System.ServiceModel;
using System.Collections.Generic;

namespace ICommon
{
    [ServiceContract]
    public interface IHiringService
    {
        [OperationContract]
        void ResponseForPartnershipRequest(bool accepted, string outsourcingCompName);

        [OperationContract]
        void ResponseForProjectRequest(string outsourcingCompanyName, ProjectCommon p);

        [OperationContract]
        void SendUserStoriesToHiringCompany(List<UserStoryCommon> userStories, string projectName);
    }
}
