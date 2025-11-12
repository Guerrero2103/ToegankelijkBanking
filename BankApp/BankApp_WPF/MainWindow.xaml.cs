using BankApp_Models;
using BankApp_WPF;
using System.Windows;

namespace BankApp_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Navigeer naar StartPagina in plaats van HoofdPagina
            MainFrame.Navigate(new StartPagina());
        }
    }
}
