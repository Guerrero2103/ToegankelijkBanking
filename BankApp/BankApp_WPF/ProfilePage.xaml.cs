using BankApp_Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;

namespace BankApp_WPF
{
    public partial class ProfilePage : Window
    {
        private Gebruiker? _gebruiker;

        public ProfilePage()
        {
            InitializeComponent();
            Loaded += ProfilePage_Loaded;
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
            AddressTextBox.Text =
                $"{_gebruiker.Straatnaam} {_gebruiker.Huisnummer}{(_gebruiker.Bus != null ? " " + _gebruiker.Bus : "")}, " +
                $"{_gebruiker.Postcode} {_gebruiker.Gemeente}";
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

        // 🚫 SaveButton: voorlopig geen functionaliteit
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aanpassingen opslaan is momenteel uitgeschakeld.",
                            "Niet beschikbaar",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        // 🚫 DeleteButton: voorlopig geen functionaliteit
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Profiel verwijderen is momenteel uitgeschakeld.",
                            "Niet beschikbaar",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }
    }
}
