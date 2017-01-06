using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;
using EmployeeCommon;

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

        public void SignOut(string username)
        {
            try
            {
                factory.SignOut(username);
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

        public void ChangeEmployeeData(string username, string name, string surname, string email, string password)
        {
            try
            {
                factory.ChangeEmployeeData(username, name, surname, email, password);
            }
            catch (Exception)
            {


            }
        }

        public void SetWorkingHours(string username, int beginH, int beginM, int endH, int endM)
        {
            try
            {
                factory.SetWorkingHours(username, beginH, beginM, endH, endM);
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

        public void AddNewEmployee(Employee e)
        {
            try
            {
                factory.AddNewEmployee(e);
            }
            catch (Exception)
            {


            }
        }

        public void ChangeEmployeeType(string username,EmployeeType type)
        {
            try
            {

                factory.ChangeEmployeeType(username, type);
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
