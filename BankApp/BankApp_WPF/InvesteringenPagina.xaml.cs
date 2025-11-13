using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BankApp_WPF

{
    // Pagina voor het bekijken van investeringen
    public partial class InvesteringenPagina : Window
    {
        public InvesteringenPagina()
        {
            InitializeComponent();
            this.KeyDown += Window_KeyDown;
            this.Focusable = true;
            this.Focus();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
            {
                e.Handled = true;
                SparenInvesteren sparenInvesteren = new SparenInvesteren();
                sparenInvesteren.Show();
                this.Close();

            }
        }

        // Terug knop - sluit het huidige venster
        // Terug knop - keert terug naar de Sparen & Investeren pagina
        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            SparenInvesteren sparenInvesteren = new SparenInvesteren();
            sparenInvesteren.Show();
            this.Close();
        }


        // Details knop - toont meer informatie over een investering
        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Gedetailleerde informatie over deze investering wordt weergegeven...", "Details");
        }

        // Verkopen knop - opent dialoog voor verkopen van een investering
        private void BtnVerkopen_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Verkoopscherm voor deze investering wordt geopend...", "Verkopen");
        }

        // Opnemen knop - opent dialoog voor opnemen van geld
        private void BtnOpnemen_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Opnamescherm wordt geopend...", "Opnemen");
        }

        // Nieuw Beleggen knop - opent scherm voor nieuwe investering
        private void BtnNieuwBeleggen_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Scherm voor nieuwe belegging wordt geopend...", "Nieuw Beleggen");
        }

        // Historie Bekijken knop - toont geschiedenis van investeringen
        private void BtnHistorie_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Transactiehistorie van uw investeringen wordt weergegeven...", "Historie");
        }
    }
}
