using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Speech.Synthesis;
)

namespace BankApp_WPF
{
    public partial class LoginPagina : Window
    {
        private bool isDarkMode = true;
        private SpeechSynthesizer synthesizer;

        public LoginPagina()
        {
            InitializeComponent();
            InitializeSpeechSynthesizer();
            this.Loaded += (s, e) => TxtEmail.Focus();
        }

        private void InitializeSpeechSynthesizer()
        {
            try
            {
                synthesizer = new SpeechSynthesizer();
                synthesizer.SetOutputToDefaultAudioDevice();
                synthesizer.Rate = 0;
                synthesizer.Volume = 100;
            }
            catch
            {
                synthesizer = null;
            }
        }

        private void Announce(string message)
        {
            try
            {
                synthesizer?.SpeakAsyncCancelAll();
                synthesizer?.SpeakAsync(message);
            }
            catch { }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Announce("Terug naar startpagina");
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
                BtnTheme.Content = "☀";
                Announce("Donker thema geactiveerd");
                SetTextColors(Brushes.White);
            }
            else
            {
                this.Background = Brushes.White;
                BtnTheme.Content = "🌙";
                Announce("Licht thema geactiveerd");
                SetTextColors(Brushes.Black);
            }
        }

        private void SetTextColors(Brush kleur)
        {
            foreach (var element in LogicalTreeHelper.GetChildren(this))
            {
                if (element is TextBlock tb && tb != TxtError)
                    tb.Foreground = kleur;
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = TxtEmail.Text.Trim();
            string password = TxtPassword.Password;
            TxtError.Visibility = Visibility.Collapsed;

            if (string.IsNullOrEmpty(email))
            {
                ShowError("Gelieve je e-mailadres in te vullen.");
                Announce("E-mailadres is verplicht.");
                TxtEmail.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Gelieve je wachtwoord in te vullen.");
                Announce("Wachtwoord is verplicht.");
                TxtPassword.Focus();
                return;
            }

            if (!IsValidEmail(email))
            {
                ShowError("Ongeldig e-mailadres. Gebruik: gebruiker@voorbeeld.be");
                Announce("Ongeldig e-mailadres.");
                TxtEmail.Focus();
                return;
            }

            if (ValidateLogin(email, password))
            {
                Announce("Inloggen succesvol.");
                MessageBox.Show($"Welkom terug, {email}!",
                                "Login Succesvol",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            else
            {
                ShowError("Onjuiste e-mail of wachtwoord. Probeer opnieuw.");
                Announce("Onjuiste inloggegevens.");
                TxtPassword.Clear();
                TxtEmail.Focus();
            }
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

        private void LinkForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            Announce("Wachtwoord vergeten");
            MessageBox.Show(
                "Wachtwoord Reset\n\n" +
                "Stuur een e-mail naar support@accessbank.be met:\n" +
                "- Je geregistreerd e-mailadres\n" +
                "- Je klantnummer\n\n" +
                "Je ontvangt binnen 24 uur een resetlink.",
                "Wachtwoord vergeten",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
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

        private bool ValidateLogin(string email, string password)
        {
            return email == "test@accessbank.be" && password == "test123";
        }

        protected override void OnClosed(EventArgs e)
        {
            synthesizer?.Dispose();
            base.OnClosed(e);
        }
    }
}
