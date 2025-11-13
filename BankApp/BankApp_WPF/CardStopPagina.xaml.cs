using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BankApp_WPF
{
    public partial class CardStopPagina : Window
    {
        public CardStopPagina()
        {
            InitializeComponent();
            this.KeyDown += Window_KeyDown;
            this.Focusable = true;
            this.Focus();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z)
            {
                e.Handled = true;
                HoofdPagina hoofdPagina = new HoofdPagina();
                hoofdPagina.Show();
                this.Close();

            }
        }


        // Toegankelijkheidsknop placeholder
        private void BtnAccessibility_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Toegankelijkheidsopties worden later toegevoegd.",
                            "AccessBank - Toegankelijkheid",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        // Terug naar hoofdmenu
        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            HoofdPagina hoofdPagina = new HoofdPagina();
            hoofdPagina.Show();
            this.Close();
        }
    }
}