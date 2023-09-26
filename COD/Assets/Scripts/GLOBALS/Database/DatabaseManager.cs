using Firebase.Database;
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
        if (instance == null)
        {
            instance = this;
        }

        firebaseDatabase = FirebaseDatabase.GetInstance("https://cod-database-default-rtdb.asia-southeast1.firebasedatabase.app");
        dbReference = firebaseDatabase.RootReference;
    }

    void Start()
    {
        firebaseDatabase.SetPersistenceEnabled(true);
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
