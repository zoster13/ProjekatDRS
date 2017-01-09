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
            employee.Channel = OperationContext.Current.GetCallbackChannel<ICallbackMethods>();
            subscribers.Add(employee.Channel);
            
            PublishLogInChanges(employee);
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
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        public void LogOutCallback(Employee employee)
        {
            subscribers.Remove(employee.Channel);
            employee.Channel = null;
            
            PublishLogOutChanges(employee);
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
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        public void TypeChangeCallback(Team team, EmployeeType newType)
        {
            try
            {
                if (((IClientChannel)team.TeamLeader.Channel).State == CommunicationState.Opened)
                {
                    team.TeamLeader.Channel.TypeChangeCallback(team,newType);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }

        public void EditEmployeeCallback(Employee employee)
        {
            foreach (ICallbackMethods sub in subscribers)
            {
                try
                {
                    if (((IClientChannel)sub).State == CommunicationState.Opened)
                    {
                        sub.EditEmployeeCallback(employee);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }

        public void AddEmployeeCallback(Employee employee)
        {
            foreach (ICallbackMethods sub in subscribers)
            {
                try
                {
                    if (((IClientChannel)sub).State == CommunicationState.Opened)
                    {
                        sub.AddEmployeeCallback(employee);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }
        }
    }
}
