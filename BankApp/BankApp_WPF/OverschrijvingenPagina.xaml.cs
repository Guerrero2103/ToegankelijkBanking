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
    /// <summary>
    /// Interaction logic for OverschrijvingenPagina.xaml
    /// </summary>
        // Pagina voor het maken van overschrijvingen
        public partial class OverschrijvingenPagina : Window
        {
            public OverschrijvingenPagina()
            {
            InitializeComponent();
            this.KeyDown += Window_KeyDown;
            this.Focusable = true;
            this.Focus();
        }
        // Z Toets Handler - Voeg toe aan ELKE window
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z)
            {
                e.Handled = true;

                // Open specifiek venster bij indrukken van Z
                HoofdPagina hoofdPagina = new HoofdPagina();
                hoofdPagina.Show();

                // Sluit huidige venster
                this.Close();
            }
        }

            // Terug knop - sluit het huidige venster
            private void BtnTerug_Click(object sender, RoutedEventArgs e)
            {
            // Open specifiek venster bij indrukken van Z
            HoofdPagina hoofdPagina = new HoofdPagina();
            hoofdPagina.Show();
            this.Close();
            }

            // Annuleren knop - wist alle invoervelden
            private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
            {
                txtIban.Clear();
                txtNaamOntvanger.Clear();
                txtBedrag.Clear();
                txtOmschrijving.Clear();
                MessageBox.Show("Overschrijving geannuleerd.", "Info");
            }

            // Verzenden knop - valideert en verwerkt de overschrijving
            private void BtnVerzenden_Click(object sender, RoutedEventArgs e)
            {
                // Controleer of alle vereiste velden ingevuld zijn
                if (string.IsNullOrWhiteSpace(txtIban.Text))
                {
                    MessageBox.Show("Voer alstublieft een IBAN in.", "Fout");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNaamOntvanger.Text))
                {
                    MessageBox.Show("Voer alstublieft de naam van de ontvanger in.", "Fout");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtBedrag.Text))
                {
                    MessageBox.Show("Voer alstublieft een bedrag in.", "Fout");
                    return;
                }

                // Controleer of het bedrag een geldig getal is
                if (!decimal.TryParse(txtBedrag.Text, out decimal amount))
                {
                    MessageBox.Show("Het bedrag moet een geldig getal zijn.", "Fout");
                    return;
                }

                if (amount <= 0)
                {
                    MessageBox.Show("Het bedrag moet groter zijn dan 0.", "Fout");
                    return;
                }

                // Laat succes bericht zien en leeg de velden
                MessageBox.Show($"Overschrijving van €{amount:F2} naar {txtNaamOntvanger.Text} verzonden!", "Succes");

                // Leeg alle velden
                txtIban.Clear();
                txtNaamOntvanger.Clear();
                txtBedrag.Clear();
                txtOmschrijving.Clear();
            }
        }
}
