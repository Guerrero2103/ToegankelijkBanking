using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using BankApp_Models;
using Microsoft.EntityFrameworkCore;

namespace BankApp_WPF
{
    public partial class Sparen : Window
    {
        // ObservableCollection voor data binding van spaarrekeningen
        private ObservableCollection<Rekening> _spaarrekeningen;
        public ObservableCollection<Rekening> Spaarrekeningen
        {
            get { return _spaarrekeningen; }
            set { _spaarrekeningen = value; }
        }

        public Sparen()
        {
            InitializeComponent();
            Spaarrekeningen = new ObservableCollection<Rekening>();
            LaadSpaarrekeningen(); // Laad spaarrekeningen uit database
        }

        // Laad alle spaarrekeningen van de ingelogde gebruiker uit database
        private void LaadSpaarrekeningen()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    // LINQ query: Haal alle spaarrekeningen op (RekeningType.Spaar)
                    var spaarrekeningen = context.Rekeningen
                        .Where(r => r.Type == RekeningType.Spaar) // Lambda expressie
                        .Include(r => r.Gebruiker) // Include gebruiker voor referentie
                        .ToList();

                    // Vul de ObservableCollection
                    Spaarrekeningen.Clear();
                    foreach (var rekening in spaarrekeningen)
                    {
                        Spaarrekeningen.Add(rekening);
                    }

                    // Bind aan de ItemsControl in XAML
                    SpaarrekiningenListBox.ItemsSource = Spaarrekeningen;

                    Console.WriteLine($"✅ {spaarrekeningen.Count} spaarrekeningen geladen");
                }
            }
            catch (Exception ex)
            {
                // Try-Catch foutafhandeling
                MessageBox.Show(
                    $"Fout bij laden spaarrekeningen:\n{ex.Message}",
                    "Database Fout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // IBAN kopiëren functionaliteit
        private void BtnKopieerIban_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Haal IBAN op van de knop Tag property
                Button btn = (Button)sender;
                string iban = btn.Tag.ToString();

                // Kopieer naar clipboard
                Clipboard.SetText(iban);

                // Toon bevestiging
                MessageBox.Show(
                    $"IBAN gekopieerd!\n\n{iban}",
                    "✓ Succes",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Try-Catch foutafhandeling
                MessageBox.Show(
                    $"Fout bij kopiëren IBAN:\n{ex.Message}",
                    "Fout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Terug knop
        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HoofdPagina hoofd = new HoofdPagina();
                hoofd.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                // Try-Catch foutafhandeling
                MessageBox.Show(
                    $"Fout bij navigeren:\n{ex.Message}",
                    "Fout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}