using System.Windows;
using System.Windows.Input;

namespace BankApp_WPF;

public partial class SparenInvesteren : Window
{
    private bool _voiceEnabled = true;

    public SparenInvesteren()
    {
        
        InitializeComponent();
        // Enable keyboard shortcuts
        this.KeyDown += Window_KeyDown;
        this.Focusable = true;
        this.Focus();
    }

    // Keyboard shortcuts handler
    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Z)
        {
            e.Handled = true;
            GaTerugNaarHoofdPagina();
        }
    }

    // Terug knop (top)
    private void BtnTerug_Click(object sender, RoutedEventArgs e)
    {
        GaTerugNaarHoofdPagina();
    }

    // Terug knop (alternative name - voor compatibility)
    private void BackButtonTop_Click(object sender, RoutedEventArgs e)
    {
        GaTerugNaarHoofdPagina();
    }

    // Terug knop (bottom)
    private void BtnTerugHoofd_Click(object sender, RoutedEventArgs e)
    {
        GaTerugNaarHoofdPagina();
    }

    // Alternative name
    private void BackButtonMain_Click(object sender, RoutedEventArgs e)
    {
        GaTerugNaarHoofdPagina();
    }

    // Voice toggle button
    private void VoiceToggleButton_Click(object sender, RoutedEventArgs e)
    {
        _voiceEnabled = !_voiceEnabled;

        if (sender is System.Windows.Controls.Button button)
        {
            button.Content = _voiceEnabled ? "🔊" : "🔇";
        }

        if (_voiceEnabled)
        {
            AnnounceMessage("Spraak feedback ingeschakeld");
        }
    }

    // Sparen knop
    private void BtnSparen_Click(object sender, RoutedEventArgs e)
    {
        Sparen sparen = new Sparen();
        sparen.Show();
        this.Close();
    }

    // Alternative name
    private void SavingsButton_Click(object sender, RoutedEventArgs e)
    {
        OpenSparenPagina();
    }

    // Investeren knop
    private void BtnInvesteren_Click(object sender, RoutedEventArgs e)
    {
        InvesteringenPagina investeringenPagina = new InvesteringenPagina();
        investeringenPagina.Show();
        this.Close();
    }

    // Alternative name
    private void InvestmentsButton_Click(object sender, RoutedEventArgs e)
    {
        OpenInvesterenPagina();
    }

    // Helper methods
    private void OpenSparenPagina()
    {
        try
        {
            // Probeer Sparen window te openen
            var sparenWindow = new Sparen();
            sparenWindow.ShowDialog();
        }
        catch
        {
            // Als Sparen window niet bestaat, toon placeholder
            MessageBox.Show(
                "Sparen functionaliteit\n\n" +
                "Hier kunt u:\n" +
                "• Spaarrekeningen bekijken\n" +
                "• Spaardoelen instellen\n" +
                "• Geld overmaken naar spaarrekening\n\n" +
                "Deze functie wordt binnenkort toegevoegd.",
                "Sparen",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }

    private void OpenInvesterenPagina()
    {
        try
        {
            // Probeer InvesteringenPagina te openen
            var investeringenWindow = new InvesteringenPagina();
            investeringenWindow.ShowDialog();
        }
        catch
        {
            // Als InvesteringenPagina niet bestaat, toon placeholder
            MessageBox.Show(
                "Investeren functionaliteit\n\n" +
                "Hier kunt u:\n" +
                "• Uw investeringen bekijken\n" +
                "• Rendement controleren\n" +
                "• Nieuwe investeringen doen\n" +
                "• Investeringen verkopen\n\n" +
                "Deze functie wordt binnenkort toegevoegd.",
                "Investeren",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }

    private void GaTerugNaarHoofdPagina()
    {
        AnnounceMessage("Terug naar hoofdpagina");

        // Open de HoofdPagina
        HoofdPagina hoofdPagina = new HoofdPagina();
        hoofdPagina.Show();

        // Sluit dit venster
        this.Close();
    }


    private void AnnounceMessage(string message)
    {
        if (_voiceEnabled)
        {
            // TODO: Implementeer text-to-speech
            System.Diagnostics.Debug.WriteLine($"Announce: {message}");
        }
    }
}