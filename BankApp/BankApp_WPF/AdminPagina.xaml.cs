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



            // Controleer of het een nieuwe klant is of een bestaande klant wordt bijgewerkt
            if (string.IsNullOrEmpty(CustomerIdTextBox.Text))
            {
                // NIEUWE KLANT TOEVOEGEN
                MessageBox.Show("Nieuwe klant wordt toegevoegd...\n" +
                    $"Naam: {FirstNameTextBox.Text} {LastNameTextBox.Text}\n" +
                    $"Email: {EmailTextBox.Text}",
                    "Info", MessageBoxButton.OK, MessageBoxImage.Information);

                // Later Voeg klant toe aan database

                // Form leegmaken na toevoegen
                ClearForm();
            }

            else
            {
                // BESTAANDE KLANT BIJWERKEN
                string customerId = CustomerIdTextBox.Text;
                MessageBox.Show($"Klant {customerId} wordt bijgewerkt...\n" +
                    $"Naam: {FirstNameTextBox.Text} {LastNameTextBox.Text}\n" +
                    $"Email: {EmailTextBox.Text}",
                    "Info", MessageBoxButton.OK, MessageBoxImage.Information);

                

                // Form leegmaken na bijwerken
                ClearForm();
            }
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
            string customerId = btn.Tag.ToString();

            // Voor nu: voorbeeld data
            MessageBox.Show($"Klant {customerId} wordt geladen voor bewerken...",
                "Info", MessageBoxButton.OK, MessageBoxImage.Information);

            // Voorbeeld: vul form met dummy data
            CustomerIdTextBox.Text = customerId;
            FirstNameTextBox.Text = "Jan";
            LastNameTextBox.Text = "de Vries";
            EmailTextBox.Text = "jan@example.com";
            PhoneNumberTextBox.Text = "+32 456 78 90 12";
            // ... vul alle andere velden

            // Verander knop tekst naar "Bijwerken"
            SaveCustomerButton.Content = "Klant bijwerken";

            
        }


        // Verwijderen - klant verwijderen
        private void BtnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string customerId = btn.Tag.ToString();

            // Bevestiging vragen
            var result = MessageBox.Show(
                $"Weet u zeker dat u klant {customerId} wilt verwijderen?",
                "Bevestigen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Later Verwijder klant uit database
                MessageBox.Show($"Klant {customerId} is verwijderd!",
                    "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
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
            SaveCustomerButton.Content = "Klant toevoegen";


            MessageBox.Show("Formulier is geleegd.", "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }



        // ========== GEBRUIKERS TAB - Event Handlers ==========

        // Wijzig rol naar Klant
        private void BtnWijzigNaarKlant_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string userId = btn.Tag.ToString();


            var result = MessageBox.Show(
                $"Rol van gebruiker {userId} wijzigen naar Klant?",
                "Bevestigen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);




            if (result == MessageBoxResult.Yes)
            {
                // TODO: Update rol in database
                MessageBox.Show($"Gebruiker {userId} rol is gewijzigd naar Klant!",
                    "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }


        }

        // Wijzig rol naar Admin
        private void BtnWijzigNaarAdmin_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string userId = btn.Tag.ToString();

            var result = MessageBox.Show(
                $"Rol van gebruiker {userId} wijzigen naar Admin?",
                "Bevestigen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);


            if (result == MessageBoxResult.Yes)
            {
                
                MessageBox.Show($"Gebruiker {userId} rol is gewijzigd naar Admin!",
                    "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }





        //   KAARTEN TAB - Event Handlers ==========

        //   Kaart bevriezen
        private void BtnBevriezen_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string cardId = btn.Tag.ToString();

            var result = MessageBox.Show(
                $"Kaart {cardId} bevriezen?",
                "Bevestigen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);


            if (result == MessageBoxResult.Yes)
            {
                // TODO: Update kaartstatus in database
                MessageBox.Show($"Kaart {cardId} is bevroren! ",
                    "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }



        // Kaart blokkeren
        private void BtnBlokkeren_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string cardId = btn.Tag.ToString();

            var result = MessageBox.Show(
                $"Kaart {cardId} permanent blokkeren?\nDeze actie kan niet ongedaan worden gemaakt!",
                "Waarschuwing",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);


            if (result == MessageBoxResult.Yes)
            {
                // TODO: Update kaartstatus in database
                MessageBox.Show($"Kaart {cardId} is geblokkeerd! ",
                    "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // AANVRAGEN TAB - Event Handlers 

        // Aanvraag goedkeuren
        private void BtnGoedkeuren_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string requestId = btn.Tag.ToString();


            var result = MessageBox.Show(
                $"Aanvraag {requestId} goedkeuren?",
                "Bevestigen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // TODO: Update aanvraag status in database
                MessageBox.Show($"Aanvraag {requestId} is goedgekeurd! ✓",
                    "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Aanvraag afwijzen
        private void BtnAfwijzen_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string requestId = btn.Tag.ToString();


            var result = MessageBox.Show(
                $"Aanvraag {requestId} afwijzen?",
                "Bevestigen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Later Update aanvraag status in database
                MessageBox.Show($"Aanvraag {requestId} is afgewezen! ✕",
                    "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


    }
}
