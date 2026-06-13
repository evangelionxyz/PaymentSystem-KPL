using System.ComponentModel;
using System.Runtime.CompilerServices;
using Desktop.Avalonia.Services;
using Minimarket.Core.Services;
using MongoDB.Bson;

namespace Desktop.Avalonia.ViewModels;

public enum AppPage
{ 
    ProductList, 
    Cart, 
    Payment, 
    Receipt
}

public class MainWindowViewModel : INotifyPropertyChanged
{
    public ApiClient Api { get; }
    public PricingConfigCache Cache { get; }
    public ProductListViewModel ProductListVm { get; }
    public CartViewModel CartVm { get; }
    public PaymentViewModel PaymentVm { get; }
    public ReceiptViewModel ReceiptVm { get; }
    public AuthViewModel AuthVm { get; }

    public bool IsAuthenticated => AuthVm.AuthenticatedUser != null;

    private AppPage _currentPage = AppPage.ProductList;
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

    public bool IsProductScan => CurrentPage == AppPage.ProductList;
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
        ProductListVm = new ProductListViewModel(Api, Cache, CartVm);
        PaymentVm     = new PaymentViewModel(Api, CartVm, new Dictionary<string, decimal>
        {
            ["Cash"] = 0m, 
            ["EWallet"] = 0.005m, 
            ["BankTransfer"] = 0.003m,
            ["QRIS"] = 0.007m, 
            ["CreditCard"] = 0.015m,
        });
        ReceiptVm = new ReceiptViewModel(CartVm);

        AuthVm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(AuthVm.AuthenticatedUser))
            {
                OnPropertyChanged(nameof(IsAuthenticated));
                if (IsAuthenticated)
                {
                    CartVm.CartId = ObjectId.GenerateNewId().ToString();
                    CartVm.IsVip = AuthVm.Username.EndsWith("vip", StringComparison.OrdinalIgnoreCase);
                }
            }
        };
    }

    public async Task InitializeAsync()
    {
        await Cache.LoadAsync();
        await ProductListVm.LoadProductsAsync();
        OnPropertyChanged(nameof(ConnectivityBanner));
    }

    // Navigation
    public void GoToProductScan()  => CurrentPage = AppPage.ProductList;
    public void GoToCart() => CurrentPage = AppPage.Cart;
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
        CurrentPage = AppPage.ProductList;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
