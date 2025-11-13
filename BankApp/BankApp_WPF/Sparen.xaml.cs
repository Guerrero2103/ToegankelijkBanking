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
            this.KeyDown += Window_KeyDown;
            this.Focusable = true;
            this.Focus();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
            {
                e.Handled = true;
                SparenInvesteren sparenInvesteren = new SparenInvesteren();
                sparenInvesteren.Show();
                this.Close();
            }
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

                    // Als er spaarrekeningen zijn in database, toon die
                    if (spaarrekeningen.Count > 0)
                    {
                        foreach (var rekening in spaarrekeningen)
                        {
                            Spaarrekeningen.Add(rekening);
                        }
                        Console.WriteLine($"✅ {spaarrekeningen.Count} spaarrekeningen geladen uit database");
                    }
                    else
                    {
                        // Geen spaarrekeningen gevonden, toon voorbeeld data voor test
                        Spaarrekeningen.Add(new Rekening
                        {
                            Id = 1,
                            Iban = "BE12 3456 7890 1234",
                            Type = RekeningType.Spaar,
                            Saldo = 5000.00m,
                            GebruikerId = 1
                        });
                        Spaarrekeningen.Add(new Rekening
                        {
                            Id = 2,
                            Iban = "BE98 7654 3210 9876",
                            Type = RekeningType.Spaar,
                            Saldo = 12500.50m,
                            GebruikerId = 1
                        });
                        Console.WriteLine("⚠️ Geen spaarrekeningen in database - voorbeeld data getoond");
                    }

                    // Bind aan de ItemsControl in XAML
                    SpaarrekiningenListBox.ItemsSource = Spaarrekeningen;
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