using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BankApp_WPF;

public partial class SaldoRaadplegenPagina : Window
{
    private bool _voiceEnabled = true;

    public SaldoRaadplegenPagina()
    {
        InitializeComponent();
        LoadMockData();
    }

    private void LoadMockData()
    {
        // Mock Balance Data
        lblCurrentBalance.Content = "€ 12.847,50";
        lblAccountInfo.Content = "Zichtrekening •••• 4892";

        txtBalanceChangeIcon.Text = "📈";
        lblBalanceChange.Content = "+ € 1.197,50 (+10.3%)";
        lblBalanceChange.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4ADE80"));

        // Mock Transactions
        LoadMockTransactions();
    }

    private void LoadMockTransactions()
    {
        TransactionsPanel.Children.Clear();

        var mockTransactions = new[]
        {
            new { Description = "Loon - Werkgever BV", Amount = 2850.00m, Date = "27 oktober 2024", Balance = 12847.50m, IsCredit = true },
            new { Description = "Supermarkt Delhaize", Amount = -89.45m, Date = "26 oktober 2024", Balance = 9997.50m, IsCredit = false },
            new { Description = "Huur - Appartement", Amount = -950.00m, Date = "25 oktober 2024", Balance = 10086.95m, IsCredit = false },
            new { Description = "Terugbetaling - Belastingen", Amount = 345.60m, Date = "24 oktober 2024", Balance = 11036.95m, IsCredit = true },
            new { Description = "Elektriciteit - Engie", Amount = -156.30m, Date = "23 oktober 2024", Balance = 10691.35m, IsCredit = false },
            new { Description = "Restaurant La Trattoria", Amount = -67.50m, Date = "22 oktober 2024", Balance = 10847.65m, IsCredit = false },
            new { Description = "Terugbetaling - Verzekering", Amount = 120.00m, Date = "21 oktober 2024", Balance = 10915.15m, IsCredit = true },
            new { Description = "Tankstation Shell", Amount = -75.80m, Date = "20 oktober 2024", Balance = 10795.15m, IsCredit = false },
        };

        for (int i = 0; i < mockTransactions.Length; i++)
        {
            var transaction = mockTransactions[i];
            var button = CreateTransactionButton(
                transaction.Description,
                transaction.Amount,
                transaction.Date,
                transaction.Balance,
                transaction.IsCredit,
                i,
                mockTransactions.Length
            );
            TransactionsPanel.Children.Add(button);

            // Add separator except for last item
            if (i < mockTransactions.Length - 1)
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

    private Button CreateTransactionButton(string description, decimal amount, string date, decimal balance, bool isCredit, int index, int total)
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

        // Right side - Amount and Balance
        var rightStack = new StackPanel { HorizontalAlignment = HorizontalAlignment.Right };

        var amountText = new TextBlock
        {
            Text = $"{(isCredit ? "+" : "")}€ {amount:N2}",
            FontSize = 24,
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(isCredit ? "#4ADE80" : "#F87171")),
            Margin = new Thickness(0, 0, 0, 4)
        };
        rightStack.Children.Add(amountText);

        var balanceText = new TextBlock
        {
            Text = $"Saldo: € {balance:N2}",
            FontSize = 18,
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"))
        };
        rightStack.Children.Add(balanceText);

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
            // TODO: Implementeer text-to-speech later
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
        // Open specifiek venster bij indrukken van Z
        HoofdPagina hoofdPagina = new HoofdPagina();
        hoofdPagina.Show();
        this.Close();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        // Global keyboard shortcut: Z to go back
        if (e.Key == Key.Z)
        {

            // Open specifiek venster bij indrukken van Z
            HoofdPagina hoofdPagina = new HoofdPagina();
            hoofdPagina.Show();

            // Sluit huidige venster
            this.Close();
        }
    }
}
