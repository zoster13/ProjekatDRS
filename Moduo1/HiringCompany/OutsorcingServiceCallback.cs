using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace HiringCompany
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class OutsorcingServiceCallback : ICommon.IOutsourcingServiceCallback
    {
        public void AskForPartnershipCallback(bool accepted)
        {
            Console.WriteLine("Komunikacija uspostavljena, partnerstvo uspostavljeno");
        }
    }
}
