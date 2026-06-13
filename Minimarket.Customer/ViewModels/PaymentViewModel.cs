using System.ComponentModel;
using System.Runtime.CompilerServices;
using Minimarket.Core.Models;
using Minimarket.Core.Services;

namespace Desktop.Avalonia.ViewModels;

public class PaymentViewModel : INotifyPropertyChanged
{
    private readonly ApiClient _api;
    private readonly CartViewModel _cartVm;

    private PaymentMethod _selectedMethod = PaymentMethod.Cash;
    private decimal _feeAmount;
    private decimal _finalTotal;
    private string _errorMessage = string.Empty;

    public PaymentMethod[] PaymentMethods { get; } =
        Enum.GetValues<PaymentMethod>();

    public PaymentMethod SelectedMethod
    {
        get => _selectedMethod;
        set
        {
            _selectedMethod = value;
            OnPropertyChanged();
            RecalculateFee();
        }
    }

    public decimal FeeAmount   { get => _feeAmount;   set { _feeAmount = value;   OnPropertyChanged(); } }
    public decimal FinalTotal  { get => _finalTotal;  set { _finalTotal = value;  OnPropertyChanged(); } }
    public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }

    private readonly Dictionary<string, decimal> _fees;

    public PaymentViewModel(ApiClient api, CartViewModel cartVm, Dictionary<string, decimal> fees)
    {
        _api    = api;
        _cartVm = cartVm;
        _fees   = fees;
        RecalculateFee();
    }

    private void RecalculateFee()
    {
        var feeRate = _fees.TryGetValue(SelectedMethod.ToString(), out var r) ? r : 0m;
        FeeAmount  = Math.Round(_cartVm.Total * feeRate, 2);
        FinalTotal = _cartVm.Total + FeeAmount;
    }

    public async Task<Receipt?> ProcessPaymentAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
            _cartVm.TriggerFsm("PaymentSelected");
            var receipt = await _api.ProcessPaymentAsync(_cartVm.CartId, SelectedMethod);
            if (receipt is not null)
                _cartVm.TriggerFsm("PaymentConfirmed");
            return receipt;
        }
        catch (Exception ex)
        {
            _cartVm.TriggerFsm("PaymentFailed");
            ErrorMessage = $"Payment failed: {ex.Message}";
            return null;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
