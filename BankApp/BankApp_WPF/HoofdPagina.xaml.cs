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
            MessageBox.Show("Saldo raadplegen...");
        }

        private void BtnInvest_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sparen en investeren scherm openen...");
        }

        private void BtnTransfer_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Overschrijven scherm openen...");
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Profiel raadplegen...");
        }

        private void BtnContact_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Contact opnemen met de bank...");
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Uitloggen...");
            this.Close();
        }
    }
}
