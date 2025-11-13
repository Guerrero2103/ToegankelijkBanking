using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BankApp_Models;
using Microsoft.EntityFrameworkCore;

namespace BankApp_WPF
{
    /// <summary>
    /// Interaction logic for AdminPagina.xaml
    /// </summary>
    public partial class AdminPagina : Window
    {
        // Property voor data binding van klanten
        private System.Collections.ObjectModel.ObservableCollection<Gebruiker> _klanten;
        public System.Collections.ObjectModel.ObservableCollection<Gebruiker> Klanten
        {
            get { return _klanten; }
            set { _klanten = value; }
        }

        public AdminPagina()
        {
            InitializeComponent();
            Klanten = new System.Collections.ObjectModel.ObservableCollection<Gebruiker>();
            LaadKlanten(); // Laad klanten bij opstarten
            LaadKaarten(); // Laad kaarten voor Kaarten tab
        }

        // Laad alle klanten uit de database
        private void LaadKlanten()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    // Haal alle gebruikers op met hun rollen en rekeningen
                    var gebruikers = context.Gebruikers
                        .Include(g => g.Rol)
                        .Include(g => g.Rekeningen)
                        .ToList();

                    // Leeg de huidige lijst en vul opnieuw
                    Klanten.Clear();
                    foreach (var gebruiker in gebruikers)
                    {
                        Klanten.Add(gebruiker);
                    }

                    // Update de ListBox
                    KlantenListBox.ItemsSource = Klanten;

                    // Update de teller in de UI
                    KlantenTellerTextBlock.Text = $"Klanten overzicht ({gebruikers.Count})";

