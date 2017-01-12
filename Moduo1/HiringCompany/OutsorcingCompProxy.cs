using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;
using ICommon;

namespace HiringCompany
{
    public class OutsorcingCompProxy : ChannelFactory<IOutsourcingService>, ICommon.IOutsourcingService, IDisposable
    {
        private IOutsourcingService factory;

        public OutsorcingCompProxy(NetTcpBinding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
            factory = this.CreateChannel();
        }

        public void AskForPartnership(string hiringCompanyName)
        {
            try
            {
                factory.AskForPartnership(hiringCompanyName);
            }
            catch (Exception) 
            {
                // srediti ako nihov server otkaze
                throw;
            }
        }



        public void SendEvaluatedUserstoriesToOutsourcingCompany(List<UserStoryCommon> userStories, string projectName)
        {
            throw new NotImplementedException();
        }

        public void SendProjectToOutsourcingCompany(string hiringCompanyName, ProjectCommon p)
        {
            try
            {
                factory.SendProjectToOutsourcingCompany(hiringCompanyName, p);
            }
            catch (Exception) 
            {
                // srediti ako nihov server otkaze
                throw;
            }
        }


        // srediti ovde da se izbrise sve iz baze sto treba ako se klijent ugasi neregularno
        public void Dispose()
        {

            if (factory != null)
            {
                factory = null;
            }
            try
            {
                this.Close();
            }
            catch (CommunicationException)
            {
                this.Abort();
            }
            catch (TimeoutException)
            {
                this.Abort();
            }
        }
    }
}
