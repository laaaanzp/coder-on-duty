using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;

    public LanguageDatabase currentLanguage;

    void Awake()
    {
        if (!SecurePlayerPrefs.isInitialized())
        {
            SecurePlayerPrefs.Init();
        }

        if (instance == null)
        {
            instance = this;
        }
    }
}
