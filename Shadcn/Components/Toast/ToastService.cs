namespace Shadcn.Components.Table;

public class ToastService {
    public event Action _OnChange = () => { };


    public string title = string.Empty;
    public string description = string.Empty;
    public ToastLevel level;

    
    private System.Timers.Timer? _toastTimer;
    private bool _toastVisibility = false;

    public void RegisterToast(Action onChange) {
        _OnChange = onChange;
    }

    public void UnregisterToast() {
        //_OnChange.Remove();
    }


    public void ShowToast(string message, string title, ToastLevel level) {
        _OnChange.Invoke();
        _toastVisibility = true;

        this.title = title;
        this.description = message;
        this.level = level;

        // Configura o Timer para esconder o toast após 3 segundos
        _toastTimer?.Stop();
        _toastTimer = new System.Timers.Timer(3000); // 3000 milissegundos = 3 segundos
        _toastTimer.Elapsed += (sender, e) => HideToast();
        _toastTimer.Start();
    }

    public void HideToast() {
        _toastVisibility = false;
        _OnChange?.Invoke();

        // Para o Timer quando o toast for escondido
        _toastTimer?.Stop();
    }

    // Verificar se o toast está visível
    public bool IsVisible() {
        return _toastVisibility;
    }
}

public enum ToastLevel {
    Info,
    Success,
    Warning,
    Error
}
