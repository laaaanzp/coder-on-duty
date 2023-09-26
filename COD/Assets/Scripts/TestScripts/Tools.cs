using System.Linq;
using UnityEngine;

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
}
