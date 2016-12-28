using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCommon
{
    public interface IEmployeeService
    {
        void LogIn(); // da li da se u ovom LogIn-u interno implementira i subscribe? Da korisnik dobija podatke koji su mu od znacaja dok je logovan
    }
}
