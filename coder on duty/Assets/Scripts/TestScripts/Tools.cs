using System.Linq;
using UnityEngine;
using UnityEngine.Windows;

public static class Tools
{
    public static string RandomString(int length)
    {
        const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        return new string(
            Enumerable
                .Repeat(characters, length)
                .Select(c => c[Random.Range(0, c.Length)])
                .ToArray()
            );
    }

    public static T[] ShuffleArray<T>(T[] array)
    {
        int n = array.Length;

        for (int i = 0; i < n; i++)
        {
            int r = i + Random.Range(0, n - i); // Random index from i to n-1
            T temp = array[i];
            array[i] = array[r];
            array[r] = temp;
        }

        return array;
    }

    public static string ReverseString(string value)
    {
        char[] charArray = value.ToCharArray();
        System.Array.Reverse(charArray);
        return new string(charArray);
    }
}
