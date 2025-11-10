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


        public AdminPagina()
        {
            InitializeComponent();
            LaadKlanten(); // Laad klanten bij opstarten
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

                    // Voor nu: toon aantal in console/debug
                    Console.WriteLine($"Aantal gebruikers geladen: {gebruikers.Count}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden klanten: {ex.Message}",
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
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                MessageBox.Show("Voornaam is verplicht!", "Fout",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                MessageBox.Show("Naam is verplicht!", "Fout",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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
                    // Controleer of het een nieuwe klant is of een bestaande klant wordt bijgewerkt
                    if (string.IsNullOrEmpty(CustomerIdTextBox.Text))
                    {
                        // NIEUWE KLANT TOEVOEGEN

                        // Bepaal rol (standaard Klant = 1)
                        int rolId = 1; // Klant
                        if (RoleComboBox.SelectedIndex == 1)
                        {
                            rolId = 3; // Beheerder
                        }

                        // Hash het wachtwoord (simpel voor nu)
                        string passwordHash = HashPassword(PasswordBox.Password);

                        // Maak nieuwe gebruiker aan
                        var nieuweGebruiker = new Gebruiker
                        {
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

                        MessageBox.Show($"Nieuwe klant toegevoegd!\n\n" +
                            $"Naam: {FirstNameTextBox.Text} {LastNameTextBox.Text}\n" +
                            $"Email: {EmailTextBox.Text}\n" +
                            $"ID: {nieuweGebruiker.Id}",
                            "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Form leegmaken na toevoegen
                        ClearForm();
                        LaadKlanten(); // Herlaad klanten lijst
                    }
                    else
                    {
                        // BESTAANDE KLANT BIJWERKEN
                        int customerId = int.Parse(CustomerIdTextBox.Text);

                        // Zoek gebruiker in database
                        var gebruiker = context.Gebruikers.Find(customerId);

                        if (gebruiker != null)
                        {
                            // Update gegevens
                            gebruiker.Email = EmailTextBox.Text.Trim();
                            gebruiker.Telefoonnummer = PhoneNumberTextBox.Text.Trim();
                            gebruiker.Geboortedatum = BirthDatePicker.SelectedDate.Value;
                            gebruiker.Straatnaam = StreetNameTextBox.Text.Trim();
                            gebruiker.Huisnummer = HouseNumberTextBox.Text.Trim();
                            gebruiker.Bus = BusTextBox.Text.Trim();
                            gebruiker.Postcode = PostcodeTextBox.Text.Trim();
                            gebruiker.Gemeente = CityTextBox.Text.Trim();
                            gebruiker.Land = CountryTextBox.Text.Trim();

                            // Update rol
                            if (RoleComboBox.SelectedIndex == 1)
                            {
                                gebruiker.RolId = 3; // Beheerder
                            }
                            else
                            {
                                gebruiker.RolId = 1; // Klant
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij opslaan: {ex.Message}",
                    "Database Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Simpele password hashing functie
        private string HashPassword(string password)
        {
            // Voor productie: gebruik BCrypt of ASP.NET Identity
            // Voor nu: simpele hash voor demonstratie
            return Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(password + "_salt123"));
        }



        // Annuleren - form leegmaken
        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }


        // Bewerken - klantgegevens laden in form
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
                        FirstNameTextBox.Text = ""; // Geen voornaam veld in model
                        LastNameTextBox.Text = ""; // Geen achternaam veld in model
                        EmailTextBox.Text = gebruiker.Email;
                        PhoneNumberTextBox.Text = gebruiker.Telefoonnummer ?? "";
                        BirthDatePicker.SelectedDate = gebruiker.Geboortedatum;
                        StreetNameTextBox.Text = gebruiker.Straatnaam ?? "";
                        HouseNumberTextBox.Text = gebruiker.Huisnummer ?? "";
                        BusTextBox.Text = gebruiker.Bus ?? "";
                        PostcodeTextBox.Text = gebruiker.Postcode ?? "";
                        CityTextBox.Text = gebruiker.Gemeente ?? "";
                        CountryTextBox.Text = gebruiker.Land ?? "";

                        // Zet rol combobox
                        if (gebruiker.RolId == 3)
                        {
                            RoleComboBox.SelectedIndex = 1; // Admin
                        }
                        else
                        {
                            RoleComboBox.SelectedIndex = 0; // Klant
                        }

                        // Verander knop tekst naar "Bijwerken"
                        SaveCustomerButton.Content = "✓ Klant bijwerken";

                        MessageBox.Show($"Klant {customerId} geladen voor bewerken",
                            "Info", MessageBoxButton.OK, MessageBoxImage.Information);
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



        // Helper methode: form leegmaken
        private void ClearForm()
        {
            CustomerIdTextBox.Text = "";
            FirstNameTextBox.Clear();
            LastNameTextBox.Clear();
            EmailTextBox.Clear();
            PasswordBox.Clear();
            ConfirmPasswordBox.Clear();
            PhoneNumberTextBox.Clear();
            BirthDatePicker.SelectedDate = null;
            StreetNameTextBox.Clear();
            HouseNumberTextBox.Clear();
            BusTextBox.Clear();
            PostcodeTextBox.Clear();
            CityTextBox.Clear();
            CountryTextBox.Text = "België";
            RoleComboBox.SelectedIndex = -1;

            // Verander knop tekst terug naar "Toevoegen"
            SaveCustomerButton.Content = "👤 Klant toevoegen";

            MessageBox.Show("Formulier is geleegd.", "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }



        // ========== GEBRUIKERS TAB - Event Handlers ==========

        // Wijzig rol naar Klant
        private void BtnWijzigNaarKlant_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int userId = int.Parse(btn.Tag.ToString());

            var result = MessageBox.Show(
                $"Rol van gebruiker {userId} wijzigen naar Klant?",
                "Bevestigen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new AppDbContext())
                    {
                        // Zoek gebruiker
                        var gebruiker = context.Gebruikers.Find(userId);

                        if (gebruiker != null)
                        {
                            // Update rol naar Klant (RolId = 1)
                            gebruiker.RolId = 1;
                            context.SaveChanges();

                            MessageBox.Show($"Gebruiker {userId} rol is gewijzigd naar Klant!",
                                "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    MessageBox.Show($"Fout bij wijzigen rol: {ex.Message}",
                        "Database Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Wijzig rol naar Admin
        private void BtnWijzigNaarAdmin_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int userId = int.Parse(btn.Tag.ToString());

            var result = MessageBox.Show(
                $"Rol van gebruiker {userId} wijzigen naar Admin?",
                "Bevestigen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new AppDbContext())
                    {
                        // Zoek gebruiker
                        var gebruiker = context.Gebruikers.Find(userId);

                        if (gebruiker != null)
                        {
                            // Update rol naar Beheerder (RolId = 3)
                            gebruiker.RolId = 3;
                            context.SaveChanges();

                            MessageBox.Show($"Gebruiker {userId} rol is gewijzigd naar Admin!",
                                "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    MessageBox.Show($"Fout bij wijzigen rol: {ex.Message}",
                        "Database Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }





        //   KAARTEN TAB - Event Handlers ==========

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

        // AANVRAGEN TAB - Event Handlers 

        // Aanvraag goedkeuren
        private void BtnGoedkeuren_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int requestId = int.Parse(btn.Tag.ToString());

            var result = MessageBox.Show(
                $"Aanvraag {requestId} goedkeuren?",
                "Bevestigen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new AppDbContext())
                    {
                        // Zoek afspraak in database
                        var afspraak = context.Afspraken.Find(requestId);

                        if (afspraak != null)
                        {
                            // Update status naar Goedgekeurd
                            afspraak.Status = AfspraakStatus.Goedgekeurd;
                            context.SaveChanges();

                            MessageBox.Show($"Aanvraag {requestId} is goedgekeurd! ✓",
                                "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Aanvraag niet gevonden!",
                                "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fout bij goedkeuren aanvraag: {ex.Message}",
                        "Database Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Aanvraag afwijzen
        private void BtnAfwijzen_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int requestId = int.Parse(btn.Tag.ToString());

            var result = MessageBox.Show(
                $"Aanvraag {requestId} afwijzen?",
                "Bevestigen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new AppDbContext())
                    {
                        // Zoek afspraak in database
                        var afspraak = context.Afspraken.Find(requestId);



                        if (afspraak != null)
                        {
                            // Update status naar Geannuleerd
                            afspraak.Status = AfspraakStatus.Geannuleerd;
                            context.SaveChanges();


                            MessageBox.Show($"Aanvraag {requestId} is afgewezen! ✕",
                                "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Aanvraag niet gevonden!",
                                "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fout bij afwijzen aanvraag: {ex.Message}",
                        "Database Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}