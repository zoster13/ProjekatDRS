﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace HiringCompany
{
    public class OutsorcingCompProxy : DuplexClientBase<ICommon.IOutsourcingService>, ICommon.IOutsourcingService, IDisposable
    {
        ICommon.IOutsourcingService factory;

        public OutsorcingCompProxy(InstanceContext callbackContext,Binding binding,EndpointAddress remoteAddress):
            base(callbackContext, binding, remoteAddress)
        {
            factory = this.ChannelFactory.CreateChannel();
        }

        public void AskForPartnership(string hiringCompanyName)
        {
            try
            {
                factory.AskForPartnership(hiringCompanyName);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public void Dispose() // srediti ovde da se izbrise sve iz baze sto treba ako se klijent ugasi neregularno
        {

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