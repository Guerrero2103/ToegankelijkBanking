using BankApp_Models;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace BankApp_WPF
{
    public partial class RegistratiePagina : Window
    {
        public RegistratiePagina()
        {
            InitializeComponent();
            LandBox.Text = "België";
            this.KeyDown += Window_KeyDown;
            this.Focusable = true;
            this.Focus();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
            {
                e.Handled = true;
                StartPagina startPagina = new StartPagina();
                startPagina.Show();
                this.Close();
            }
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
                    // Controleer of e-mailadres al bestaat
                    bool bestaatAl = db.Gebruikers.Any(g => g.Email == EmailBox.Text.Trim());
                    if (bestaatAl)
                    {
                        MessageBox.Show("Er bestaat al een account met dit e-mailadres.",
                                        "Registratie mislukt",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                        return;
                    }

                    // Nieuwe gebruiker aanmaken
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
                        RolId = 1 // standaard klant
                    };

                    db.Gebruikers.Add(gebruiker);
                    db.SaveChanges();

                    // Automatisch een zichtrekening aanmaken
                    var nieuweRekening = new Rekening
                    {
                        Iban = "BE" + DateTime.Now.Ticks.ToString().Substring(0, 10),
                        Type = RekeningType.Zicht,
                        Saldo = 0.0m,
                        GebruikerId = gebruiker.Id
                    };

                    db.Rekeningen.Add(nieuweRekening);
                    db.SaveChanges();

                    // Automatisch een kaart aanmaken voor de nieuwe gebruiker
                    string kaartNummer = GenereerUniekKaartNummer(db);
                    var nieuweKaart = new Kaart
                    {
                        KaartNummer = kaartNummer,
                        Status = KaartStatus.Actief,
                        GebruikerId = gebruiker.Id
                    };

                    db.Kaarten.Add(nieuweKaart);
                    db.SaveChanges();
                }

                MessageBox.Show(
                    "✅ Registratie geslaagd!\n\n" +
                    "Uw account is succesvol aangemaakt.\n" +
                    "Uw IBAN en kaartnummer worden automatisch toegewezen.\n\n" +
                    "U wordt nu doorgestuurd naar de login pagina.",
                    "Succes",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                MaakVeldenLeeg();

                LoginPagina loginPagina = new LoginPagina();
                loginPagina.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Er trad een fout op bij het registreren:\n{ex.Message}",
                                "Databasefout",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void BtnAccessibility_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "🔊 Toegankelijkheidsfuncties\n\n" +
                "AccessBank biedt de volgende toegankelijkheidsfuncties:\n\n" +
                "• Groot lettertype en hoog contrast\n" +
                "• Spraakfeedback en schermlezer ondersteuning\n" +
                "• Toetsenbordnavigatie (Tab en pijltjestoetsen)\n" +
                "• Zoomfunctie (100% - 200%)\n" +
                "• Duidelijke visuele feedback\n\n" +
                "Deze instellingen kunnen na registratie aangepast worden in uw profiel.",
                "Toegankelijkheid",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // --- VALIDATIE EN HELPERS ---

        private string ValideerFormulier()
        {
            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrWhiteSpace(NaamBox.Text))
                sb.AppendLine("• Naam is verplicht.");
            else if (NaamBox.Text.Length < 2)
                sb.AppendLine("• Naam moet minimaal 2 tekens bevatten.");

            if (!IsGeldigEmail(EmailBox.Text))
                sb.AppendLine("• Voer een geldig e-mailadres in.");

            if (WachtwoordBox.Password.Length < 8)
                sb.AppendLine("• Wachtwoord moet minimaal 8 tekens bevatten.");
            else if (!HeeftHoofdletterEnCijfer(WachtwoordBox.Password))
                sb.AppendLine("• Wachtwoord moet minimaal 1 hoofdletter en 1 cijfer bevatten.");

            if (WachtwoordBox.Password != BevestigBox.Password)
                sb.AppendLine("• Wachtwoorden komen niet overeen.");

            if (string.IsNullOrWhiteSpace(TelefoonBox.Text))
                sb.AppendLine("• Telefoonnummer is verplicht.");
            else if (!Regex.IsMatch(TelefoonBox.Text.Replace(" ", "").Replace("+", ""), @"^\d+$"))
                sb.AppendLine("• Telefoonnummer mag alleen cijfers bevatten (+ en spaties zijn toegestaan).");

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
            else if (!Regex.IsMatch(PostcodeBox.Text, @"^\d{4}$"))
                sb.AppendLine("• Postcode moet 4 cijfers bevatten.");

            if (string.IsNullOrWhiteSpace(GemeenteBox.Text))
                sb.AppendLine("• Gemeente is verplicht.");

            if (string.IsNullOrWhiteSpace(LandBox.Text))
                sb.AppendLine("• Land is verplicht.");

            return sb.ToString();
        }

        private bool IsGeldigEmail(string email) =>
            !string.IsNullOrWhiteSpace(email) &&
            Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);

        private bool IsOuderDan18(DateTime geboortedatum)
        {
            int leeftijd = DateTime.Now.Year - geboortedatum.Year;
            if (geboortedatum.Date > DateTime.Now.AddYears(-leeftijd))
                leeftijd--;
            return leeftijd >= 18;
        }

        private bool HeeftHoofdletterEnCijfer(string wachtwoord) =>
            Regex.IsMatch(wachtwoord, @"[A-Z]") && Regex.IsMatch(wachtwoord, @"\d");

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

        // Genereer uniek kaartnummer (formaat: XXXX-XXXX-XXXX-XXXX)
        private string GenereerKaartNummer()
        {
            // Gebruik DateTime.Ticks als seed voor betere randomisatie
            Random random = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
            string kaartNummer = "";

            // Genereer 4 groepen van 4 cijfers
            for (int i = 0; i < 4; i++)
            {
                if (i > 0) kaartNummer += "-";
                kaartNummer += random.Next(1000, 10000).ToString();
            }

            return kaartNummer;
        }

        // Genereer uniek kaartnummer en controleer of het al bestaat
        private string GenereerUniekKaartNummer(AppDbContext db)
        {
            string kaartNummer;
            int maxPogingen = 100; // Maximaal 100 pogingen om uniek nummer te vinden
            int poging = 0;

            do
            {
                kaartNummer = GenereerKaartNummer();
                poging++;

                // Controleer of kaartnummer al bestaat
                bool bestaatAl = db.Kaarten.Any(k => k.KaartNummer == kaartNummer);
                if (!bestaatAl)
                {
                    return kaartNummer;
                }
            } while (poging < maxPogingen);

            // Als na 100 pogingen nog geen uniek nummer gevonden, voeg timestamp toe
            return GenereerKaartNummer() + "-" + DateTime.Now.Ticks.ToString().Substring(Math.Max(0, DateTime.Now.Ticks.ToString().Length - 4));
        }
    }
}

