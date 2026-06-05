using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Minimarket.Core.Models;
using Desktop.Avalonia.Services;

namespace Desktop.Avalonia.ViewModels;

public class AuthViewModel : INotifyPropertyChanged
{
    private readonly ApiClient _api;

    private string _username = string.Empty;
    public string Username
    {
        get => _username;
        set { _username = value; OnPropertyChanged(); }
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    private bool _isLoginMode = true;
    public bool IsLoginMode
    {
        get => _isLoginMode;
        set 
        { 
            _isLoginMode = value; 
            OnPropertyChanged(); 
            OnPropertyChanged(nameof(IsRegisterMode));
            OnPropertyChanged(nameof(TitleText));
            OnPropertyChanged(nameof(SwitchButtonText));
            OnPropertyChanged(nameof(ActionButtonText));
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }
    }

    public bool IsRegisterMode => !IsLoginMode;
    public string TitleText => IsLoginMode ? "Welcome Back" : "Create Account";
    public string RoleText = "Customer";
    public string ActionButtonText => IsLoginMode ? "Sign In" : "Register";
    public string SwitchButtonText => IsLoginMode ? "Don't have an account? Register" : "Already have an account? Sign In";

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(); }
    }

    private string _successMessage = string.Empty;
    public string SuccessMessage
    {
        get => _successMessage;
        set { _successMessage = value; OnPropertyChanged(); }
    }

    private User? _authenticatedUser;
    public User? AuthenticatedUser
    {
        get => _authenticatedUser;
        set { _authenticatedUser = value; OnPropertyChanged(); }
    }

    public AuthViewModel(ApiClient api)
    {
        _api = api;
    }

    public async Task<bool> ExecuteAuthActionAsync()
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Username and password are required.";
            return false;
        }

        try
        {
            if (IsLoginMode)
            {
                // Login
                var response = await _api.LoginAsync(Username, Password);
                if (response != null)
                {
                    AuthenticatedUser = response;
                    SuccessMessage = "Login successful!";
                    return true;
                }
                else
                {
                    ErrorMessage = "Invalid credentials.";
                }
            }
            else
            {
                // Register
                var newUser = new User { Username = Username, Password = Password, Role = RoleText };
                var response = await _api.RegisterAsync(newUser);
                if (response != null)
                {
                    SuccessMessage = "Account registered successfully! You can now log in.";
                    IsLoginMode = true; // Switch back to login
                }
                else
                {
                    ErrorMessage = "Registration failed.";
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }

        return false;
    }

    public void Logout()
    {
        AuthenticatedUser = null;
        Username = string.Empty;
        Password = string.Empty;
        SuccessMessage = string.Empty;
        ErrorMessage = string.Empty;
        RoleText = "Customer";
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
