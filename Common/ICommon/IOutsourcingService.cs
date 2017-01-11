using System.Collections.Generic;
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

        [OperationContract]
        void SendEvaluatedUserstoriesToOutsourcingCompany(List<UserStoryCommon> userStories, string projectName);
    }
}
