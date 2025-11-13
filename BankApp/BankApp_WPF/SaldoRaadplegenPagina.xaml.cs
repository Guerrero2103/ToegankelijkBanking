using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BankApp_BusinessLogic;
using BankApp_Models;

namespace BankApp_WPF;

public partial class SaldoRaadplegenPagina : Window
{
    private bool _voiceEnabled = true;
    private readonly IRekeningService _rekeningService;
    private readonly ITransactieService _transactieService;

    public SaldoRaadplegenPagina()
    {
        InitializeComponent();

        // Initialize services
        var context = new AppDbContext();
        _rekeningService = new RekeningService(context);
        _transactieService = new TransactieService(context); // small compatibility: field name must be _transactieService
        _transactieService = new TransactieService(context);

        // Load real data
        LoadRealData();

        this.KeyDown += Window_KeyDown;
        this.Focusable = true;
        this.Focus();
    }

    private async void LoadRealData()
    {
        if (!UserSession.IsIngelogd)
        {
            MessageBox.Show("Je moet ingelogd zijn om je saldo te bekijken.",
                "Niet ingelogd", MessageBoxButton.OK, MessageBoxImage.Warning);
            this.Close();
            return;
        }

        try
        {
            var gebruikerId = UserSession.IngelogdeGebruiker!.Id;

            // Haal rekeningen op; maak zichtrekening aan als er geen bestaan
            var rekeningen = await _rekeningService.GetRekeningenByGebruikerIdAsync(gebruikerId);
            if (rekeningen == null || rekeningen.Count == 0)
            {
                var nieuwe = await _rekeningService.MaakRekeningAanAsync(gebruikerId, RekeningType.Zicht);
                rekeningen = new List<Rekening> { nieuwe };
            }

            var totaalSaldo = await _rekeningService.GetTotaalSaldoAsync(gebruikerId);

            // Update saldo display
            lblCurrentBalance.Content = $"€ {totaalSaldo:N2}";

            // Toon eerste zichtrekening info (volledige IBAN)
            if (rekeningen.Any())
            {
                var hoofdRekening = rekeningen.FirstOrDefault(r => r.Type == RekeningType.Zicht) ?? rekeningen.First();
                lblAccountInfo.Content = $"Zichtrekening {hoofdRekening.Iban}";
            }

            // Rest van de methode: transacties ophalen en tonen
            var transacties = await _transactieService.GetTransactiesByGebruikerIdAsync(gebruikerId, 10);
            if (transacties.Any())
            {
                LoadTransactions(transacties, rekeningen);
            }
            else
            {
                TransactionsPanel.Children.Add(new TextBlock
                {
                    Text = "Nog geen transacties beschikbaar.",
                    FontSize = 18,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(0, 20, 0, 0)
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fout bij laden van gegevens: {ex.Message}",
                "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadTransactions(List<Transactie> transacties, List<Rekening> gebruikerRekeningen)
    {
        TransactionsPanel.Children.Clear();

        var gebruikerIbans = gebruikerRekeningen.Select(r => r.Iban).ToList();

        for (int i = 0; i < transacties.Count; i++)
        {
            var t = transacties[i];

            bool isCredit = gebruikerIbans.Contains(t.NaarIban);
            decimal displayAmount = isCredit ? t.Bedrag : -t.Bedrag;
            string description = string.IsNullOrWhiteSpace(t.Omschrijving)
                ? (isCredit ? "Ontvangst" : "Betaling")
                : t.Omschrijving;

            var button = CreateTransactionButton(
                description,
                displayAmount,
                t.Datum.ToString("dd MMMM yyyy"),
                0,
                isCredit,
                i,
                transacties.Count
            );
            TransactionsPanel.Children.Add(button);

            if (i < transacties.Count - 1)
            {
                var separator = new Separator
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4B5563")),
                    Height = 2,
                    Margin = new Thickness(0, 16, 0, 16)
                };
                TransactionsPanel.Children.Add(separator);
            }
        }
    }

    private Button CreateTransactionButton(string description, decimal amount, string date,
        decimal balance, bool isCredit, int index, int total)
    {
        var button = new Button
        {
            Style = (Style)FindResource("TransactionButton"),
            TabIndex = index
        };

        var transactionType = isCredit ? "ontvangen" : "betaald";
        AutomationProperties.SetName(button,
            $"Transactie {index + 1} van {total}: {description}, " +
            $"{transactionType} € {Math.Abs(amount):N2}, {date}");

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var leftStack = new StackPanel();
        var descriptionPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };

        var icon = new TextBlock
        {
            Text = isCredit ? "↙" : "↗",
            FontSize = 32,
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(isCredit ? "#4ADE80" : "#F87171")),
            Margin = new Thickness(0, 0, 12, 0),
            VerticalAlignment = VerticalAlignment.Center
        };
        descriptionPanel.Children.Add(icon);

        var descriptionText = new TextBlock
        {
            Text = description,
            FontSize = 20,
            Foreground = Brushes.White,
            VerticalAlignment = VerticalAlignment.Center
        };
        descriptionPanel.Children.Add(descriptionText);

        leftStack.Children.Add(descriptionPanel);

        var dateText = new TextBlock
        {
            Text = date,
            FontSize = 18,
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9CA3AF")),
            Margin = new Thickness(44, 0, 0, 0)
        };
        leftStack.Children.Add(dateText);

        Grid.SetColumn(leftStack, 0);
        grid.Children.Add(leftStack);

        var rightStack = new StackPanel { HorizontalAlignment = HorizontalAlignment.Right };
        var amountText = new TextBlock
        {
            Text = $"{(amount >= 0 ? "+" : "")}€ {amount:N2}",
            FontSize = 24,
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(isCredit ? "#4ADE80" : "#F87171")),
            Margin = new Thickness(0, 0, 0, 4)
        };
        rightStack.Children.Add(amountText);

        Grid.SetColumn(rightStack, 1);
        grid.Children.Add(rightStack);

        button.Content = grid;
        button.Click += (s, e) => AnnounceTransaction(description, amount);
        button.KeyDown += (s, e) => HandleTransactionKeyDown(e, index, total);

        return button;
    }

    private void HandleTransactionKeyDown(KeyEventArgs e, int currentIndex, int totalCount)
    {
        int newIndex = currentIndex;
        switch (e.Key)
        {
            case Key.Down:
                e.Handled = true;
                newIndex = currentIndex < totalCount - 1 ? currentIndex + 1 : 0;
                break;
            case Key.Up:
                e.Handled = true;
                newIndex = currentIndex > 0 ? currentIndex - 1 : totalCount - 1;
                break;
            case Key.Enter:
            case Key.Space:
                e.Handled = true;
                return;
            default:
                return;
        }

        var targetIndex = newIndex * 2;
        if (targetIndex < TransactionsPanel.Children.Count &&
            TransactionsPanel.Children[targetIndex] is Button targetButton)
        {
            targetButton.Focus();
        }
    }

    private void AnnounceTransaction(string description, decimal amount)
    {
        if (_voiceEnabled)
        {
            var message = $"{description}, € {Math.Abs(amount):N2}";
            System.Diagnostics.Debug.WriteLine($"Announce: {message}");
        }
    }

    private void BtnVoice_Click(object sender, RoutedEventArgs e)
    {
        _voiceEnabled = !_voiceEnabled;

        if (sender is Button button)
        {
            var textBlock = new TextBlock { Text = _voiceEnabled ? "🔊" : "🔇", FontSize = 28 };
            button.Content = textBlock;
        }
    }

    private void BtnBack_Click(object sender, RoutedEventArgs e)
    {
        HoofdPagina hoofdPagina = new HoofdPagina();
        hoofdPagina.Show();
        this.Close();
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
}