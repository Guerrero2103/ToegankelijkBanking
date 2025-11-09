using BankApp_Models;

namespace BankApp_WPF
{
    public static class SessionManager
    {
        public static Gebruiker? CurrentUser { get; set; }

        public static bool IsLoggedIn => CurrentUser != null;

        public static void Logout()
        {
            CurrentUser = null;
        }

        public static void Login(Gebruiker gebruiker)
        {
            CurrentUser = gebruiker;
        }
    }
}