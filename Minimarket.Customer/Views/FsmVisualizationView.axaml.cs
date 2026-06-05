using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Minimarket.Core.States;
using System;

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
        // Reset all
        ResetNode(this.FindControl<Border>("NodeIdle"), "#1C1C3A", "#2C2C4E");
        ResetNode(this.FindControl<Border>("NodeScanning"), "#1C1C3A", "#2C2C4E");
        ResetNode(this.FindControl<Border>("NodeAwaitingPayment"), "#1C1C3A", "#2C2C4E");
        ResetNode(this.FindControl<Border>("NodeProcessingPayment"), "#1C1C3A", "#2C2C4E");
        ResetNode(this.FindControl<Border>("NodeCompleted"), "#1C1C3A", "#2C2C4E");
        ResetNode(this.FindControl<Border>("NodeCancelled"), "#1C1C3A", "#2C2C4E");

        // Set active
        switch (state)
        {
            case TransactionState.Idle:
                HighlightNode(this.FindControl<Border>("NodeIdle"), "#004D40", "#00BFA5");
                break;
            case TransactionState.Scanning:
                HighlightNode(this.FindControl<Border>("NodeScanning"), "#0D47A1", "#29B6F6");
                break;
            case TransactionState.AwaitingPayment:
                HighlightNode(this.FindControl<Border>("NodeAwaitingPayment"), "#FF6F00", "#FFB300");
                break;
            case TransactionState.ProcessingPayment:
                HighlightNode(this.FindControl<Border>("NodeProcessingPayment"), "#311B92", "#7E57C2");
                break;
            case TransactionState.Completed:
                HighlightNode(this.FindControl<Border>("NodeCompleted"), "#1B5E20", "#66BB6A");
                break;
            case TransactionState.Cancelled:
                HighlightNode(this.FindControl<Border>("NodeCancelled"), "#B71C1C", "#EF5350");
                break;
        }
    }

    private void HighlightNode(Border? border, string bgHex, string borderHex)
    {
        if (border == null) return;
        border.Background = Brush.Parse(bgHex);
        border.BorderBrush = Brush.Parse(borderHex);
        border.Opacity = 1.0;
    }

    private void ResetNode(Border? border, string bgHex, string borderHex)
    {
        if (border == null) return;
        border.Background = Brush.Parse(bgHex);
        border.BorderBrush = Brush.Parse(borderHex);
        border.Opacity = 0.4;
    }
}
