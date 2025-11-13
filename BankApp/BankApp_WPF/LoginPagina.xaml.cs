using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BankApp_Models;
using Microsoft.EntityFrameworkCore;

namespace BankApp_WPF
{
    public partial class LoginPagina : Window
    {
        private bool isDarkMode = true;

        public LoginPagina()
        {
            InitializeComponent();


            this.Loaded += (s, e) => TxtEmail.Focus();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {

            StartPagina startPagina = new StartPagina();
            startPagina.Show();
            this.Close();
        }

        private void BtnTheme_Click(object sender, RoutedEventArgs e)
        {

            isDarkMode = !isDarkMode;

            if (isDarkMode)
            {
                this.Background = Brushes.Black;
                BtnTheme.Content = new TextBlock { Text = "☀", Foreground = Brushes.White, FontSize = 24 };
                BtnTheme.ToolTip = "Schakel naar licht thema";
            }
            else
            {
                this.Background = Brushes.White;
                BtnTheme.Content = new TextBlock { Text = "🌙", Foreground = Brushes.White, FontSize = 24 };
                BtnTheme.ToolTip = "Schakel naar donker thema";
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {

            string email = TxtEmail.Text.Trim();
            string password = TxtPassword.Password;

            TxtError.Visibility = Visibility.Collapsed;

            if (string.IsNullOrEmpty(email))
            {
                ShowError("Gelieve je email in te vullen.");
                TxtEmail.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Gelieve je wachtwoord in te vullen.");
                TxtPassword.Focus();
                return;
            }


            if (!IsValidEmail(email))
            {
                ShowError("Ongeldig email formaat. Gebruik: gebruiker@voorbeeld.be");
                TxtEmail.Focus();
                return;
            }


            var gebruiker = ValidateLogin(email, password);
            if (gebruiker != null)
            {
                MessageBox.Show("Login succesvol!", "Welkom", MessageBoxButton.OK, MessageBoxImage.Information);

                // Controleer rol en open juiste pagina
                // RolId 3 = Beheerder (Admin)
                if (gebruiker.RolId == 3)
                {
                    // Beheerder gaat naar AdminPagina
                    AdminPagina adminPagina = new AdminPagina();
                    adminPagina.Show();
                }
                else
                {
                    // Klant of Medewerker gaat naar HoofdPagina
                    HoofdPagina hoofd = new HoofdPagina();
                    hoofd.Show();
                }

                this.Close();
            }
            else
            {
                ShowError("Onjuiste email of wachtwoord. Probeer opnieuw.");
                TxtPassword.Clear();
                TxtEmail.Focus();
            }
        }

        private void LinkForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Wachtwoord Reset Aanvraag\n\n" +
                "Stuur een email naar support@accessbank.be met:\n" +
                "- Je geregistreerd email adres\n" +
                "- Je klantnummer\n\n" +
                "Je ontvangt binnen 24 uur een reset link.",
                "Wachtwoord Vergeten",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                BtnLogin_Click(sender, e);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Escape)
            {
                BtnBack_Click(sender, e);
            }
        }


        private void ShowError(string message)
        {
            TxtError.Text = message;
            TxtError.Visibility = Visibility.Visible;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Retourneert Gebruiker object als login succesvol is, anders null
        private Gebruiker ValidateLogin(string email, string password)
        {
            using (var context = new AppDbContext())
            {
                // Zoek gebruiker met dit e-mailadres
                var gebruiker = context.Gebruikers
                    .Include(g => g.Rol) // Laad rol mee voor rolcontrole
                    .FirstOrDefault(g => g.Email.ToLower() == email.ToLower());

                if (gebruiker == null)
                    return null;

                // Hash het ingevoerde wachtwoord
                var ingevoerdeHash = HashWachtwoord(password);

                // Vergelijk hashes - retourneer gebruiker als wachtwoord correct is
                if (gebruiker.WachtwoordHash == ingevoerdeHash)
                {
                    return gebruiker;
                }

                return null;
            }
        }

        private string HashWachtwoord(string wachtwoord)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(wachtwoord);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

    }
}