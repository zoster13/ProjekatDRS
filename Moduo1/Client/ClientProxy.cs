using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Client
{
    public class ClientProxy : DuplexClientBase<EmployeeCommon.IEmployeeService>, EmployeeCommon.IEmployeeService,IDisposable
    {
        EmployeeCommon.IEmployeeService factory;

        public ClientProxy(InstanceContext callbackContext, Binding binding, EndpointAddress remoteAddress) :
         base(callbackContext, binding, remoteAddress)
        {
            factory = this.ChannelFactory.CreateChannel();           
        }



        public bool SignIn(string username,string password)
        {
            bool retval = false;
            try
            {
                retval=factory.SignIn(username,password);  //Ne moze se pozvati SyncData jer je zapucao ovde i ceka odgovor!
            }
            catch (Exception)
            {
                
               
            }
            return retval;
        }

        public void SignOut()
        {
            try
            {

            }
            catch (Exception)
            {


            }
        }

        public void ListOnlineEmployees()
        {
            try
            {

            }
            catch (Exception)
            {


            }
        }

        public void ListOutsorcingCompanies()
        {
            try
            {

            }
            catch (Exception)
            {


            }
        }

        public void ChangeEmployeeData()
        {
            try
            {

            }
            catch (Exception)
            {


            }
        }

        public void SetWorkingHours()
        {
            try
            {

            }
            catch (Exception)
            {


            }
        }

        public void AskForPartnership()
        {
            try
            {

            }
            catch (Exception)
            {


            }
        }

        public void AddNewEmployee()
        {
            try
            {

            }
            catch (Exception)
            {


            }
        }

        public void ChangeEmployeeType()
        {
            try
            {

            }
            catch (Exception)
            {


            }
        }

        public void ProjectOverview()
        {
            try
            {

            }
            catch (Exception)
            {


            }
        }

        public void CreateNewProject()
        {
            try
            {

            }
            catch (Exception)
            {


            }
        }

        public void Dispose()
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
