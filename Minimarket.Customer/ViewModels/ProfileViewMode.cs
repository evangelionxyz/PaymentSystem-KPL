using Minimarket.Core.Services;
using Minimarket.Core.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Minimarket.Customer.ViewModels;

internal class ProfileViewMode : INotifyPropertyChanged
{
    private readonly ApiClient _api;

    private User? _authUser;
    public User? AuthenticatedUser
    {
        get => _authUser;
        set { _authUser = value; OnPropertyChanged(); }
    }

    public ProfileViewMode(ApiClient api)
    {
        _api = api;

    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
