using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseButtonWidget<T> : ButtonComponent where T : UIWidget
{
    [SerializeField] protected T _imageAndLabelWidget;

    public override void SetContext(UIContext context)
    {
        base.SetContext(context);
        _imageAndLabelWidget.SetContext(context);
    }

    public void SetLabelText(string labelText)
    {
        _imageAndLabelWidget.SetLabel(labelText);
    }
}
