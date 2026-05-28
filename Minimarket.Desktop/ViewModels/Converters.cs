using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Desktop.Avalonia.ViewModels;

/// <summary>Converts bool → active/inactive background color for step indicator.</summary>
public class BoolToActiveColorConverter : IValueConverter
{
    public static readonly BoolToActiveColorConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var active = value is true;
        return new SolidColorBrush(active ? Color.Parse("#1565C0") : Color.Parse("#1E1E35"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

/// <summary>Returns true when a string is not null or empty.</summary>
public class StringNotEmptyConverter : IValueConverter
{
    public static readonly StringNotEmptyConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        !string.IsNullOrEmpty(value as string);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

/// <summary>Allows PaymentMethod RadioButton binding in ItemsControl.</summary>
public class PaymentMethodConverter : IValueConverter
{
    public static readonly PaymentMethodConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value?.Equals(parameter) ?? false;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is true ? parameter : null;
}
