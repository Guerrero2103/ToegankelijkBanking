using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BankApp_WPF
{
    public partial class CardStopBlokkeringPagina : Window
    {
        public CardStopBlokkeringPagina()
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
                StartPagina startPagina = new StartPagina();
                startPagina.Show();
                this.Close();

            }
        }

    }
}
