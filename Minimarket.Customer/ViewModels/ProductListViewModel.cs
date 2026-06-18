using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Minimarket.Core.Models;
using Minimarket.Core.Services;

namespace Desktop.Avalonia.ViewModels;

public class ProductListViewModel : INotifyPropertyChanged
{
    private readonly ApiClient _api;
    private readonly PricingConfigCache _cache;
    private readonly CartViewModel _cartVm;

    private List<Product> _allProducts = new();
    private string _searchText = string.Empty;
    private string _statusMessage = string.Empty;

    public ObservableCollection<Product> FilteredProducts { get; } = new();

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            FilterProducts();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }

    public string FsmState => _cartVm.FsmStateDisplay;

    public ProductListViewModel(ApiClient api, PricingConfigCache cache, CartViewModel cartVm)
    {
        _api   = api;
        _cache = cache;
        _cartVm = cartVm;
    }

    public async Task LoadProductsAsync()
    {
        try
        {
            _allProducts = await _api.GetProductsAsync();
            FilterProducts();
            StatusMessage = _cache.IsConnected
                ? $"Loaded {_allProducts.Count} products."
                : "API offline — using cached rules, no discounts.";
        }
        catch
        {
            StatusMessage = "Could not load products from API.";
        }
    }

    private void FilterProducts()
    {
        FilteredProducts.Clear();
        var query = _searchText.Trim().ToLowerInvariant();
        var matches = string.IsNullOrEmpty(query)
            ? _allProducts
            : _allProducts.Where(p =>
                p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                p.ID!.Contains(query));

        foreach (var p in matches)
            FilteredProducts.Add(p);
    }

    public async Task AddToCartAsync(Product product)
    {
        await _cartVm.AddItemAsync(product);
        OnPropertyChanged(nameof(FsmState));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
