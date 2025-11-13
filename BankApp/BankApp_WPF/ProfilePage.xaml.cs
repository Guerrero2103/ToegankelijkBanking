using BankApp_Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace BankApp_WPF
{
    public partial class ProfilePage : Window
    {
        private Gebruiker? _gebruiker;

        public ProfilePage()
        {
            InitializeComponent();
            Loaded += ProfilePage_Loaded;
            this.KeyDown += Window_KeyDown;
            this.Focusable = true;
            this.Focus();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
            {
                e.Handled = true;
                HoofdPagina hoofdPagina = new HoofdPagina();
                hoofdPagina.Show();
                this.Close();

            }
        }

        private void ProfilePage_Loaded(object sender, RoutedEventArgs e)
        {
            // ✅ Controleer of er iemand is ingelogd
            if (UserSession.IngelogdeGebruiker == null)
            {
                MessageBox.Show("Geen gebruiker actief. Log eerst in.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
                return;
            }

            _gebruiker = UserSession.IngelogdeGebruiker;

            // 🔄 Vul alle velden met de huidige gegevens
            EmailTextBox.Text = _gebruiker.Email;
            PhoneTextBox.Text = _gebruiker.Telefoonnummer ?? "";

            using (var context = new AppDbContext())
            {
                var gebruikerMetRekeningen = context.Gebruikers
                    .Include(g => g.Rekeningen)
                    .FirstOrDefault(g => g.Id == _gebruiker.Id);

                if (gebruikerMetRekeningen?.Rekeningen != null && gebruikerMetRekeningen.Rekeningen.Any())
                {
                    IbanTextBox.Text = gebruikerMetRekeningen.Rekeningen.First().Iban;
                }
                else
                {
                    IbanTextBox.Text = "Geen rekening gevonden";
                }
            }

            BirthdatePicker.SelectedDate = _gebruiker.Geboortedatum;
            StraatTextBox.Text = _gebruiker.Straatnaam ?? "";
            HuisnummerTextBox.Text = _gebruiker.Huisnummer ?? "";
            BusTextBox.Text = _gebruiker.Bus ?? "";
            PostcodeTextBox.Text = _gebruiker.Postcode ?? "";
            GemeenteTextBox.Text = _gebruiker.Gemeente ?? "";

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            HoofdPagina hoofd = new HoofdPagina();
            hoofd.Show();
            Close();
        }

        private void VoiceToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (VoiceToggleButton.Content.ToString() == "🔊")
            {
                VoiceToggleButton.Content = "🔇";
                MessageBox.Show("Spraak feedback uitgeschakeld", "Voice");
            }
            else
            {
                VoiceToggleButton.Content = "🔊";
                MessageBox.Show("Spraak feedback ingeschakeld", "Voice");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_gebruiker == null)
            {
                MessageBox.Show("Geen actieve gebruiker gevonden.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var context = new AppDbContext())
                {
                    var gebruikerInDb = context.Gebruikers.FirstOrDefault(g => g.Id == _gebruiker.Id);

                    if (gebruikerInDb == null)
                    {
                        MessageBox.Show("Gebruiker niet gevonden in de database.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // ✅ Validatie
                    if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
                    {
                        MessageBox.Show("E-mail mag niet leeg zijn.", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!DateTime.TryParse(BirthdatePicker.Text, out DateTime geboortedatum))
                    {
                        MessageBox.Show("Ongeldige geboortedatum.", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // ✅ Wachtwoordoptie (optioneel aanpassen)
                    if (!string.IsNullOrWhiteSpace(PasswordBox.Password))
                    {
                        if (PasswordBox.Password != ConfirmPasswordBox.Password)
                        {
                            MessageBox.Show("De wachtwoorden komen niet overeen.", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        gebruikerInDb.WachtwoordHash = HashWachtwoord(PasswordBox.Password);
                    }


                    // 📝 Update velden
                    gebruikerInDb.Email = EmailTextBox.Text.Trim();
                    gebruikerInDb.Telefoonnummer = PhoneTextBox.Text.Trim();
                    gebruikerInDb.Geboortedatum = geboortedatum;
                    gebruikerInDb.Straatnaam = StraatTextBox.Text.Trim();
                    gebruikerInDb.Huisnummer = HuisnummerTextBox.Text.Trim();
                    gebruikerInDb.Bus = string.IsNullOrWhiteSpace(BusTextBox.Text) ? null : BusTextBox.Text.Trim();
                    gebruikerInDb.Postcode = PostcodeTextBox.Text.Trim();
                    gebruikerInDb.Gemeente = GemeenteTextBox.Text.Trim();

                    // 💾 Opslaan in DB
                    context.SaveChanges();

                    // 🔄 Bijwerken in UserSession
                    UserSession.IngelogdeGebruiker = gebruikerInDb;
                    _gebruiker = gebruikerInDb;

                    MessageBox.Show("Gegevens succesvol opgeslagen!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Er is een fout opgetreden bij het opslaan: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        // 🚫 DeleteButton: voorlopig geen functionaliteit
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_gebruiker == null)
            {
                MessageBox.Show("Geen actieve gebruiker gevonden.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var bevestiging = MessageBox.Show(
                "Weet je zeker dat je je profiel wilt verwijderen?\n" +
                "Je account wordt gedeactiveerd, maar je gegevens blijven bewaard voor administratie.",
                "Bevestig verwijdering",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (bevestiging != MessageBoxResult.Yes)
                return;

            try
            {
                using (var context = new AppDbContext())
                {
                    var gebruikerInDb = context.Gebruikers.FirstOrDefault(g => g.Id == _gebruiker.Id);

                    if (gebruikerInDb == null)
                    {
                        MessageBox.Show("Gebruiker niet gevonden in database.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // ⚙️ Soft delete
                    gebruikerInDb.IsActief = false;
                    context.SaveChanges();
                }

                // 🧹 Clear sessie
                UserSession.IngelogdeGebruiker = null;

                MessageBox.Show("Je account is gedeactiveerd. Bedankt om onze bank te gebruiken!", "Account gedeactiveerd", MessageBoxButton.OK, MessageBoxImage.Information);

                // 🔄 Terug naar loginpagina
                LoginPagina login = new LoginPagina();
                login.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Er is een fout opgetreden bij het verwijderen: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
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
