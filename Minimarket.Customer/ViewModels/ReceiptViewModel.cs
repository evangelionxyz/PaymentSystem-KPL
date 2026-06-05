using System.ComponentModel;
using System.Runtime.CompilerServices;
using Minimarket.Core.Models;
using Desktop.Avalonia.Services;

namespace Desktop.Avalonia.ViewModels;

public class ReceiptViewModel : INotifyPropertyChanged
{
    private readonly CartViewModel _cartVm;
    private Receipt? _receipt;

    public Receipt? Receipt
    {
        get => _receipt;
        set
        {
            _receipt = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasReceipt));
            OnPropertyChanged(nameof(ItemsDisplay));
        }
    }

    public bool HasReceipt => _receipt is not null;

    public string ItemsDisplay => _receipt is null ? string.Empty :
        string.Join("\n", _receipt.Items.Select(i =>
            $"  {i.ProductName} x{i.Quantity}  @ {i.UnitPrice:N0}  = {i.LineTotal:N0}"));

    public ReceiptViewModel(CartViewModel cartVm)
    {
        _cartVm = cartVm;
    }

    public void LoadReceipt(Receipt receipt)
    {
        Receipt = receipt;
    }

    /// <summary>Resets the FSM and cart for a new transaction.</summary>
    public void StartNewTransaction()
    {
        _cartVm.TriggerFsm("Reset");
        _cartVm.Reset();
        Receipt = null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
