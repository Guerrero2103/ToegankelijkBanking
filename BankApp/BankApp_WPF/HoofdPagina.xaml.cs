using BankApp_Models;
using System.Text;
using System.Windows;

namespace BankApp_WPF
{
    public partial class HoofdPagina : Window
    {
        public HoofdPagina()
        {
            InitializeComponent();

            // 🔹 Database aanmaken bij opstarten
            try
            {
                using (var context = new AppDbContext())
                {
                    // Database.EnsureCreated() wordt al aangeroepen in de constructor
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij aanmaken database: {ex.Message}");
            }

            // 🔹 Tijdelijk debug: alle gebruikers tonen
            try
            {
                using (var context = new AppDbContext())
                {
                    var gebruikers = context.Gebruikers.ToList();

                    StringBuilder sb = new StringBuilder();
                    foreach (var gebruiker in gebruikers)
                    {
                        sb.AppendLine($"Id: {gebruiker.Id}, Email: {gebruiker.Email}, RolId: {gebruiker.RolId}");
                    }

                    MessageBox.Show(sb.Length > 0 ? sb.ToString() : "Geen gebruikers gevonden.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij lezen database: {ex.Message}");
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
            MessageBox.Show("Uitloggen...");
            this.Close();
        }
    }
}