                    Console.WriteLine($"✅ Aantal gebruikers geladen: {gebruikers.Count}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden klanten: {ex.Message}",
                    "Database Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Laad alle kaarten voor de Kaarten tab
        private void LaadKaarten()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    // Haal alle kaarten op met hun gebruikers
                    var kaarten = context.Kaarten
                        .Include(k => k.Gebruiker)
                        .ToList();

                    // Update de ListBox
                    KaartenListBox.ItemsSource = kaarten;

                    // Update de teller in de UI
                    KaartenTellerTextBlock.Text = $"Kaart beheer - Card Stop ({kaarten.Count})";

                    Console.WriteLine($"✅ Kaarten tab: {kaarten.Count} kaarten geladen");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden kaarten: {ex.Message}",
                    "Database Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        // ========

        //  KLANTEN TAB - Event Handlers 

        // Klant toevoegen of bijwerken
        private void BtnKlantOpslaan_Click(object sender, RoutedEventArgs e)
        {
            // Validatie: controleer of verplichte velden zijn ingevuld
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Email is verplicht!", "Fout",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (BirthDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Geboortedatum is verplicht!", "Fout",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new AppDbContext())
                {
                    // Controleer of het een update of nieuwe klant is
                    bool isUpdate = !string.IsNullOrEmpty(CustomerIdTextBox.Text);

                    if (isUpdate)
                    {
                        // BESTAANDE KLANT BIJWERKEN
                        int customerId = int.Parse(CustomerIdTextBox.Text);
                        var gebruiker = context.Gebruikers.Find(customerId);

                        if (gebruiker != null)
                        {
                            // Update gegevens
                            // Voornaam en Achternaam worden niet opgeslagen (alleen visueel in formulier)
                            gebruiker.Email = EmailTextBox.Text.Trim();
                            gebruiker.Telefoonnummer = PhoneNumberTextBox.Text.Trim();
                            gebruiker.Geboortedatum = BirthDatePicker.SelectedDate.Value;
                            gebruiker.Straatnaam = StreetNameTextBox.Text.Trim();
                            gebruiker.Huisnummer = HouseNumberTextBox.Text.Trim();
                            gebruiker.Bus = BusTextBox.Text.Trim();
                            gebruiker.Postcode = PostcodeTextBox.Text.Trim();
                            gebruiker.Gemeente = CityTextBox.Text.Trim();
                            gebruiker.Land = CountryTextBox.Text.Trim();

                            // Update rol op basis van ComboBox index
                            // Index 0 = Klant (Id 1)
                            // Index 1 = Medewerker (Id 2)
                            // Index 2 = Beheerder (Id 3)
                            if (RoleComboBox.SelectedIndex == 0)
                            {
                                gebruiker.RolId = 1; // Klant
                            }
                            else if (RoleComboBox.SelectedIndex == 1)
                            {
                                gebruiker.RolId = 2; // Medewerker
                            }
                            else if (RoleComboBox.SelectedIndex == 2)
                            {
                                gebruiker.RolId = 3; // Beheerder
                            }
                            else
                            {
                                gebruiker.RolId = 1; // Standaard Klant
                            }

                            // Update wachtwoord alleen als er een nieuw wachtwoord is ingevoerd
                            if (!string.IsNullOrEmpty(PasswordBox.Password))
                            {
                                gebruiker.WachtwoordHash = HashPassword(PasswordBox.Password);
                            }

                            // Sla wijzigingen op
                            context.SaveChanges();

                            MessageBox.Show($"Klant {customerId} bijgewerkt!\n\n" +
                                $"Email: {gebruiker.Email}",
                                "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Form leegmaken na bijwerken
                            ClearForm();
                            LaadKlanten(); // Herlaad klanten lijst
                        }
                        else
                        {
                            MessageBox.Show("Gebruiker niet gevonden in database!",
                                "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        // NIEUWE KLANT TOEVOEGEN
                        // Bepaal rol op basis van ComboBox index
                        // Index 0 = Klant (Id 1)
                        // Index 1 = Medewerker (Id 2)
                        // Index 2 = Beheerder (Id 3)
                        int rolId = 1; // Standaard Klant
                        if (RoleComboBox.SelectedIndex == 0)
                        {
                            rolId = 1; // Klant
                        }
                        else if (RoleComboBox.SelectedIndex == 1)
                        {
                            rolId = 2; // Medewerker
                        }
                        else if (RoleComboBox.SelectedIndex == 2)
                        {
                            rolId = 3; // Beheerder
                        }

                        // Hash het wachtwoord (simpel voor nu)
                        string passwordHash = HashPassword(PasswordBox.Password);

                        // Maak nieuwe gebruiker aan
                        var nieuweGebruiker = new Gebruiker
                        {
                            // Voornaam en Achternaam worden niet opgeslagen (alleen visueel in formulier)
                            Email = EmailTextBox.Text.Trim(),
                            WachtwoordHash = passwordHash,
                            Telefoonnummer = PhoneNumberTextBox.Text.Trim(),
                            Geboortedatum = BirthDatePicker.SelectedDate.Value,
                            Straatnaam = StreetNameTextBox.Text.Trim(),
                            Huisnummer = HouseNumberTextBox.Text.Trim(),
                            Bus = BusTextBox.Text.Trim(),
                            Postcode = PostcodeTextBox.Text.Trim(),
                            Gemeente = CityTextBox.Text.Trim(),
                            Land = CountryTextBox.Text.Trim(),
                            RolId = rolId
                        };

                        // Voeg toe aan database
                        context.Gebruikers.Add(nieuweGebruiker);
                        context.SaveChanges();

                        // Automatisch een zichtrekening aanmaken
                        var nieuweRekening = new Rekening
                        {
                            Iban = "BE" + DateTime.Now.Ticks.ToString().Substring(0, 10),
                            Type = RekeningType.Zicht,
                            Saldo = 0.0m,
                            GebruikerId = nieuweGebruiker.Id
                        };
                        context.Rekeningen.Add(nieuweRekening);
                        context.SaveChanges();

                        // Automatisch een kaart aanmaken voor de nieuwe gebruiker
                        string kaartNummer = GenereerUniekKaartNummer(context);
                        var nieuweKaart = new Kaart
                        {
                            KaartNummer = kaartNummer,
                            Status = KaartStatus.Actief,
                            GebruikerId = nieuweGebruiker.Id
                        };
                        context.Kaarten.Add(nieuweKaart);
                        context.SaveChanges();

                        MessageBox.Show($"Nieuwe klant toegevoegd!\n\n" +
                            $"Email: {EmailTextBox.Text}\n" +
                            $"ID: {nieuweGebruiker.Id}\n" +
                            $"IBAN: {nieuweRekening.Iban}\n" +
                            $"Kaartnummer: {nieuweKaart.KaartNummer}",
                            "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Form leegmaken na toevoegen
                        ClearForm();
                        LaadKlanten(); // Herlaad klanten lijst
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij opslaan: {ex.Message}",
                    "Database Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Password hashing functie met SHA256 (zelfde als LoginPagina)
        private string HashPassword(string password)
        {
            // Hash het wachtwoord met SHA256 (zelfde methode als in LoginPagina)
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
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



        // Bewerken - laad klant gegevens in formulier voor bewerken
        private void BtnBewerken_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int customerId = int.Parse(btn.Tag.ToString());

            try
            {
                using (var context = new AppDbContext())
                {
                    // Zoek gebruiker in database
                    var gebruiker = context.Gebruikers
                        .Include(g => g.Rol)
                        .FirstOrDefault(g => g.Id == customerId);

                    if (gebruiker != null)
                    {
                        // Vul formulier met gebruikersgegevens
                        CustomerIdTextBox.Text = gebruiker.Id.ToString();
                        // Voornaam en Achternaam zijn niet in database, leeg laten
                        FirstNameTextBox.Text = "";
                        LastNameTextBox.Text = "";
                        EmailTextBox.Text = gebruiker.Email;
                        PhoneNumberTextBox.Text = gebruiker.Telefoonnummer ?? "";
                        BirthDatePicker.SelectedDate = gebruiker.Geboortedatum;
                        StreetNameTextBox.Text = gebruiker.Straatnaam ?? "";
                        HouseNumberTextBox.Text = gebruiker.Huisnummer ?? "";
                        BusTextBox.Text = gebruiker.Bus ?? "";
                        PostcodeTextBox.Text = gebruiker.Postcode ?? "";
                        CityTextBox.Text = gebruiker.Gemeente ?? "";
                        CountryTextBox.Text = gebruiker.Land ?? "";

                        // Zet rol combobox op basis van RolId
                        // RolId 1 = Klant (Index 0)
                        // RolId 2 = Medewerker (Index 1)
                        // RolId 3 = Beheerder (Index 2)
                        if (gebruiker.RolId == 1)
                        {
                            RoleComboBox.SelectedIndex = 0; // Klant
                        }
                        else if (gebruiker.RolId == 2)
                        {
                            RoleComboBox.SelectedIndex = 1; // Medewerker
                        }
                        else if (gebruiker.RolId == 3)
                        {
                            RoleComboBox.SelectedIndex = 2; // Beheerder
                        }
                        else
                        {
                            RoleComboBox.SelectedIndex = 0; // Standaard Klant
                        }

                        // Verander knop tekst naar "Bijwerken"
                        SaveCustomerButton.Content = "✓ Klant bijwerken";

                        // Ga naar Klanten tab
                        // Zoek de TabControl en selecteer eerste tab (Klanten)
                        var mainGrid = this.Content as Grid;
                        if (mainGrid != null)
                        {
                            var tabControl = mainGrid.Children.OfType<TabControl>().FirstOrDefault();
                            if (tabControl != null)
                            {
                                tabControl.SelectedIndex = 0; // Eerste tab = Klanten
                            }
                        }

                        MessageBox.Show($"Klant {customerId} geladen voor bewerken.\n\nGa naar Klanten tab om te bewerken.",
                            "Klant Bewerken", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Gebruiker niet gevonden!",
                            "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden klant: {ex.Message}",
                    "Database Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Verwijderen - klant verwijderen
        private void BtnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int customerId = int.Parse(btn.Tag.ToString());

            // Bevestiging vragen
            var result = MessageBox.Show(
                $"Weet u zeker dat u klant {customerId} wilt verwijderen?\n\n" +
                "Dit verwijdert ook alle gekoppelde rekeningen, kaarten en afspraken!",
                "Bevestigen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new AppDbContext())
                    {
                        // Zoek gebruiker
                        var gebruiker = context.Gebruikers.Find(customerId);

                        if (gebruiker != null)
                        {
                            // Verwijder gebruiker (cascade delete verwijdert ook gerelateerde data)
                            context.Gebruikers.Remove(gebruiker);
                            context.SaveChanges();

                            MessageBox.Show($"Klant {customerId} is verwijderd!",
                                "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                            LaadKlanten(); // Herlaad klanten lijst
                        }
                        else
                        {
                            MessageBox.Show("Gebruiker niet gevonden!",
                                "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fout bij verwijderen: {ex.Message}",
                        "Database Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        // Annuleren knop - leeg formulier
        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        // Helper methode: form leegmaken
        private void ClearForm()
        {
            CustomerIdTextBox.Text = ""; // Leeg maken voor nieuwe klant
            EmailTextBox.Clear();
            PasswordBox.Clear();
            PhoneNumberTextBox.Clear();
            BirthDatePicker.SelectedDate = null;
            StreetNameTextBox.Clear();
            HouseNumberTextBox.Clear();
            BusTextBox.Clear();
            PostcodeTextBox.Clear();
            CityTextBox.Clear();
            CountryTextBox.Text = "België";
            RoleComboBox.SelectedIndex = -1;
            FirstNameTextBox.Clear();
            LastNameTextBox.Clear();

            // Verander knop tekst terug naar "Toevoegen"
            SaveCustomerButton.Content = "👤 Klant toevoegen";
        }



        //   KAARTEN TAB - Event Handlers ==========

        // Kaart actief maken
        private void BtnActiefMaken_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int cardId = int.Parse(btn.Tag.ToString());

            var result = MessageBox.Show(
                $"Kaart {cardId} actief maken?",
                "Bevestigen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new AppDbContext())
                    {
                        // Zoek kaart in database
                        var kaart = context.Kaarten.Find(cardId);

                        if (kaart != null)
                        {
                            // Update status naar Actief
                            kaart.Status = KaartStatus.Actief;
                            context.SaveChanges();

                            MessageBox.Show($"Kaart {cardId} is actief gemaakt!",
                                "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                            LaadKaarten(); // Herlaad kaarten lijst
                        }
                        else
                        {
                            MessageBox.Show("Kaart niet gevonden!",
                                "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fout bij actief maken kaart: {ex.Message}",
                        "Database Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //   Kaart bevriezen
        private void BtnBevriezen_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int cardId = int.Parse(btn.Tag.ToString());

            var result = MessageBox.Show(
                $"Kaart {cardId} bevriezen?",
                "Bevestigen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new AppDbContext())
                    {
                        // Zoek kaart in database
                        var kaart = context.Kaarten.Find(cardId);

                        if (kaart != null)
                        {
                            // Update status naar Bevroren
                            kaart.Status = KaartStatus.Bevroren;
                            context.SaveChanges();

                            MessageBox.Show($"Kaart {cardId} is bevroren!",
                                "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                            LaadKaarten(); // Herlaad kaarten lijst
                        }
                        else
                        {
                            MessageBox.Show("Kaart niet gevonden!",
                                "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fout bij bevriezen kaart: {ex.Message}",
                        "Database Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Kaart blokkeren
        private void BtnBlokkeren_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int cardId = int.Parse(btn.Tag.ToString());

            var result = MessageBox.Show(
                $"Kaart {cardId} permanent blokkeren?\n\n" +
                "Deze actie kan niet ongedaan worden gemaakt!",
                "Waarschuwing",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new AppDbContext())
                    {
                        // Zoek kaart in database
                        var kaart = context.Kaarten.Find(cardId);

                        if (kaart != null)
                        {
                            // Update status naar Geblokkeerd
                            kaart.Status = KaartStatus.Geblokkeerd;
                            context.SaveChanges();

                            MessageBox.Show($"Kaart {cardId} is geblokkeerd!",
                                "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                            LaadKaarten(); // Herlaad kaarten lijst
                        }
                        else
                        {
                            MessageBox.Show("Kaart niet gevonden!",
                                "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fout bij blokkeren kaart: {ex.Message}",
                        "Database Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}