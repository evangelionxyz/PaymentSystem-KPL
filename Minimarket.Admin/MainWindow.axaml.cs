using Avalonia.Controls;
using Avalonia.Interactivity;
using Desktop.Avalonia.Views;
using Desktop.Avalonia.ViewModels;
using Minimarket.Core.Models;
using Minimarket.Core.States;
using System;
using System.Linq;

namespace Minimarket.Cashier;

public partial class MainWindow : Window
{
    private CashierViewModel Vm => (CashierViewModel)DataContext!;

    private AuthView _authPage = null!;
    private Grid _dashboardGrid = null!;
    
    // Product inputs
    private TextBox _txtProdId = null!;
    private TextBox _txtProdName = null!;
    private ComboBox _comboProdCategory = null!;
    private TextBox _txtProdPrice = null!;
    private TextBox _txtProdStock = null!;

    // Replay cursor
    private int _replayIndex = -1;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new CashierViewModel();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        _authPage = this.FindControl<AuthView>("AuthPage")!;
        _dashboardGrid = this.FindControl<Grid>("DashboardGrid")!;

        _txtProdId = this.FindControl<TextBox>("TxtProdId")!;
        _txtProdName = this.FindControl<TextBox>("TxtProdName")!;
        _comboProdCategory = this.FindControl<ComboBox>("ComboProdCategory")!;
        _txtProdPrice = this.FindControl<TextBox>("TxtProdPrice")!;
        _txtProdStock = this.FindControl<TextBox>("TxtProdStock")!;

        _authPage.AuthCompleted += async (s, args) =>
        {
            _authPage.IsVisible = false;
            _dashboardGrid.IsVisible = true;

            await Vm.LoadPendingCartsAsync();
            await Vm.LoadProductsAsync();
        };

        WireEvents();
    }

    private void WireEvents()
    {
        // Refresh Queue
        this.FindControl<Button>("RefreshQueueButton")!.Click += async (s, e) =>
        {
            await Vm.LoadPendingCartsAsync();
        };

        // FSM actions
        this.FindControl<Button>("BtnPayCash")!.Click += async (s, e) => await Vm.TriggerTransitionAsync("Pay_Cash");
        this.FindControl<Button>("BtnPayQr")!.Click += async (s, e) => await Vm.TriggerTransitionAsync("Pay_QR");
        this.FindControl<Button>("BtnSuccess")!.Click += async (s, e) => await Vm.TriggerTransitionAsync("Success");
        this.FindControl<Button>("BtnFail")!.Click += async (s, e) => await Vm.TriggerTransitionAsync("Fail");

        // Product CRUD Form actions
        this.FindControl<Button>("BtnClearProduct")!.Click += (s, e) => ClearProductForm();
        this.FindControl<Button>("BtnSaveProduct")!.Click += async (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(_txtProdName.Text) || 
                _comboProdCategory.SelectedItem == null ||
                !decimal.TryParse(_txtProdPrice.Text, out var price) ||
                !int.TryParse(_txtProdStock.Text, out var stock))
            {
                return;
            }

            var category = (_comboProdCategory.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Snack";
            var prod = new Product
            {
                Name = _txtProdName.Text,
                CategoryId = category,
                Price = price,
                Stock = stock
            };

            if (string.IsNullOrEmpty(_txtProdId.Text))
            {
                await Vm.AddProductAsync(prod);
            }
            else
            {
                prod.ID = _txtProdId.Text;
                await Vm.UpdateProductAsync(prod);
            }

            ClearProductForm();
        };

        // ListBox selection to edit
        var prodListBox = this.FindControl<ListBox>("ProductsListBox")!;
        prodListBox.SelectionChanged += (s, e) =>
        {
            if (prodListBox.SelectedItem is Product p)
            {
                _txtProdId.Text = p.ID;
                _txtProdName.Text = p.Name;
                _txtProdPrice.Text = p.Price.ToString();
                _txtProdStock.Text = p.Stock.ToString();
                
                // Select category in combo box
                foreach (var itemObj in _comboProdCategory.Items)
                {
                    if (itemObj is ComboBoxItem item && item.Content?.ToString() == p.CategoryId)
                    {
                        _comboProdCategory.SelectedItem = item;
                        break;
                    }
                }
            }
        };

        // Delete button inside ListBox via Bubble event handler
        prodListBox.AddHandler(Button.ClickEvent, async (object? sender, RoutedEventArgs e) =>
        {
            if (e.Source is Button btn && btn.Name == "BtnDeleteProduct" && btn.Tag is Product p)
            {
                await Vm.DeleteProductAsync(p.ID!);
                ClearProductForm();
            }
        });

        // Audit Logs loading
        this.FindControl<Button>("BtnLoadAuditLogs")!.Click += async (s, e) =>
        {
            if (!string.IsNullOrEmpty(Vm.AuditTransactionId))
            {
                await Vm.LoadAuditLogsAsync(Vm.AuditTransactionId);
                _replayIndex = Vm.AuditLogs.Count - 1; // Point to latest
            }
        };

        // Replay timeline logic (Backward / Forward)
        this.FindControl<Button>("BtnReplayPrev")!.Click += (s, e) =>
        {
            if (Vm.AuditLogs.Count > 0 && _replayIndex > 0)
            {
                _replayIndex--;
                UpdateReplayState();
            }
        };

        this.FindControl<Button>("BtnReplayNext")!.Click += (s, e) =>
        {
            if (Vm.AuditLogs.Count > 0 && _replayIndex < Vm.AuditLogs.Count - 1)
            {
                _replayIndex++;
                UpdateReplayState();
            }
        };
    }

    private void UpdateReplayState()
    {
        if (_replayIndex >= 0 && _replayIndex < Vm.AuditLogs.Count)
        {
            var log = Vm.AuditLogs[_replayIndex];
            if (Enum.TryParse<TransactionState>(log.ToState, out var state))
            {
                Vm.CurrentState = state;
            }
        }
    }

    private void ClearProductForm()
    {
        _txtProdId.Text = string.Empty;
        _txtProdName.Text = string.Empty;
        _txtProdPrice.Text = string.Empty;
        _txtProdStock.Text = string.Empty;
        _comboProdCategory.SelectedItem = null;
    }
}