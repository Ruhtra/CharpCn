namespace Shadcn.Components.Accordion;

using Shadcn.Components.Accordion;
using System.Runtime.CompilerServices;

public class AccordionService {
    private readonly Dictionary<string, Action> _onChangeActions = new(); // Renomeado para maior clareza
    private readonly Dictionary<string, List<Accordionitem>> _listItems = new();
    private readonly Dictionary<string, AccordionType> _accordionTypes = new(); // Armazena o tipo (single ou multiple)

    // Enum para definir os tipos de acordeão

    public void RegisterAccordion(string accordionId, AccordionType type, Action onChange) {
        _onChangeActions[accordionId] = onChange;
        _listItems[accordionId] = new List<Accordionitem>();
        _accordionTypes[accordionId] = type; // Armazena o tipo de acordeão
    }

    public void UnregisterAccordion(string accordionId) {
        _onChangeActions.Remove(accordionId);
        _listItems.Remove(accordionId);
        _accordionTypes.Remove(accordionId);
    }

    public void RegisterItem(string accordionId, string accordionItemId) {
        // Se o item já existir, lança uma exceção
        if (_listItems[accordionId].Any(e => e.id == accordionItemId)) {
            throw new ArgumentException("Item already exists in the accordion.");
        }
        _listItems[accordionId].Add(new Accordionitem { id = accordionItemId });
    }

    public void UnRegisterItem(string accordionId, string accordionItemId) {
        // Verifica se o item existe na lista de itens do acordeão
        var itemToRemove = _listItems[accordionId].FirstOrDefault(e => e.id == accordionItemId);

        // Se o item não existir, lança uma exceção
        if (itemToRemove == null) {
            throw new ArgumentException("Item not found in the accordion.");
        }

        // Remove o item da lista
        _listItems[accordionId].Remove(itemToRemove);

        // Dispara o evento onChange, se houver
        _onChangeActions.TryGetValue(accordionId, out var onChange);
        onChange?.Invoke();
    }


    public void visibleItem(string accordionId, string accordionItemId) {
        // Encontra o índice do item com o ID fornecido
        var idItem = _listItems[accordionId].FindIndex(e => e.id == accordionItemId);

        // Verifica se o item foi encontrado, caso contrário lança uma exceção
        if (idItem < 0) throw new ArgumentOutOfRangeException(nameof(idItem), "Item not found in the list.");

        // Obtém o item clicado
        var selectedItem = _listItems[accordionId][idItem];

        // Verifica o tipo de acordeão (single ou multiple)
        if (_accordionTypes[accordionId] == AccordionType.Single) {
            // Tipo single: alterna visibilidade apenas de um item
            if (selectedItem.visible) {
                selectedItem.visible = false; // Se já estiver visível, torna invisível
            } else {
                _listItems[accordionId].ForEach(item => item.visible = item.id == accordionItemId);
            }
        } else if (_accordionTypes[accordionId] == AccordionType.Multiple) {
            // Tipo multiple: alterna visibilidade sem alterar os outros itens
            selectedItem.visible = !selectedItem.visible;
        }

        // Dispara o evento onChange, se houver
        _onChangeActions.TryGetValue(accordionId, out var onChange);
        onChange?.Invoke();
    }

    public bool IsVisible(string accordionId, string accordionItemId) {
        var accordionItem = _listItems[accordionId].Find(e => e.id == accordionItemId);
        if (accordionItem != null) return accordionItem.visible;
        else throw new ArgumentException("Item not found in the accordion.");
    }
}

public class Accordionitem {
    public string id { get; set; } = string.Empty;
    public bool visible = false;
}



public enum AccordionType {
    Single,
    Multiple
}