using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using BankApp_Models;

namespace BankApp_WPF
{
    public partial class RegistratiePagina : Window
    {
        public RegistratiePagina()
        {
            InitializeComponent();
            LandBox.Text = "België";
        }

        private void RegistreerBtn_Click(object sender, RoutedEventArgs e)
        {
            string fouten = ValideerFormulier();

            if (!string.IsNullOrEmpty(fouten))
            {
                MessageBox.Show(fouten, "Fouten bij registratie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    bool bestaatAl = db.Gebruikers.Any(g => g.Email == EmailBox.Text.Trim());
                    if (bestaatAl)
                    {
                        MessageBox.Show("Er bestaat al een account met dit e-mailadres.", "Registratie mislukt", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var gebruiker = new Gebruiker
                    {
                        Email = EmailBox.Text.Trim(),
                        WachtwoordHash = HashWachtwoord(WachtwoordBox.Password),
                        Telefoonnummer = TelefoonBox.Text.Trim(),
                        Geboortedatum = GeboortePicker.SelectedDate.Value,
                        Straatnaam = StraatBox.Text.Trim(),
                        Huisnummer = HuisnrBox.Text.Trim(),
                        Bus = string.IsNullOrWhiteSpace(BusBox.Text) ? null : BusBox.Text.Trim(),
                        Postcode = PostcodeBox.Text.Trim(),
                        Gemeente = GemeenteBox.Text.Trim(),
                        Land = LandBox.Text.Trim(),
                        RolId = 1
                    };

                    db.Gebruikers.Add(gebruiker);
                    db.SaveChanges();

                    var nieuweRekening = new Rekening
                    {
                        Iban = "BE" + DateTime.Now.Ticks.ToString().Substring(0, 10),
                        Type = RekeningType.Zicht,
                        Saldo = 0.0m,
                        GebruikerId = gebruiker.Id
                    };

                    db.Rekeningen.Add(nieuweRekening);
                    db.SaveChanges();
                }

                MessageBox.Show("Registratie geslaagd!\n\nUw account is succesvol aangemaakt.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                MaakVeldenLeeg();

                LoginPagina loginPagina = new LoginPagina();
                loginPagina.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Er trad een fout op:\n{ex.Message}", "Databasefout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAccessibility_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Toegankelijkheidsfuncties\n\nAccessBank biedt:\n• Groot lettertype\n• Spraakfeedback\n• Toetsenbordnavigatie", "Toegankelijkheid", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private string ValideerFormulier()
        {
            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrWhiteSpace(NaamBox.Text))
                sb.AppendLine("• Naam is verplicht.");

            if (!IsGeldigEmail(EmailBox.Text))
                sb.AppendLine("• Voer een geldig e-mailadres in.");

            if (WachtwoordBox.Password.Length < 8)
                sb.AppendLine("• Wachtwoord moet minimaal 8 tekens bevatten.");

            if (WachtwoordBox.Password != BevestigBox.Password)
                sb.AppendLine("• Wachtwoorden komen niet overeen.");

            if (string.IsNullOrWhiteSpace(TelefoonBox.Text))
                sb.AppendLine("• Telefoonnummer is verplicht.");

            if (GeboortePicker.SelectedDate == null)
                sb.AppendLine("• Geboortedatum is verplicht.");
            else if (!IsOuderDan18(GeboortePicker.SelectedDate.Value))
                sb.AppendLine("• Je moet minimaal 18 jaar oud zijn.");

            if (string.IsNullOrWhiteSpace(StraatBox.Text))
                sb.AppendLine("• Straatnaam is verplicht.");

            if (string.IsNullOrWhiteSpace(HuisnrBox.Text))
                sb.AppendLine("• Huisnummer is verplicht.");

            if (string.IsNullOrWhiteSpace(PostcodeBox.Text))
                sb.AppendLine("• Postcode is verplicht.");

            if (string.IsNullOrWhiteSpace(GemeenteBox.Text))
                sb.AppendLine("• Gemeente is verplicht.");

            if (string.IsNullOrWhiteSpace(LandBox.Text))
                sb.AppendLine("• Land is verplicht.");

            return sb.ToString();
        }

        private bool IsGeldigEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
        }

        private bool IsOuderDan18(DateTime geboortedatum)
        {
            int leeftijd = DateTime.Now.Year - geboortedatum.Year;
            if (geboortedatum.Date > DateTime.Now.AddYears(-leeftijd))
                leeftijd--;
            return leeftijd >= 18;
        }

        private void MaakVeldenLeeg()
        {
            NaamBox.Clear();
            EmailBox.Clear();
            WachtwoordBox.Clear();
            BevestigBox.Clear();
            TelefoonBox.Clear();
            StraatBox.Clear();
            HuisnrBox.Clear();
            BusBox.Clear();
            PostcodeBox.Clear();
            GemeenteBox.Clear();
            LandBox.Clear();
            GeboortePicker.SelectedDate = null;
        }

        private string HashWachtwoord(string wachtwoord)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(wachtwoord);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

    }
}

