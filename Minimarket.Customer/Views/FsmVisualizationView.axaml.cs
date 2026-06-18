using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Minimarket.Core.States;

namespace Desktop.Avalonia.Views;

public partial class FsmVisualizationView : UserControl
{
    public static readonly StyledProperty<TransactionState> CurrentStateProperty =
        AvaloniaProperty.Register<FsmVisualizationView, TransactionState>(nameof(CurrentState));

    public TransactionState CurrentState
    {
        get => GetValue(CurrentStateProperty);
        set => SetValue(CurrentStateProperty, value);
    }

    public FsmVisualizationView()
    {
        InitializeComponent();
        UpdateHighlight(CurrentState);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CurrentStateProperty)
        {
            UpdateHighlight((TransactionState)change.NewValue!);
        }
    }

    private void UpdateHighlight(TransactionState state)
    {
        ResetNode(this.FindControl<Border>("NodeIdle"));
        ResetNode(this.FindControl<Border>("NodeAwaitingPayment"));
        ResetNode(this.FindControl<Border>("NodeProcessingPayment"));
        ResetNode(this.FindControl<Border>("NodeCompleted"));
        ResetNode(this.FindControl<Border>("NodeCancelled"));

        switch (state)
        {
            case TransactionState.Idle:
                HighlightNode(this.FindControl<Border>("NodeIdle"), "Brush.DarkBrown", "Brush.Accent", "Brush.Light");
                break;
            case TransactionState.AwaitingPayment:
                HighlightNode(this.FindControl<Border>("NodeAwaitingPayment"), "Brush.Accent", "Brush.Light", "Brush.DarkBrown");
                break;
            case TransactionState.ProcessingPayment:
                HighlightNode(this.FindControl<Border>("NodeProcessingPayment"), "Brush.LightBrown", "Brush.Brown", "Brush.DarkBrown");
                break;
            case TransactionState.Completed:
                HighlightNode(this.FindControl<Border>("NodeCompleted"), "Brush.Good", "Brush.DarkBrown", "Brush.DarkBrown");
                break;
            case TransactionState.Cancelled:
                HighlightNode(this.FindControl<Border>("NodeCancelled"), "Brush.Danger", "Brush.DarkBrown", "Brush.Light");
                break;
        }
    }

    private void HighlightNode(Border? border, string backgroundKey, string borderKey, string textKey)
    {
        SetNodeColors(border, backgroundKey, borderKey, textKey, 1.0);
    }

    private void ResetNode(Border? border)
    {
        SetNodeColors(border, "Brush.Brown", "Brush.LightBrown", "Brush.Light", 0.55);
    }

    private void SetNodeColors(Border? border, string backgroundKey, string borderKey, string textKey, double opacity)
    {
        if (border is null) return;

        border.Background = GetBrush(backgroundKey);
        border.BorderBrush = GetBrush(borderKey);
        border.Opacity = opacity;

        if (border.Child is TextBlock textBlock)
        {
            textBlock.Foreground = GetBrush(textKey);
        }
    }

    private static IBrush GetBrush(string resourceKey)
    {
        return Application.Current?.Resources[resourceKey] as IBrush ?? Brushes.Transparent;
    }
}
