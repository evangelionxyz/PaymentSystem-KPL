using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Minimarket.Core.Models;
using Minimarket.Core.States;
using Minimarket.Core.Services;

namespace Desktop.Avalonia.ViewModels;

public class CashierViewModel : INotifyPropertyChanged
{
    private readonly ApiClient _api;
    public AuthViewModel AuthVm { get; }
    public ObservableCollection<Cart> PendingCarts { get; } = new();
    public ObservableCollection<Product> Products { get; } = new();
    public ObservableCollection<AuditLog> AuditLogs { get; } = new();
    private Cart? _selectedCart;
    public Cart? SelectedCart
    {
        get => _selectedCart;
        set 
        { 
            _selectedCart = value; 
            OnPropertyChanged(); 
            if (value != null)
            {
                // Init FSM for this transaction
                CurrentState = TransactionState.AwaitingPayment; // Typically customer has checked out
            }
        }
    }

    private TransactionState _currentState = TransactionState.Idle;
    public TransactionState CurrentState
    {
        get => _currentState;
        set { _currentState = value; OnPropertyChanged(); }
    }

    private string _auditTransactionId = string.Empty;
    public string AuditTransactionId
    {
        get => _auditTransactionId;
        set { _auditTransactionId = value; OnPropertyChanged(); }
    }

    public CashierViewModel()
    {
        _api = new ApiClient();
        AuthVm = new AuthViewModel(_api);
    }

    public async Task LoadPendingCartsAsync()
    {
        try
        {
            var carts = await _api.GetPendingCartsAsync();
            PendingCarts.Clear();
            foreach (var c in carts) PendingCarts.Add(c);
        }
        catch { }
    }

    public async Task LoadProductsAsync()
    {
        try
        {
            var products = await _api.GetProductsAsync();
            Products.Clear();
            foreach (var p in products) Products.Add(p);
        }
        catch { }
    }

    public async Task LoadAuditLogsAsync(string txId)
    {
        try
        {
            var logs = await _api.GetAuditLogsAsync(txId);
            AuditLogs.Clear();
            foreach (var log in logs) AuditLogs.Add(log);
        }
        catch { }
    }

    public async Task TriggerTransitionAsync(string trigger)
    {
        if (SelectedCart == null) return;

        // Save audit log
        var log = new AuditLog
        {
            TransactionId = SelectedCart.ID!,
            FromState = CurrentState.ToString(),
            Trigger = trigger
        };

        if (CurrentState == TransactionState.AwaitingPayment && (trigger == "Pay_Cash" || trigger == "Pay_QR"))
        {
            CurrentState = TransactionState.ProcessingPayment;
        }
        else if (CurrentState == TransactionState.ProcessingPayment && trigger == "Success")
        {
            var method = trigger == "Pay_QR" ? PaymentMethod.QRIS : PaymentMethod.Cash;
            await _api.ProcessPaymentAsync(SelectedCart.ID!, method, SelectedCart.CustomerId);
            CurrentState = TransactionState.Completed;
        }
        else if (CurrentState == TransactionState.ProcessingPayment && trigger == "Fail")
        {
            CurrentState = TransactionState.AwaitingPayment;
        }

        log.ToState = CurrentState.ToString();
        await _api.LogTransitionAsync(log);
    }

    // Product CRUD wrapper methods
    public async Task AddProductAsync(Product p)
    {
        var created = await _api.CreateProductAsync(p);
        if (created != null)
        {
            Products.Add(created);
        }
    }

    public async Task UpdateProductAsync(Product p)
    {
        var success = await _api.UpdateProductAsync(p.ID!, p);
        if (success)
        {
            var index = Products.IndexOf(Products.First(x => x.ID == p.ID));
            if (index >= 0)
            {
                Products[index] = p;
            }
        }
    }

    public async Task DeleteProductAsync(string id)
    {
        var success = await _api.DeleteProductAsync(id);
        if (success)
        {
            var item = Products.FirstOrDefault(x => x.ID == id);
            if (item != null)
            {
                Products.Remove(item);
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
