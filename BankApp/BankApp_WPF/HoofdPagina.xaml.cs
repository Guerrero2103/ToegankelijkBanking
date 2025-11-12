using BankApp_Models;
using BankApp_BusinessLogic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BankApp_WPF
{
    public partial class HoofdPagina : Window
    {
        private readonly IRekeningService _rekeningService;

        public HoofdPagina()
        {
            InitializeComponent();

            // Initialize database
            try
            {
                using (var context = new AppDbContext())
                {
                    // Database.EnsureCreated() wordt al aangeroepen in de constructor
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij aanmaken database: {ex.Message}");
            }

            // Initialize services
            var dbContext = new AppDbContext();
            _rekeningService = new RekeningService(dbContext);

            // Load user data
            LoadUserData();
        }

        private async void LoadUserData()
        {
            if (!UserSession.IsIngelogd)
            {
                MessageBox.Show("Je bent niet ingelogd.", "Fout",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var gebruikerId = UserSession.IngelogdeGebruiker!.Id;

                // Haal rekeningen op; maak er één aan als er geen zijn
                var rekeningen = await _rekeningService.GetRekeningenByGebruikerIdAsync(gebruikerId);
                if (rekeningen == null || rekeningen.Count == 0)
                {
                    var nieuwe = await _rekeningService.MaakRekeningAanAsync(gebruikerId, RekeningType.Zicht);
                    rekeningen = new System.Collections.Generic.List<Rekening> { nieuwe };
                }

                // Haal totaal saldo op
                var totaalSaldo = await _rekeningService.GetTotaalSaldoAsync(gebruikerId);
                lblTotalSaldo.Content = $"€{totaalSaldo:N2}";

                // Toon zichtrekening IBAN volledig. Gebruik de eerste zichtrekening of de eerste beschikbare.
                var zichtRekening = rekeningen.FirstOrDefault(r => r.Type == RekeningType.Zicht) ?? rekeningen.First();
                if (zichtRekening != null)
                {
                    lblAccountNumber.Content = $"Zichtrekening {zichtRekening.Iban}";
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij laden gegevens: {ex.Message}", "Fout",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAccessibility_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Toegankelijkheidsinstellingen openen...");
        }

        private void BtnViewSaldo_Click(object sender, RoutedEventArgs e)
        {
            SaldoRaadplegenPagina saldoRaadplegenPagina = new SaldoRaadplegenPagina();
            saldoRaadplegenPagina.Show();
            this.Close();
        }

        private void BtnInvest_Click(object sender, RoutedEventArgs e)
        {
            SparenInvesteren sparenInvesteren = new SparenInvesteren();
            sparenInvesteren.Show();
            this.Close();
        }

        private void BtnTransfer_Click(object sender, RoutedEventArgs e)
        {
            OverschrijvingenPagina overschrijvingenPagina = new OverschrijvingenPagina();
            overschrijvingenPagina.Show();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            ProfilePage profilePage = new ProfilePage();
            profilePage.Show();
            this.Close();
        }

        private void BtnContact_Click(object sender, RoutedEventArgs e)
        {
            KlantendienstPagina klantendienstPagina = new KlantendienstPagina();
            klantendienstPagina.Show();
            this.Close();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Weet je zeker dat je wilt uitloggen?",
                "Uitloggen", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                UserSession.LogUit();
                StartPagina startPagina = new StartPagina();
                startPagina.Show();
                this.Close();
            }
        }
    }
}