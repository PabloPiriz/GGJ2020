using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static void Shuffle<T>(this IList<T> list)  
    {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = (int)(Random.value * n);
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }
}
