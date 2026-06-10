using System.ComponentModel;
using System.Runtime.CompilerServices;
using Desktop.Avalonia.Services;
using Minimarket.Core.Services;

namespace Desktop.Avalonia.ViewModels;

public enum AppPage
{ 
    ProductScan, 
    Cart, 
    Payment, 
    Receipt
}

public class MainWindowViewModel : INotifyPropertyChanged
{
    public ApiClient Api { get; }
    public PricingConfigCache Cache { get; }
    public ProductScanViewModel ProductScanVm { get; }
    public CartViewModel CartVm { get; }
    public PaymentViewModel PaymentVm { get; }
    public ReceiptViewModel ReceiptVm { get; }
    public AuthViewModel AuthVm { get; }

    public bool IsAuthenticated => AuthVm.AuthenticatedUser != null;

    private AppPage _currentPage = AppPage.ProductScan;
    public AppPage CurrentPage
    {
        get => _currentPage;
        set
        {
            _currentPage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsProductScan));
            OnPropertyChanged(nameof(IsCart));
            OnPropertyChanged(nameof(IsPayment));
            OnPropertyChanged(nameof(IsReceipt));
        }
    }

    public bool IsProductScan => CurrentPage == AppPage.ProductScan;
    public bool IsCart        => CurrentPage == AppPage.Cart;
    public bool IsPayment     => CurrentPage == AppPage.Payment;
    public bool IsReceipt     => CurrentPage == AppPage.Receipt;

    public string ConnectivityBanner => Cache.IsConnected ? string.Empty : "API offline — operating with base prices only";

    public MainWindowViewModel()
    {
        Api   = new ApiClient();
        Cache = new PricingConfigCache(Api);

        AuthVm        = new AuthViewModel(Api);
        CartVm        = new CartViewModel(Api, Cache);
        ProductScanVm = new ProductScanViewModel(Api, Cache, CartVm);
        PaymentVm     = new PaymentViewModel(Api, CartVm, new Dictionary<string, decimal>
        {
            ["Cash"] = 0m, ["EWallet"] = 0.005m, ["BankTransfer"] = 0.003m,
            ["QRIS"] = 0.007m, ["CreditCard"] = 0.015m,
        });
        ReceiptVm = new ReceiptViewModel(CartVm);

        AuthVm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(AuthVm.AuthenticatedUser))
            {
                OnPropertyChanged(nameof(IsAuthenticated));
                if (IsAuthenticated)
                {
                    CartVm.CartId = Guid.NewGuid().ToString();
                    CartVm.IsVip = AuthVm.Username.EndsWith("vip", StringComparison.OrdinalIgnoreCase);
                }
            }
        };
    }

    public async Task InitializeAsync()
    {
        await Cache.LoadAsync();
        await ProductScanVm.LoadProductsAsync();
        OnPropertyChanged(nameof(ConnectivityBanner));
    }

    // Navigation
    public void GoToProductScan()  => CurrentPage = AppPage.ProductScan;
    public void GoToCart()         => CurrentPage = AppPage.Cart;

    public void GoToPayment()
    {
        CartVm.TriggerFsm("CartConfirmed");
        CurrentPage = AppPage.Payment;
    }

    public async Task ProcessPaymentAndGoToReceipt()
    {
        var receipt = await PaymentVm.ProcessPaymentAsync();
        if (receipt is not null)
        {
            ReceiptVm.LoadReceipt(receipt);
            CurrentPage = AppPage.Receipt;
        }
    }

    public void StartNewTransaction()
    {
        ReceiptVm.StartNewTransaction();
        CurrentPage = AppPage.ProductScan;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
