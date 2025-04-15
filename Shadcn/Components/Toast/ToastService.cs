using System;
using System.Threading;

namespace Shadcn.Components.Toast;

public class ToastService {
    public event Action? OnChange;

    public string title = string.Empty;
    public string description = string.Empty;
    public ToastLevel level;

    private Timer? _toastTimer;
    private bool _toastVisibility = false;

    private SynchronizationContext? _syncContext;

    public void RegisterToast(Action onChange) {
        OnChange = onChange;
        _syncContext = SynchronizationContext.Current; // Armazena o contexto da UI
    }

    public void UnregisterToast() {
        OnChange = null;
        _syncContext = null;
    }

    public void ShowToast(string message, string title, ToastLevel level) {
        _toastVisibility = true;
        this.title = title;
        this.description = message;
        this.level = level;

        // Limpa qualquer timer anterior
        _toastTimer?.Dispose();

        // Cria novo timer
        _toastTimer = new Timer(_ => {
            if (_syncContext != null) {
                _syncContext.Post(_ => HideToast(), null); // Executa dentro do contexto correto
            }
        }, null, 3000, Timeout.Infinite);

        OnChange?.Invoke();
    }

    public void HideToast() {
        _toastVisibility = false;
        OnChange?.Invoke();

        _toastTimer?.Dispose();
        _toastTimer = null;
    }

    public bool IsVisible() => _toastVisibility;
}

public enum ToastLevel {
    Info,
    Success,
    Warning,
    Error
}
