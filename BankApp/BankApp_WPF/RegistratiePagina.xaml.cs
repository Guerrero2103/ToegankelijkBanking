using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BankApp_WPF
{
    public partial class RegistratiePagina : Window
    {
        public RegistratiePagina()
        {
            InitializeComponent();
        }
<<<<<<< Updated upstream
=======

        private void RegistreerBtn_Click(object sender, RoutedEventArgs e)
        {
            string fouten = ValideerFormulier();
            if (!string.IsNullOrEmpty(fouten))
            {
                MessageBox.Show(fouten, "Fouten bij registratie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MessageBox.Show("Registratie geslaagd! Uw IBAN wordt automatisch toegewezen.",
                            "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            MaakVeldenLeeg();
        }

        // ← VOEG DEZE FUNCTIE TOE!
        private void BtnAccessibility_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Toegankelijkheidsopties:\n\n" +
                "• Spraakfeedback voor visueel beperkten\n" +
                "• Groot lettertype voor betere leesbaarheid\n" +
                "• Hoog contrast modus\n" +
                "• Toetsenbord navigatie (Tab/Enter)\n" +
                "• Schermlezer compatibel\n" +
                "• Focus indicatoren voor beter overzicht",
                "Toegankelijkheid",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private string ValideerFormulier()
        {
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrWhiteSpace(NaamBox.Text))
                sb.AppendLine("Naam is verplicht.");
            if (!IsGeldigEmail(EmailBox.Text))
                sb.AppendLine("Voer een geldig e-mailadres in.");
            if (WachtwoordBox.Password.Length < 8)
                sb.AppendLine("Wachtwoord moet minimaal 8 tekens bevatten.");
            if (WachtwoordBox.Password != BevestigBox.Password)
                sb.AppendLine("Wachtwoorden komen niet overeen.");
            if (!Regex.IsMatch(TelefoonBox.Text, @"^\d+$"))
                sb.AppendLine("Telefoonnummer mag alleen cijfers bevatten.");
            if (GeboortePicker.SelectedDate == null || !IsOuderDan18(GeboortePicker.SelectedDate.Value))
                sb.AppendLine("Je moet minimaal 18 jaar oud zijn.");
            if (string.IsNullOrWhiteSpace(StraatBox.Text))
                sb.AppendLine("Straatnaam is verplicht.");
            if (string.IsNullOrWhiteSpace(HuisnrBox.Text))
                sb.AppendLine("Huisnummer is verplicht.");
            if (string.IsNullOrWhiteSpace(PostcodeBox.Text))
                sb.AppendLine("Postcode is verplicht.");
            if (string.IsNullOrWhiteSpace(GemeenteBox.Text))
                sb.AppendLine("Gemeente is verplicht.");
            if (string.IsNullOrWhiteSpace(LandBox.Text))
                sb.AppendLine("Land is verplicht.");
            return sb.ToString();
        }

        private bool IsGeldigEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
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
>>>>>>> Stashed changes
    }
}