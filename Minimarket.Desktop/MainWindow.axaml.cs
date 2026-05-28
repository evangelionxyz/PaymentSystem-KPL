using Avalonia.Controls;
using Avalonia.Interactivity;
using Desktop.Avalonia.ViewModels;
using Desktop.Avalonia.Views;
using Minimarket.Core.Models;

namespace Desktop.Avalonia;

public partial class MainWindow : Window
{
    private MainWindowViewModel Vm => (MainWindowViewModel)DataContext!;

    // Page references
    private ProductScanView _scanPage = null!;
    private CartView        _cartPage = null!;
    private PaymentView     _payPage  = null!;
    private ReceiptView     _rcptPage = null!;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        // Grab named controls
        _scanPage = this.FindControl<ProductScanView>("ProductScanPage")!;
        _cartPage = this.FindControl<CartView>("CartPage")!;
        _payPage  = this.FindControl<PaymentView>("PaymentPage")!;
        _rcptPage = this.FindControl<ReceiptView>("ReceiptPage")!;

        // Hide offline banner if connected
        this.FindControl<Border>("OfflineBanner")!.IsVisible = false;

        await Vm.InitializeAsync();

        if (!Vm.Cache.IsConnected)
            this.FindControl<Border>("OfflineBanner")!.IsVisible = true;

        WireButtons();
        ShowPage(AppPage.ProductScan);
    }

    // ── Navigation ────────────────────────────────────────────────────────────

    private void ShowPage(AppPage page)
    {
        _scanPage.IsVisible = page == AppPage.ProductScan;
        _cartPage.IsVisible = page == AppPage.Cart;
        _payPage.IsVisible  = page == AppPage.Payment;
        _rcptPage.IsVisible = page == AppPage.Receipt;
    }

    // ── Button Wiring ─────────────────────────────────────────────────────────

    private void WireButtons()
    {
        // ProductScanView — "View Cart →"
        var viewCartBtn = _scanPage.FindControl<Button>("ViewCartButton")!;
        viewCartBtn.Click += (_, _) =>
        {
            Vm.CartVm.RecomputeTotalsLocally();
            ShowPage(AppPage.Cart);
        };

        // ProductScanView — individual "Add" buttons via ListBox
        var productList = _scanPage.FindControl<ListBox>("ProductList")!;
        productList.AddHandler(Button.ClickEvent, (object? sender, RoutedEventArgs e) =>
        {
            if (e.Source is Button btn && btn.Tag is Product product)
                _ = Vm.ProductScanVm.AddToCartAsync(product);
        });

        // CartView
        _cartPage.FindControl<Button>("BackButton")!.Click    += (_, _) => ShowPage(AppPage.ProductScan);
        _cartPage.FindControl<Button>("ConfirmButton")!.Click += (_, _) =>
        {
            Vm.GoToPayment();
            ShowPage(AppPage.Payment);
        };

        // PaymentView — method selection via ListBox
        var methodList = _payPage.FindControl<ListBox>("MethodList")!;
        methodList.SelectionChanged += (_, e) =>
        {
            if (methodList.SelectedItem is PaymentMethod m)
                Vm.PaymentVm.SelectedMethod = m;
        };

        _payPage.FindControl<Button>("BackButton")!.Click += (_, _) => ShowPage(AppPage.Cart);
        _payPage.FindControl<Button>("PayButton")!.Click  += async (_, _) =>
        {
            var receipt = await Vm.PaymentVm.ProcessPaymentAsync();
            if (receipt is not null)
            {
                Vm.ReceiptVm.LoadReceipt(receipt);
                ShowPage(AppPage.Receipt);
            }
        };

        // ReceiptView — "New Transaction"
        _rcptPage.FindControl<Button>("NewTransactionButton")!.Click += (_, _) =>
        {
            Vm.StartNewTransaction();
            ShowPage(AppPage.ProductScan);
        };
    }
}