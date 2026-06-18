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
    private ProductListView _productListPage = null!;
    private CartView        _cartPage = null!;
    private PaymentView     _payPage  = null!;
    private ReceiptView     _rcptPage = null!;
    private AuthView        _authPage = null!;
    private Grid            _mainPOSGrid = null!;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        // Grab named controls
        _productListPage = this.FindControl<ProductListView>("ProductListPage")!;
        _cartPage = this.FindControl<CartView>("CartPage")!;
        _payPage  = this.FindControl<PaymentView>("PaymentPage")!;
        _rcptPage = this.FindControl<ReceiptView>("ReceiptPage")!;
        _authPage = this.FindControl<AuthView>("AuthPage")!;
        _mainPOSGrid = this.FindControl<Grid>("MainPOSGrid")!;

        _authPage.AuthCompleted += (sender, args) =>
        {
            _authPage.IsVisible = false;
            _mainPOSGrid.IsVisible = true;
        };

        // Hide offline banner if connected
        this.FindControl<Border>("OfflineBanner")!.IsVisible = false;

        await Vm.InitializeAsync();

        if (!Vm.Cache.IsConnected)
            this.FindControl<Border>("OfflineBanner")!.IsVisible = true;

        WireButtons();
        ShowPage(AppPage.ProductList);
    }

    private void ShowPage(AppPage page)
    {
        _productListPage.IsVisible = page == AppPage.ProductList;
        _cartPage.IsVisible = page == AppPage.Cart;
        _payPage.IsVisible  = page == AppPage.Payment;
        _rcptPage.IsVisible = page == AppPage.Receipt;
    }

    private void WireButtons()
    {
        // ProductListView — "View Cart →"
        var viewCartBtn = _productListPage.FindControl<Button>("ViewCartButton")!;
        viewCartBtn.Click += (_, _) =>
        {
            Vm.CartVm.RecomputeTotalsLocally();
            ShowPage(AppPage.Cart);
        };

        // ProductListView — individual "Add" buttons via ListBox
        var productList = _productListPage.FindControl<ListBox>("ProductList")!;
        productList.AddHandler(Button.ClickEvent, (object? sender, RoutedEventArgs e) =>
        {
            if (e.Source is Button btn && btn.Tag is Product product)
                _ = Vm.ProductListVm.AddToCartAsync(product);
        });

        // CartView
        _cartPage.FindControl<Button>("BackButton")!.Click    += (_, _) => ShowPage(AppPage.ProductList);
        _cartPage.FindControl<Button>("ConfirmButton")!.Click += (_, _) =>
        {
            Vm.GoToPayment();
            ShowPage(AppPage.Payment);
        };

        var cartItemsControl = _cartPage.FindControl<ItemsControl>("CartItemsControl")!;
        cartItemsControl.AddHandler(Button.ClickEvent, (object? sender, RoutedEventArgs e) =>
        {
            if (e.Source is Button btn && btn.Tag is CartItem cartItem)
                _ = Vm.CartVm.RemoveItemAsync(cartItem);
        });

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
            ShowPage(AppPage.ProductList);
        };
    }
}