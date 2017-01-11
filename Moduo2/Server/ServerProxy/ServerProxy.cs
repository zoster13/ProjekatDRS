using ICommon;
using System;
using System.ServiceModel;

namespace Server.ServerProxy
{
    public class ServerProxy : ChannelFactory<IHiringService>, IHiringService, IDisposable
    {
        IHiringService factory;

        public ServerProxy(NetTcpBinding binding, string address) : base(binding,address)
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
    }
}
