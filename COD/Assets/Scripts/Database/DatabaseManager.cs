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

    public IEnumerator InitializeNewUserData(string userId)
    {
        DatabaseReference userDB = dbReference.Child("users").Child(userId);

        DatabaseReference userProgression = userDB.Child("progression");

        DatabaseReference csharpProgression = userProgression.Child("csharp");
        yield return csharpProgression.Child("current-time").SetValueAsync(0);
        yield return csharpProgression.Child("current-level").SetValueAsync(1);
        yield return csharpProgression.Child("current-score").SetValueAsync(0);

        DatabaseReference javaProgression = userProgression.Child("java");
        yield return javaProgression.Child("current-time").SetValueAsync(0);
        yield return javaProgression.Child("current-level").SetValueAsync(1);
        yield return javaProgression.Child("current-score").SetValueAsync(0);
    }

    public IEnumerator UpdateDatabaseUsername(string username)
    {
        Task dbTask = dbReference
                        .Child("users")
                        .Child(AuthManager.user.UserId)
                        .Child("username")
                        .SetValueAsync(username);

        yield return new WaitUntil(() => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogError($"Failed to update database username: {dbTask.Exception}");
        }
        else
        {
            Debug.Log("Update database username successful.");
        }
    }
}
