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
    public partial class Sparen : Window
    {
        public Sparen()
        {
            InitializeComponent();
        }

        private void BtnKopieerVakantie_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("BE71 0961 2345 6769");
            MessageBox.Show("IBAN van Vakantie Spaarrekening gekopieerd!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnKopieerNoodfonds_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("BE71 0961 9876 5432");
            MessageBox.Show("IBAN van Noodfonds Spaarrekening gekopieerd!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            HoofdPagina hoofd = new HoofdPagina();
            hoofd.Show();
            this.Close();
        }
    }
}

