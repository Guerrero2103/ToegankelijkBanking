using System.Windows;

namespace BankApp_WPF
{
    public partial class ProfilePage : Window
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        // Terug knop - sluit het venster
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Open de HoofdPagina
            HoofdPagina hoofdPagina = new HoofdPagina();
            hoofdPagina.Show();

            // Sluit dit venster
            this.Close();
        }

        // Voice toggle knop
        private void VoiceToggleButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle voice functionaliteit
            if (VoiceToggleButton.Content.ToString() == "🔊")
            {
                VoiceToggleButton.Content = "🔇";
                MessageBox.Show("Spraak feedback uitgeschakeld", "Voice", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                VoiceToggleButton.Content = "🔊";
                MessageBox.Show("Spraak feedback ingeschakeld", "Voice", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Opslaan knop
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validatie: Check of wachtwoorden overeenkomen
            if (!string.IsNullOrEmpty(PasswordBox.Password) &&
                PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Wachtwoorden komen niet overeen!", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Profiel opslaan
            MessageBox.Show("Profiel succesvol bijgewerkt!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Verwijder account knop
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Weet u zeker dat u uw profiel wilt verwijderen?\n\nDeze actie kan niet ongedaan gemaakt worden!",
                "Waarschuwing",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("Account succesvol verwijderd", "Verwijderd", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }
    }
}