using System.Windows;

namespace BankApp_WPF
{
    public partial class Investment : Window
    {
        public Investment()
        {
            InitializeComponent();
        }

        // Terug knop bovenaan
        private void BackButtonTop_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Terug knop onderaan
        private void BackButtonMain_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}