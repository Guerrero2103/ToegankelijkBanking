using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankApp_Models;

namespace BankApp_WPF
{
    public static class UserSession
    {
        public static Gebruiker? IngelogdeGebruiker { get; set; }

        public static void LogUit()
        {
            IngelogdeGebruiker = null;
        }

        public static bool IsIngelogd => IngelogdeGebruiker != null;
    }
}

