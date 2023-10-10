using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public FirebaseDatabase firebaseDatabase;
    public DatabaseReference dbReference;
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

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                firebaseDatabase = FirebaseDatabase.GetInstance("https://cod-database-default-rtdb.asia-southeast1.firebasedatabase.app");
                dbReference = firebaseDatabase.RootReference;
                firebaseDatabase.SetPersistenceEnabled(true);
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");
            }
        });
    }
}
