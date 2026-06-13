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
        // Reset all 5 nodes
        ResetNode(this.FindControl<Border>("NodeIdle"),              "#2C1A0E", "#412D15");
        ResetNode(this.FindControl<Border>("NodeAwaitingPayment"),   "#2C1A0E", "#412D15");
        ResetNode(this.FindControl<Border>("NodeProcessingPayment"), "#2C1A0E", "#412D15");
        ResetNode(this.FindControl<Border>("NodeCompleted"),         "#2C1A0E", "#412D15");
        ResetNode(this.FindControl<Border>("NodeCancelled"),         "#2C1A0E", "#412D15");

        // Highlight active node with palette-harmonious colours
        switch (state)
        {
            case TransactionState.Idle:
                // Brown-tinted (neutral/resting)
                HighlightNode(this.FindControl<Border>("NodeIdle"), "#1F150C", "#FFB74D");
                break;
            case TransactionState.AwaitingPayment:
                // Amber/Accent — cart ready, awaiting payment choice
                HighlightNode(this.FindControl<Border>("NodeAwaitingPayment"), "#5C3D00", "#FFB74D");
                break;
            case TransactionState.ProcessingPayment:
                // Brown mid-tone — processing in progress
                HighlightNode(this.FindControl<Border>("NodeProcessingPayment"), "#412D15", "#F1CDA5");
                break;
            case TransactionState.Completed:
                // Green (Good) — success
                HighlightNode(this.FindControl<Border>("NodeCompleted"), "#1B5E20", "#81C784");
                break;
            case TransactionState.Cancelled:
                // Red (Danger) — failed/cancelled
                HighlightNode(this.FindControl<Border>("NodeCancelled"), "#7C0000", "#FF3131");
                break;
        }
    }

    private void HighlightNode(Border? border, string bgHex, string borderHex)
    {
        if (border == null) return;
        border.Background  = Brush.Parse(bgHex);
        border.BorderBrush = Brush.Parse(borderHex);
        border.Opacity     = 1.0;
    }

    private void ResetNode(Border? border, string bgHex, string borderHex)
    {
        if (border == null) return;
        border.Background  = Brush.Parse(bgHex);
        border.BorderBrush = Brush.Parse(borderHex);
        border.Opacity     = 0.4;
    }
}
