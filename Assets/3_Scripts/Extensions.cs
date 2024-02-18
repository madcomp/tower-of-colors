using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static T GetAndRemoveAt<T>(this List<T> list, int index)
    {
        if (!IsValidIndex(list, index))
        {
            return default;
        }
        T element = list[index];
        list.RemoveAt(index);
        return element;
    }
    
    public static bool IsValidIndex<T>(this List<T> list, int index)
    {
        return (index >= 0 && index < list.Count);
    }
}
