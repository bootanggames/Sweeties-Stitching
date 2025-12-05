using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonComponent : MonoBehaviour
{
    [SerializeField] protected Button _buttonComponent;
    public UIContext Context { get; private set; }

    private GameEvent<UIContext> _clickWithContext = new();
    public bool HasContext => Context != null;
    public bool IsInteractable => _buttonComponent.interactable;

    public GameEvent<bool> InteractableStatusChanged = new();
    
    protected virtual void Start()
    {
        SubscribeEvent(OnClick);
    }

    private void OnDestroy()
    {
        _clickWithContext = null;
    }

    public void SetInteractable(bool status)
    {
        _buttonComponent.interactable = status;
        InteractableStatusChanged.Raise(status);
    }
    
    public virtual void SetContext(UIContext context)
    {
        Context = context;
    }
    
    public void SubscribeToContextEvent(Action<UIContext> contextEvent)
    {
        _clickWithContext.Register(contextEvent);
    }
    
    public void UnSubscribeFromContextEvent(Action<UIContext> contextEvent)
    {
        _clickWithContext.UnRegister(contextEvent);
    }
    
    public void SubscribeEvent(UnityAction btnAction)
    {
        _buttonComponent.onClick.AddListener(btnAction);
    }
    
    public virtual void OnClick()
    {
        if (Context == null)
            return;
        
        _clickWithContext.Raise(Context);
      //  GameEvents.MenuEvents.PlayButtonClickSFX.Raise();
    }
}
