using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static string FormatNumber(this int value)
    {
        return value switch
        {
            < 1000 => value.ToString(),
            < 1_000_000 => (value / 1000f).ToString("0.#") + "K",
            < 1_000_000_000 => (value / 1_000_000f).ToString("0.#") + "M",
            _ => (value / 1_000_000_000f).ToString("0.#") + "B"
        };
    }

    public static string ReplaceUnderScores(this string str, char c)
    {
        return !str.Contains('_') ? str : str.Replace('_', c);
    }
    
    public static T GetKeyByValue<T, V>(this Dictionary<T, V> dictionary, V value)
    {
        foreach (KeyValuePair<T, V> pair in dictionary)
        {
            if (pair.Value.Equals(value))
                return pair.Key;
        }
        return default;
    }

    public static void SetActive(this List<GameObject> gameObjects, bool status)
    {
        gameObjects.ToArray().SetActive(status);
    }
    
    public static void SetActive(this GameObject[] gameObjects, bool status)
    {
        foreach (GameObject childObject in gameObjects)
        {
            childObject.SetActive(status);
        }
    }
    
    public static void ClearAllChildren(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            Object.Destroy(child.gameObject);
        }
    }
    
    public static T GetRandomItem<T>(this List<T> list) => list.Count == 0 ? default : list[Random.Range(0, list.Count)];

    public static Transform GetRandom(this Transform[] array) => array[Random.Range(0, array.Length)];

    public static void SetParentAndReset(this Transform source, Transform point,bool useScale)
    {
        source.SetParent(point);
            
        source.localPosition = Vector3.zero;
        source.localRotation = Quaternion.identity;
        
        if (!useScale)
            return;
        
        source.localScale = Vector3.one;
    }
}
