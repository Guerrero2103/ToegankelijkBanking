using BankApp_Models;

namespace BankApp_WPF
{
    // Compatibiliteitslaag tussen oudere "SessionManager" calls en huidige "UserSession"
    public static class SessionManager
    {
        public static void Login(Gebruiker gebruiker)
        {
            UserSession.IngelogdeGebruiker = gebruiker;
        }

        public static void Logout()
        {
            UserSession.LogUit();
        }

        // Backwards compatibility: sommige plaatsen gebruiken LogUit()
        public static void LogUit()
        {
            UserSession.LogUit();
        }

        public static bool IsLoggedIn => UserSession.IsIngelogd;

        public static Gebruiker? CurrentUser => UserSession.IngelogdeGebruiker;
    }
}