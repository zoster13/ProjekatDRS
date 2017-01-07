using ClientCommon;
using System;
using System.ServiceModel;
using ClientCommon.Data;
using System.Collections.Generic;

namespace Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class Publisher : ICallbackMethods
    {
        private static Publisher instance;
        private static List<ICallbackMethods> subscribers;
        private static ICallbackMethods callbackMethod;

        public Publisher()
        {
            if (instance == null)
            {
                subscribers = new List<ICallbackMethods>();
                instance = this;
            }
        }

        public static Publisher Instance
        {
            get
            {
                if (instance == null)
                    instance = new Publisher();

                return instance;
            }
            set
            {
                if (instance == null)
                    instance = value;
            }
        }

        //Properties

        //Methods
        public void LogInCallback(Employee employee)
        {
            callbackMethod = OperationContext.Current.GetCallbackChannel<ICallbackMethods>();
            subscribers.Add(callbackMethod);

            PublishLogInChanges(employee);
        }

        public  void LogOutCallback(Employee employee)
        {
            //callbackMethod = OperationContext.Current.GetCallbackChannel<ICallbackMethods>();
            //subscribers.Add(callbackMethod);

            PublishLogOutChanges(employee);
        }

        public void PublishLogInChanges(Employee employee)
        {
            foreach (ICallbackMethods sub in subscribers)
            {
                try
                {
                    if (((IClientChannel)sub).State == CommunicationState.Opened)
                    {
                        sub.LogInCallback(employee);
                    }
                    else
                    {
                        //subscribers.Remove(sub);
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        public void PublishLogOutChanges(Employee employee)
        {
            foreach (ICallbackMethods sub in subscribers)
            {
                try
                {
                    if (((IClientChannel)sub).State == CommunicationState.Opened)
                    {
                        sub.LogOutCallback(employee);
                    }
                    else
                    {
                        //subscribers.Remove(sub);
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        public void TeamAddedCallback(Team team, bool flag)
        {
            foreach (ICallbackMethods sub in subscribers)
            {
                try
                {
                    if (((IClientChannel)sub).State == CommunicationState.Opened)
                    {
                        sub.TeamAddedCallback(team, flag);
                    }
                    else
                    {
                        //subscribers.Remove(sub);
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        public void TypeChangeCallback(Team team, EmployeeType newType)
        {
            throw new NotImplementedException();
        }
    }
}
