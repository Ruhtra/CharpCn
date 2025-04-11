namespace Shadcn.Components.Dialog;

public class DialogService {
    // Evento global para notificar mudanças de estado
    //public event Action? OnChange;

    // Armazenar as ações de exibição e ocultação para cada diálogo
    private readonly Dictionary<string, Action> _showActions = new();
    private readonly Dictionary<string, Action> _hideActions = new();
    private readonly Dictionary<string, Action> _onChangeActions = new(); // Renomeado para maior clareza

    // Armazenar a visibilidade de cada diálogo
    private readonly Dictionary<string, bool> _dialogVisibility = new();

    private static readonly Action EmptyAction = () => { };

    public void RegisterDialog(string dialogId, Action onChange, bool InitialStateVisible = false, Action? onShow = null, Action? onHide = null) {
        _showActions[dialogId] = onShow ?? EmptyAction;
        _hideActions[dialogId] = onHide ?? EmptyAction;
        _onChangeActions[dialogId] = onChange; // Associa a ação onChange ao dialog específico
        _dialogVisibility[dialogId] = InitialStateVisible;
    }

    public void UnregisterDialog(string dialogId) {
        _showActions.Remove(dialogId);
        _hideActions.Remove(dialogId);
        _onChangeActions.Remove(dialogId); // Remove a ação de mudança ao desregistrar o diálogo
        _dialogVisibility.Remove(dialogId);
    }

    // Exibir o diálogo
    public void Show(string dialogId) {
        if (_showActions.TryGetValue(dialogId, out var showAction)) {
            showAction.Invoke();
            _dialogVisibility[dialogId] = true;

            // Notificar mudanças para o diálogo específico
            if (_onChangeActions.TryGetValue(dialogId, out var onChange)) {
                onChange.Invoke();
            }

            //NotifyStateChanged(); // Notificar globalmente que houve uma mudança
        }
    }

    // Ocultar o diálogo
    public void Hide(string dialogId) {
        if (_hideActions.TryGetValue(dialogId, out var hideAction)) {
            hideAction.Invoke();
            _dialogVisibility[dialogId] = false;

            // Notificar mudanças para o diálogo específico
            if (_onChangeActions.TryGetValue(dialogId, out var onChange)) {
                onChange.Invoke();
            }

            //NotifyStateChanged(); // Notificar globalmente que houve uma mudança
        }
    }

    // Verificar se o diálogo está visível
    public bool IsVisible(string dialogId) {
        return _dialogVisibility.TryGetValue(dialogId, out var isVisible) && isVisible;
    }

    // Método para notificar os inscritos de que o estado global mudou
    //private void NotifyStateChanged() => OnChange?.Invoke();
}
