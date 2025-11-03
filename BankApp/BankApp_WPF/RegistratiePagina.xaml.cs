using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace BankApp_WPF;

public partial class RegistratiePagina : Window
{
    public RegistratiePagina()
    {
        InitializeComponent();

        // Set default land
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

        // TODO: Hier database registratie code toevoegen

        MessageBox.Show(
            "✅ Registratie geslaagd!\n\n" +
            "Uw account is succesvol aangemaakt.\n" +
            "Uw IBAN wordt automatisch toegewezen.\n\n" +
            "U wordt nu doorgestuurd naar de login pagina.",
            "Succes",
            MessageBoxButton.OK,
            MessageBoxImage.Information);

        MaakVeldenLeeg();

        // Open login pagina
        LoginPagina loginPagina = new LoginPagina();
        loginPagina.Show();
        this.Close();
    }

    // ✅ TOEGEVOEGD: BtnAccessibility_Click event handler
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

    private string ValideerFormulier()
    {
        StringBuilder sb = new StringBuilder();

        // Naam validatie
        if (string.IsNullOrWhiteSpace(NaamBox.Text))
            sb.AppendLine("• Naam is verplicht.");
        else if (NaamBox.Text.Length < 2)
            sb.AppendLine("• Naam moet minimaal 2 tekens bevatten.");

        // Email validatie
        if (!IsGeldigEmail(EmailBox.Text))
            sb.AppendLine("• Voer een geldig e-mailadres in.");

        // Wachtwoord validatie
        if (WachtwoordBox.Password.Length < 8)
            sb.AppendLine("• Wachtwoord moet minimaal 8 tekens bevatten.");
        else if (!HeeftHoofdletterEnCijfer(WachtwoordBox.Password))
            sb.AppendLine("• Wachtwoord moet minimaal 1 hoofdletter en 1 cijfer bevatten.");

        if (WachtwoordBox.Password != BevestigBox.Password)
            sb.AppendLine("• Wachtwoorden komen niet overeen.");

        // Telefoonnummer validatie
        if (string.IsNullOrWhiteSpace(TelefoonBox.Text))
            sb.AppendLine("• Telefoonnummer is verplicht.");
        else if (!Regex.IsMatch(TelefoonBox.Text.Replace(" ", "").Replace("+", ""), @"^\d+$"))
            sb.AppendLine("• Telefoonnummer mag alleen cijfers bevatten (+ en spaties zijn toegestaan).");

        // Geboortedatum validatie
        if (GeboortePicker.SelectedDate == null)
            sb.AppendLine("• Geboortedatum is verplicht.");
        else if (!IsOuderDan18(GeboortePicker.SelectedDate.Value))
            sb.AppendLine("• Je moet minimaal 18 jaar oud zijn om een account aan te maken.");

        // Adres validatie
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

    private bool IsGeldigEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private bool IsOuderDan18(DateTime geboortedatum)
    {
        int leeftijd = DateTime.Now.Year - geboortedatum.Year;

        if (geboortedatum.Date > DateTime.Now.AddYears(-leeftijd))
            leeftijd--;

        return leeftijd >= 18;
    }

    private bool HeeftHoofdletterEnCijfer(string wachtwoord)
    {
        bool heeftHoofdletter = Regex.IsMatch(wachtwoord, @"[A-Z]");
        bool heeftCijfer = Regex.IsMatch(wachtwoord, @"\d");

        return heeftHoofdletter && heeftCijfer;
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
}