using EmployeeCommon;
using HiringCompany.DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace HiringCompany.Services
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
      ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class EmployeeService : IEmployeeService
    {
        HiringCompanyDB hiringCompanyDB = HiringCompanyDB.Instance();

        System.Timers.Timer lateOnJobTimer = new System.Timers.Timer();

        public EmployeeService()
        {
            lateOnJobTimer.Elapsed += new ElapsedEventHandler(NotifyOnLate);
            lateOnJobTimer.Interval = 15000; // every five seconds
            lateOnJobTimer.Enabled = true;
        }

        private void NotifyOnLate(object sender, ElapsedEventArgs e)
        {
             string _senderEmailAddress = "mzftn123fakultet@gmail.com";
                string _senderPassword = "miljanazvezdana123";
            Console.WriteLine("alarm...");
            foreach (Employee em in hiringCompanyDB.AllEmployees) // slanje maila onima koji nisu online
            {
                if (!hiringCompanyDB.OnlineEmployees.Contains(em))
                {

                    var client = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new NetworkCredential(_senderEmailAddress, _senderPassword),
                        EnableSsl = true
                    };
                    client.Send(_senderEmailAddress, em.Email, "Obavestenje", "Zakasnili ste na posao!");


                    //MailAddress fromAddress = new MailAddress("mzftn123fakultet@gmail.com");
                    //MailAddress toAddress = new MailAddress(em.Email.Trim());
                    //const string subject = "test";
                    //const string body = "HEY, LISTEN!";

                    //DateTime current = DateTime.Now;
                    //DateTime workTimeEmployee = new DateTime(current.Year, current.Month, current.Day, em.StartHour, em.StartMinute, 0);
                    //TimeSpan timeDiff = current - workTimeEmployee;
                    //TimeSpan allowed = new TimeSpan(0, 15, 0);

                    //if (timeDiff > allowed)
                    //{
                    //    Console.WriteLine("slanje e maila...");
                    //    string _senderEmailAddress = "mzftn123fakultet@gmail.com";
                    //    string _senderPassword = "miljanazvezdana123";
                    //    MailMessage message = new MailMessage(_senderEmailAddress, em.Email, "proba", "kasnis");


                    //    try
                    //    {
                    //        //Create the msg object to be sent
                    //        MailMessage msg = new MailMessage();
                    //        //Add your email address to the recipients
                    //        msg.To.Add(em.Email);
                    //        //Configure the address we are sending the mail from
                    //        MailAddress address = new MailAddress(_senderEmailAddress);
                    //        msg.From = address;
                    //        msg.Subject = "anything";
                    //        msg.Body = "anything";

                    //        //Configure an SmtpClient to send the mail.            
                    //        SmtpClient client = new SmtpClient();
                    //        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    //        client.EnableSsl = false;
                    //        client.Host = "relay-hosting.secureserver.net";
                    //        client.Port = 25;

                    //        //Setup credentials to login to our sender email address ("UserName", "Password")
                    //        NetworkCredential credentials = new NetworkCredential(_senderEmailAddress, _senderPassword);
                    //        client.UseDefaultCredentials = true;
                    //        client.Credentials = credentials;

                    //        //Send the msg
                    //        client.Send(msg);

                    //        //Display some feedback to the user to let them know it was sent
                    //        // Label1.Text = "Your message was sent!";
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        //If the message failed at some point, let the user know
                    //        // Label1.Text = ex.ToString();
                    //        //"Your message failed to send, please try again."
                    //    }



                    //    //var client = new SmtpClient
                    //    //{
                    //    //    Host = "smtp.gmail.com",
                    //    //    Port = 587,
                    //    //    EnableSsl = true,
                    //    //    DeliveryMethod = SmtpDeliveryMethod.Network,
                    //    //    UseDefaultCredentials = false,
                    //    //    Credentials = new NetworkCredential(fromAddress.Address, _senderPassword),
                    //    //    //Timeout = 20000
                    //    //};

                    //    //using (var m = new MailMessage(fromAddress, toAddress)
                    //    //{
                    //    //    Subject = subject,
                    //    //    Body = body
                    //    //})
                    //    //{
                    //    //    client.Send(m);
                    //    //}

                    //    //client.Credentials = new NetworkCredential(_senderEmailAddress, _senderPassword);
                    //    //try
                    //    //{
                    //    //    client.Send(message);
                    //    //}
                    //    //catch (Exception ex)
                    //    //{
                    //    //    Console.WriteLine("Exception caught in CreateTestMessage1(): {0}",
                    //    //                ex.ToString());
                    //    //}

                    //    //try
                    //    //{
                    //    //    var client = new SmtpClient("smtp.gmail.com", 587)
                    //    //    {
                    //    //        Credentials = new NetworkCredential(_senderEmailAddress, _senderPassword),
                    //    //        EnableSsl = true
                    //    //    };
                    //    //    client.Send(_senderEmailAddress, em.Email, "Warning!", "You are late!");
                    //    //}
                    //    //catch (Exception ex)
                    //    //{
                    //    //    Console.WriteLine("Exception sending email." + Environment.NewLine + ex);
                    //    //}

                    
                }
            }
        }

        

        public bool SignIn(string username, string password)
        {
            Console.WriteLine("EmployeeService.LogIn() called ");

            Employee employee = hiringCompanyDB.GetEmployee(username);

            if (employee != null && password.Equals(employee.Password))
            {
                IEmployeeServiceCallback callback = OperationContext.Current.GetCallbackChannel<IEmployeeServiceCallback>();
                hiringCompanyDB.ConnectionChannels.Add(username, callback);

                lock (hiringCompanyDB.Employees_lock)
                {
                    hiringCompanyDB.OnlineEmployees.Add(employee);
                }
                CurrentData cData = new CurrentData();
                cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
                cData.AllEmployeesData = hiringCompanyDB.AllEmployees;
                cData.ProjectsForApprovalData = hiringCompanyDB.ProjectsForApproval;

                foreach (IEmployeeServiceCallback call in hiringCompanyDB.ConnectionChannels.Values)
                {
                    call.SyncData(cData);
                }

                // return employee;
            }
            else
            {
                return false;
            }

            return true;
        }

        public void SignOut(string username)
        {
            Employee employee = null; // namestiti...

            lock (hiringCompanyDB.Employees_lock)
            {
                // sacuvati podatke tog korisnika u bazi
                foreach (Employee e in hiringCompanyDB.OnlineEmployees)
                {
                    if (e.Username.Equals(username))
                    {
                        employee = e;
                        break;
                    }
                }

                hiringCompanyDB.OnlineEmployees.Remove(employee);
                hiringCompanyDB.ConnectionChannels.Remove(username);
            }
            CurrentData cData = new CurrentData();
            cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
            cData.AllEmployeesData = hiringCompanyDB.AllEmployees;
            cData.ProjectsForApprovalData = hiringCompanyDB.ProjectsForApproval;
            foreach (IEmployeeServiceCallback call in hiringCompanyDB.ConnectionChannels.Values)
            {
                call.SyncData(cData);
            }

        }

        public void ChangeEmployeeData(string username, string name, string surname, string email, string password)
        {
            try
            {
                var access = new AccessDB();
                foreach (Employee em in access.employees)
                {
                    if (em.Username == username)
                    {
                        if (name != "")
                        {
                            em.Name = name;
                        }
                        if (surname != "")
                        {
                            em.Surname = surname;
                        }
                        if (email != "")
                        {
                            em.Email = email;
                        }
                        if (password != "")
                        {
                            em.Password = password;
                        }

                        break;
                    }
                }
                access.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                    }
                }
            }

            lock (hiringCompanyDB.AllEmployees_lock)
            {
                foreach (Employee em in hiringCompanyDB.AllEmployees)
                {
                    if (em.Username == username)
                    {
                        if (name != "")
                        {
                            em.Name = name;
                        }
                        if (surname != "")
                        {
                            em.Surname = surname;
                        }
                        if (email != "")
                        {
                            em.Email = email;
                        }
                        if (password != "")
                        {
                            em.Password = password;
                        }
                        break;
                    }
                }
            }

            lock (hiringCompanyDB.Employees_lock)
            {
                foreach (Employee em in hiringCompanyDB.OnlineEmployees)
                {
                    if (em.Username == username)
                    {
                        if (name != "")
                        {
                            em.Name = name;
                        }
                        if (surname != "")
                        {
                            em.Surname = surname;
                        }
                        if (email != "")
                        {
                            em.Email = email;
                        }
                        if (password != "")
                        {
                            em.Password = password;
                        }
                        break;
                    }
                }
            }

            CurrentData cData = new CurrentData();
            cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
            cData.AllEmployeesData = hiringCompanyDB.AllEmployees;
            cData.ProjectsForApprovalData = hiringCompanyDB.ProjectsForApproval;

            foreach (IEmployeeServiceCallback call in hiringCompanyDB.ConnectionChannels.Values)
            {
                call.SyncData(cData);
            }
        }

        public void SetWorkingHours(string username, int beginH, int beginM, int endH, int endM)
        {
            try
            {
                var access = new AccessDB();
                foreach (Employee em in access.employees)
                {
                    if (em.Username == username)
                    {
                        em.StartHour = beginH;
                        em.StartMinute = beginM;
                        em.EndHour = endH;
                        em.EndMinute = endM;

                        break;
                    }
                }
                access.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                    }
                }
            }

            lock (hiringCompanyDB.Employees_lock)
            {
                foreach (Employee em in hiringCompanyDB.OnlineEmployees)
                {
                    if (em.Username == username)
                    {
                        em.StartHour = beginH;
                        em.StartMinute = beginM;
                        em.EndHour = endH;
                        em.EndMinute = endM;
                        break;
                    }
                }
            }

            lock (hiringCompanyDB.AllEmployees_lock)
            {
                foreach (Employee em in hiringCompanyDB.AllEmployees)
                {
                    if (em.Username == username)
                    {
                        em.StartHour = beginH;
                        em.StartMinute = beginM;
                        em.EndHour = endH;
                        em.EndMinute = endM;
                        break;
                    }
                }
            }

            CurrentData cData = new CurrentData();
            cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
            cData.AllEmployeesData = hiringCompanyDB.AllEmployees;
            cData.ProjectsForApprovalData = hiringCompanyDB.ProjectsForApproval;

            foreach (IEmployeeServiceCallback call in hiringCompanyDB.ConnectionChannels.Values)
            {
                call.SyncData(cData);
            }
        }

        public void AskForPartnership()
        {
            throw new NotImplementedException();
        }

        public void AddNewEmployee(Employee em)
        {
            hiringCompanyDB.AddNewEmployee(em);

            CurrentData cData = new CurrentData();
            cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
            cData.AllEmployeesData = hiringCompanyDB.AllEmployees;
            cData.ProjectsForApprovalData = hiringCompanyDB.ProjectsForApproval;

            foreach (IEmployeeServiceCallback call in hiringCompanyDB.ConnectionChannels.Values)
            {
                call.SyncData(cData);
            }
        }

        public void ChangeEmployeeType(string username, EmployeeType type)
        {
            try
            {
                var access = new AccessDB();
                foreach (Employee em in access.employees)
                {
                    if (em.Username == username)
                    {
                        em.Type = type;
                        break;
                    }
                }
                access.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                    }
                }
            }

            lock (hiringCompanyDB.Employees_lock)
            {
                foreach (Employee em in hiringCompanyDB.OnlineEmployees)
                {
                    if (em.Username == username)
                    {
                        em.Type = type;
                        break;
                    }
                }
            }

            lock (hiringCompanyDB.AllEmployees_lock)
            {
                foreach (Employee em in hiringCompanyDB.AllEmployees)
                {
                    if (em.Username == username)
                    {
                        em.Type = type;
                        break;
                    }
                }
            }

            CurrentData cData = new CurrentData();
            cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
            cData.AllEmployeesData = hiringCompanyDB.AllEmployees;
            cData.ProjectsForApprovalData = hiringCompanyDB.ProjectsForApproval;

            foreach (IEmployeeServiceCallback call in hiringCompanyDB.ConnectionChannels.Values)
            {
                call.SyncData(cData);
            }
        }

        public void ProjectOverview()
        {
            throw new NotImplementedException();
        }

        public void CreateNewProject(Project p)
        {
            try
            {
                var access = new AccessDB();
                foreach (Employee em in access.employees)
                {
                    if (em.Type == EmployeeType.CEO)
                    {
                        foreach (KeyValuePair<string, IEmployeeServiceCallback> pair in hiringCompanyDB.ConnectionChannels)
                        {
                            if (pair.Key.Equals(em.Username))
                            {
                                lock (hiringCompanyDB.ProjectsForApproval_lock)
                                {
                                    hiringCompanyDB.ProjectsForApproval.Add(p);
                                }

                                CurrentData cData = new CurrentData();
                                cData.ProjectsForApprovalData = hiringCompanyDB.ProjectsForApproval;
                                cData.AllEmployeesData = hiringCompanyDB.AllEmployees;
                                cData.EmployeesData = hiringCompanyDB.OnlineEmployees;
                                pair.Value.SyncData(cData);
                                //treba napraviti metodu koja notifikuje CEO da treba da potvrdi projekat
                            }
                        }
                        break;
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                    }
                }
            }
        }
    }
}
