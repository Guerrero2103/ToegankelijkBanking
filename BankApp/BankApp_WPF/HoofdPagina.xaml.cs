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
            this.Close();
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
