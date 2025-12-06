using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ScreenWithSelectableButtons<T> : MonoBehaviour where T : ButtonWithWidget
{
    [SerializeField] protected T _buttonPrefab;
    [SerializeField] private Transform _containerRect;

    private List<T> _buttons = new();

    public void Refresh()
    {
        SpawnButtons();
    }
    
         
    
    
    protected virtual void SpawnButtons()
    {
        ClearAll();
        OnButtonsSpawned();
    }
    
    protected T SpawnButton(UIContext uiContext,Transform overrideHeader = null)
    {
        T button = Instantiate(_buttonPrefab, overrideHeader == null ? _containerRect : overrideHeader);
        
        button.SetContext(uiContext);
        button.SubscribeToContextEvent(OnItemButtonClicked);
        _buttons.Add(button);
        return button;
    }
    
    protected virtual void ClearAll()
    {
        _buttons.Clear();
        _containerRect.ClearAllChildren();
    }  
    
    protected virtual void OnButtonsSpawned()
    {
        
    }
    
    protected abstract void OnItemButtonClicked(UIContext context);
}
