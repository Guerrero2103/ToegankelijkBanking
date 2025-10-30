using System.Windows;
using BankApp_WPF;


namespace BankApp_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Navigate(new HoofdPagina()); 

        }
    }
}
