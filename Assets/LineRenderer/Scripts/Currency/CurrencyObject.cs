using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CurrencyObject
{
    [field: SerializeField] private List<CurrencyEntity> _currencyEntities = new();
    public List<CurrencyEntity> CurrencyEntities => _currencyEntities;

    public CurrencyObject(params CurrencyEntity[] resourceEntity)
    {
        _currencyEntities.AddRange(resourceEntity);
    }

    public void AddValue(CurrencyEntity resourceObject)
    {
        _currencyEntities.Add(resourceObject);
    }
    
    public int GetResourceValue(CurrencyName resourceName) =>
        _currencyEntities.Find(entity => entity.ResourceName == resourceName).Value;
    
    public override string ToString()
    {
        string text = string.Empty;

        foreach (var currencyEntity in _currencyEntities)
        {
            text += $"{currencyEntity.Value} {currencyEntity.ResourceName.ToString().ReplaceUnderScores(' ')} \n";
        }
        return text.TrimEnd();
    }
}