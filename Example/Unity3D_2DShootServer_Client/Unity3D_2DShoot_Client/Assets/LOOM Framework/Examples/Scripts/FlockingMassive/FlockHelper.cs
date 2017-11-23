using System;
using System.Collections.Generic;
using UnityEngine;

public static class FlockHelper
{
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static Vector3 ReflectVector(Vector3 surfaceUpNormalized, Vector3 incommingDirection)
    {
        return 2f * -Vector3.Dot(incommingDirection, surfaceUpNormalized) * surfaceUpNormalized + incommingDirection;
    }

}
