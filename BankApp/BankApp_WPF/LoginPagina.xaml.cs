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
            }
            else
            {
                this.Background = Brushes.White;
                BtnTheme.Content = new TextBlock { Text = "🌙", Foreground = Brushes.White, FontSize = 24 };
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
                ShowError("Ongeldig email formaat.");
                TxtEmail.Focus();
                return;
            }

            var gebruiker = ValidateLogin(email, password);

            if (gebruiker != null)
            {
                SessionManager.Login(gebruiker);

                MessageBox.Show($"Welkom {gebruiker.Email}!", "Login Succesvol",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                HoofdPagina hoofd = new HoofdPagina();
                hoofd.Show();
                this.Close();
            }
            else
            {
                ShowError("Onjuiste email of wachtwoord.");
                TxtPassword.Clear();
                TxtEmail.Focus();
            }
        }

        private void LinkForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Wachtwoord Reset: Stuur email naar support@accessbank.be",
                "Wachtwoord Vergeten", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private Gebruiker? ValidateLogin(string email, string password)
        {
            using (var context = new AppDbContext())
            {
                // Zoek gebruiker met dit e-mailadres
                var gebruiker = context.Gebruikers
                    .Include(g => g.Rol) // optioneel: rol mee laden
                    .FirstOrDefault(g => g.Email.ToLower() == email.ToLower());

                if (gebruiker == null)
                    return false;

                // 🔐 Hash het ingevoerde wachtwoord
                var ingevoerdeHash = HashWachtwoord(password);


                // 🧩 Debug info in popup (alleen tijdelijk!)
                /*string debugInfo =
                    $"=== LOGIN DEBUG ===\n" +
                    $"Email: {email}\n\n" +
                    $"Wachtwoord: {password}\n\n" +
                    $"Ingevoerde hash:\n{ingevoerdeHash}\n\n" +
                    $"Database hash:\n{gebruiker.WachtwoordHash}\n\n" +
                    $"Hash match? {(ingevoerdeHash == gebruiker.WachtwoordHash)}";

                MessageBox.Show(debugInfo, "Login Debug Info", MessageBoxButton.OK, MessageBoxImage.Information);*/

                if (gebruiker.WachtwoordHash == ingevoerdeHash)
                {
                    //Zet de ingelogde gebruiker in de sessie
                    UserSession.IngelogdeGebruiker = gebruiker;
                    return true;
                }

                return false;

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