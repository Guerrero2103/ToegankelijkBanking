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
            // Open Sparen pagina
            Sparenpage sparenPage = new Sparenpage();
            sparenPage.Show();
        }

        private void InvestmentsButton_Click(object sender, RoutedEventArgs e)
        {
            // Open Investment pagina
            Investment investmentPage = new Investment();
            investmentPage.Show();
        }

        private void BackButtonMain_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}