using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace Desktop.Avalonia.Views;

public partial class AuthView : UserControl
{
    public event EventHandler<RoutedEventArgs>? AuthCompleted;

    public AuthView()
    {
        InitializeComponent();
        
        var actionBtn = this.FindControl<Button>("AuthActionButton");
        if (actionBtn != null)
        {
            actionBtn.Click += async (sender, e) =>
            {
                if (DataContext is ViewModels.AuthViewModel vm)
                {
                    var success = await vm.ExecuteAuthActionAsync();
                    if (success && vm.AuthenticatedUser != null)
                    {
                        AuthCompleted?.Invoke(this, new RoutedEventArgs());
                    }
                }
            };
        }

        var switchBtn = this.FindControl<Button>("SwitchModeButton");
        if (switchBtn != null)
        {
            switchBtn.Click += (sender, e) =>
            {
                if (DataContext is ViewModels.AuthViewModel vm)
                {
                    vm.IsLoginMode = !vm.IsLoginMode;
                }
            };
        }
    }
}
