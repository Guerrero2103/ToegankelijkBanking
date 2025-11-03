using System.Windows;
using System.Windows.Input;

namespace BankApp_WPF
{
    public partial class SparenInvesteren : Window
    {
        public SparenInvesteren()
        {
            InitializeComponent();
            this.KeyDown += SparenInvesteren_KeyDown;
            this.Focusable = true;
            this.Focus();
        }

        private void SparenInvesteren_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z)
                GaTerugNaarHoofdPagina();
        }

        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            GaTerugNaarHoofdPagina();
        }

        private void BtnTerugHoofd_Click(object sender, RoutedEventArgs e)
        {
            GaTerugNaarHoofdPagina();
        }

        private void BtnSparen_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Navigeren naar Spaarpagina...");
        }

        private void BtnInvesteren_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Navigeren naar Investeerpagina...");
        }

        private void GaTerugNaarHoofdPagina()
        {
            this.Close(); 
        }
    }
}
