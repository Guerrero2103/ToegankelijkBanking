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
            this.KeyDown += Window_KeyDown;
            this.Focusable = true;
            this.Focus();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z)
            {
                e.Handled = true;
                StartPagina startPagina = new StartPagina();
                startPagina.Show();
                this.Close();

            }
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
                // Gebruik UserSession in plaats van (niet-gevonden) SessionManager
                UserSession.IngelogdeGebruiker = gebruiker;

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
            using var context = new AppDbContext();
            var gebruiker = context.Gebruikers
                .Include(g => g.Rol)
                .FirstOrDefault(g => g.Email.ToLower() == email.ToLower());

            if (gebruiker == null)
                return null;

            var ingevoerdeHash = HashWachtwoord(password);

            if (gebruiker.WachtwoordHash == ingevoerdeHash)
            {
                return gebruiker;
            }

            return null;
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