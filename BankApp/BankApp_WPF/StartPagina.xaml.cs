using BankApp_Models;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace BankApp_WPF
{
    public partial class StartPagina : Window
    {
        private bool isDarkTheme = true;

        public StartPagina()
        {
            InitializeComponent();


            // 🔹 Database aanmaken bij opstarten
            try
            {
                using (var context = new AppDbContext())
                {
                    // Database.EnsureCreated() wordt al aangeroepen in de constructor
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij aanmaken database: {ex.Message}");
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            RegistratiePagina registratiePagina = new RegistratiePagina();
            registratiePagina.Show();
            this.Close();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginPagina loginPagina = new LoginPagina();
            loginPagina.Show();
            this.Close();
        }

        private void BtnCardStop_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Weet je zeker dat je je bankkaart wilt blokkeren?",
                "Card Stop",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                CardStopBlokkeringPagina cardStopPagina = new CardStopBlokkeringPagina();
                cardStopPagina.Show();
                this.Close();
            }
        }

        private void BtnTheme_Click(object sender, RoutedEventArgs e)
        {
            isDarkTheme = !isDarkTheme;

            if (isDarkTheme)
            {
                this.Background = Brushes.Black;
                BtnTheme.Content = "☀";
            }
            else
            {
                this.Background = Brushes.White;
                BtnTheme.Content = "🌙";
            }
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "AccessBank Hulp\n\n" +
                "👥 Registreren - Maak een nieuw account aan\n" +
                "🔐 Inloggen - Meld aan met je bestaande gegevens\n" +
                "💳 Card Stop - Blokkeer je bankkaart bij verlies of diefstal\n\n" +
                "Voor verdere hulp, contacteer support@accessbank.be",
                "Help",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void BtnKlantendienst_Click(object sender, RoutedEventArgs e)
        {
            KlantendienstPagina klantendienst = new KlantendienstPagina();
            klantendienst.Show();
            this.Close();
        }
    }
}
