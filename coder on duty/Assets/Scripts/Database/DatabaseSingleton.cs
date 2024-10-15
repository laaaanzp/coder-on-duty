using UnityEngine;

public class DatabaseSingleton : MonoBehaviour
{
    private static bool isInstanced;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (isInstanced)
        {
            Destroy(gameObject);
        }

        isInstanced = true;
    }
}
