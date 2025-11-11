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
        _transactieService = new TransactieService(context);

        // Load real data
        LoadRealData();
    }

    private async void LoadRealData()
    {
        if (!SessionManager.IsLoggedIn)
        {
            MessageBox.Show("Je moet ingelogd zijn om je saldo te bekijken.",
                "Niet ingelogd", MessageBoxButton.OK, MessageBoxImage.Warning);
            this.Close();
            return;
        }

        try
        {
            var gebruikerId = SessionManager.CurrentUser!.Id;

            // Haal rekeningen en totaal saldo op
            var rekeningen = await _rekeningService.GetRekeningenByGebruikerIdAsync(gebruikerId);
            var totaalSaldo = await _rekeningService.GetTotaalSaldoAsync(gebruikerId);

            // Update saldo display
            lblCurrentBalance.Content = $"€ {totaalSaldo:N2}";

            // Toon eerste zichtrekening info
            if (rekeningen.Any())
            {
                var hoofdRekening = rekeningen.FirstOrDefault(r => r.Type == RekeningType.Zicht);
                if (hoofdRekening != null)
                {
                    string maskedIban = hoofdRekening.Iban.Length > 4
                        ? "•••• " + hoofdRekening.Iban.Substring(hoofdRekening.Iban.Length - 4)
                        : hoofdRekening.Iban;
                    lblAccountInfo.Content = $"Zichtrekening {maskedIban}";
                }
            }

            // TODO: Bereken saldoverandering (vereist historische data)
            // Voor nu: dummy waardes
            txtBalanceChangeIcon.Text = totaalSaldo > 1000 ? "📈" : "📉";
            lblBalanceChange.Content = totaalSaldo > 1000
                ? "+ € 197,50 (+2.3%)"
                : "- € 50,00 (-0.5%)";
            lblBalanceChange.Foreground = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(totaalSaldo > 1000 ? "#4ADE80" : "#F87171"));

            // Haal transacties op
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

            // Bepaal of dit een credit (ontvangen) of debit (betaald) is
            bool isCredit = gebruikerIbans.Contains(t.NaarIban);

            // Bereken weergave bedrag
            decimal displayAmount = isCredit ? t.Bedrag : -t.Bedrag;

            // Beschrijving
            string description = string.IsNullOrWhiteSpace(t.Omschrijving)
                ? (isCredit ? "Ontvangst" : "Betaling")
                : t.Omschrijving;

            var button = CreateTransactionButton(
                description,
                displayAmount,
                t.Datum.ToString("dd MMMM yyyy"),
                0, // Saldo wordt niet bewaard per transactie
                isCredit,
                i,
                transacties.Count
            );
            TransactionsPanel.Children.Add(button);

            // Separator tussen transacties
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

        // Set automation properties
        var transactionType = isCredit ? "ontvangen" : "betaald";
        AutomationProperties.SetName(button,
            $"Transactie {index + 1} van {total}: {description}, " +
            $"{transactionType} € {Math.Abs(amount):N2}, {date}");

        // Create content grid
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        // Left side - Description and Date
        var leftStack = new StackPanel();

        var descriptionPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };

        // Icon
        var icon = new TextBlock
        {
            Text = isCredit ? "↙" : "↗",
            FontSize = 32,
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(isCredit ? "#4ADE80" : "#F87171")),
            Margin = new Thickness(0, 0, 12, 0),
            VerticalAlignment = VerticalAlignment.Center
        };
        descriptionPanel.Children.Add(icon);

        // Description
        var descriptionText = new TextBlock
        {
            Text = description,
            FontSize = 20,
            Foreground = Brushes.White,
            VerticalAlignment = VerticalAlignment.Center
        };
        descriptionPanel.Children.Add(descriptionText);

        leftStack.Children.Add(descriptionPanel);

        // Date
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

        // Right side - Amount
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

        // Event handlers
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

        // Focus new transaction (skip separators: index * 2)
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
        this.Close();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Z)
        {
            e.Handled = true;
            this.Close();
        }
    }
}