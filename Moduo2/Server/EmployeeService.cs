﻿using ClientCommon;
using ClientCommon.Data;
using Server.Access;
using System;
using System.Collections.Generic;

namespace Server
{
    public class EmployeeService : IEmployeeService
    {
        public List<Employee> GetAllEmployees()
        {
            return EmployeeServiceDatabase.Instance.GetAllEmployees();
        }

        public Employee LogIn(string username, string password)
        {
            throw new NotImplementedException();
        }

        public bool LogOut(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
