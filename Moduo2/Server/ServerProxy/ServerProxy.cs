using ICommon;
using System;
using System.ServiceModel;
using System.Collections.Generic;

namespace Server.ServerProxy
{
    public class ServerProxy : ChannelFactory<IHiringService>, IHiringService, IDisposable
    {
        private IHiringService factory;

        public ServerProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public void ResponseForPartnershipRequest(bool accepted, string outsourcingCompName)
        {
            try
            {
                factory.ResponseForPartnershipRequest(accepted, outsourcingCompName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }

        public void ResponseForProjectRequest(string outsourcingCompanyName, ProjectCommon p)
        {
            try
            {
                factory.ResponseForProjectRequest(outsourcingCompanyName, p);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }

        public void SendClosedUserStory(string projectName, string title)
        {
            throw new NotImplementedException();
        }

        public void SendUserStoriesToHiringCompany(List<UserStoryCommon> userStories, string projectName)
        {
            try
            {
                factory.SendUserStoriesToHiringCompany(userStories, projectName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }
    }
}
