using System.Windows;

namespace BankApp_WPF
{
    public partial class SparenEnInvesteringen : Window
    {
        public SparenEnInvesteringen()
        {
           
        }

        private void BackButtonTop_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SavingsButton_Click(object sender, RoutedEventArgs e)
        {
            Sparenpage sparenPage = new Sparenpage();
            sparenPage.Show();
            this.Close();
        }

        private void InvestmentsButton_Click(object sender, RoutedEventArgs e)
        {
            InvesteringenPagina investeringenPagina = new InvesteringenPagina();
            investeringenPagina.Show();
            this.Close();
        }

        private void BackButtonMain_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}