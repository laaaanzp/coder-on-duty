using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tymski;
using UnityEngine;


[System.Serializable]
public class AttemptData
{
    public int time;
    public int score;

    public AttemptData(int time, int score)
    {
        this.time = time;
        this.score = score;
    }
}


public class LanguageDatabase : MonoBehaviour
{
    [SerializeField] public string languageName;
    [SerializeField] public SceneReference[] scenes;

    [HideInInspector] public int currentLevel;
    public AttemptData userCurrentData;
    public AttemptData userFirstData;
    public AttemptData userLatestData;

    // Average Datas
    public AttemptData averageFirstData;
    public AttemptData averageLatestData;

    // Instances
    public static Dictionary<string, LanguageDatabase> instances = new Dictionary<string, LanguageDatabase>();


    void Awake()
    {
        instances.TryAdd(languageName, this);
    }

    public static LanguageDatabase GetInstance(string languageName)
    {
        if (instances.TryGetValue(languageName, out LanguageDatabase instance))
        {
            return instance;
        }
        else
        {
            return null;
        }
    }

    public bool isPlayingTheLastLevel
    {
        get 
        {
            Debug.Log(currentLevel);
            Debug.Log(scenes.Length);
            return currentLevel == scenes.Length;
        }
    }

    public float progressPercentage
    {
        get
        {
            try
            {
                float result = ((float)currentLevel - 1) / scenes.Length;

                if (float.IsNaN(result) || result == float.NegativeInfinity ||  result == float.PositiveInfinity)
                {
                    result = 0f;
                }

                return result;
            }
            catch
            { return 0f; }
        }
    }

    public void ReloadData()
    {
        // Basically, saving the data to cache for offline use.
        FetchCurrentLevel(value => currentLevel = value);
        FetchCurrentScore(value => userCurrentData.score = value);
        FetchCurrentTime(value => userCurrentData.time = value);

        FetchFirstAverageAttempt(result =>
        {
            averageFirstData = result;
        }, null);

        FetchLatestAverageAttempt(result =>
        {
            averageLatestData = result;
        }, null);

        FetchUserFirstAttempt(result =>
        {
            userFirstData = result;
        }, null);

        FetchUserLatestAttempt(result =>
        {
            userLatestData = result;
        }, null);
    }

    public void OnLogin()
    {
        ReloadData();
    }

    public void OnSignout()
    {
    }

    public void FetchCurrentLevel(Action<int> onCallback)
    {
        DatabaseManager.instance.dbReference
            .Child("users")
            .Child(AuthManager.user.UserId)
            .Child("progression")
            .Child(languageName)
            .Child("current-level")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    int currentLevel = Convert.ToInt32(task.Result.Value);
                    onCallback(currentLevel);
                }
                else
                {
                    onCallback(1);
                }
            });
    }

    public void FetchCurrentTime(Action<int> onCallback)
    {
        DatabaseManager.instance.dbReference
            .Child("users")
            .Child(AuthManager.user.UserId)
            .Child("progression")
            .Child(languageName)
            .Child("current-time")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    int currentTime = Convert.ToInt32(task.Result.Value);
                    onCallback(currentTime);
                }
                else
                {
                    onCallback(0);
                }
            });
    }

    public void FetchCurrentScore(Action<int> onCallback)
    {
        DatabaseManager.instance.dbReference
            .Child("users")
            .Child(AuthManager.user.UserId)
            .Child("progression")
            .Child(languageName)
            .Child("current-score")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    int currentScore = Convert.ToInt32(task.Result.Value);
                    onCallback(currentScore);
                }
                else
                {
                    onCallback(0);
                }
            });
    }

    public static void Play(string languageName)
    {
        LanguageDatabase languageDatabase = GetInstance(languageName);
        DatabaseManager.instance.currentLanguage = languageDatabase;
        SceneReference currentScene = languageDatabase.scenes[languageDatabase.currentLevel - 1];
        SceneSwitcher.LoadScene(currentScene);
    }

    public void AddCurrentTime(int time)
    {
        userCurrentData.time += time;
        StartCoroutine(_AddCurrentTime());
    }

    private IEnumerator _AddCurrentTime()
    {
        DatabaseReference db =
            DatabaseManager.instance.dbReference
                .Child("users")
                .Child(AuthManager.user.UserId)
                .Child("progression")
                .Child(languageName)
                .Child("current-time");

        yield return db.SetValueAsync(userCurrentData.time);
    }

    public void AddCurrentScore(int score)
    {
        userCurrentData.score += score;
        StartCoroutine(_AddCurrentScore());
    }

    private IEnumerator _AddCurrentScore()
    {
        DatabaseReference db =
            DatabaseManager.instance.dbReference
                .Child("users")
                .Child(AuthManager.user.UserId)
                .Child("progression")
                .Child(languageName)
                .Child("current-score");

        yield return db.SetValueAsync(userCurrentData.score);
    }

    public void LevelUp()
    {
        if (isPlayingTheLastLevel)
        {
            AddAttempt();
            ResetProgress();
        }
        else
        {
            currentLevel++;
            StartCoroutine(_LevelUp());
        }
    }

    private IEnumerator _LevelUp()
    {
        DatabaseReference db =
            DatabaseManager.instance.dbReference
                .Child("users")
                .Child(AuthManager.user.UserId)
                .Child("progression")
                .Child(languageName)
                .Child("current-level");

        yield return db.SetValueAsync(currentLevel);
    }

    public void ResetProgress()
    {
        StartCoroutine(_ResetProgress());
    }

    public IEnumerator _ResetProgress()
    {
        currentLevel = 1;
        userCurrentData.time = 0;
        userCurrentData.score = 0;

        DatabaseReference db = DatabaseManager.instance.dbReference
                                .Child("users")
                                .Child(AuthManager.user.UserId)
                                .Child("progression")
                                .Child(languageName);

        yield return db.Child("current-level").SetValueAsync(1);
        yield return db.Child("current-time").SetValueAsync(0);
        yield return db.Child("current-score").SetValueAsync(0);
    }

    public void AddAttempt()
    {
        StartCoroutine(_AddAttempt());
    }

    private IEnumerator _AddAttempt()
    {
        Dictionary<string, int> attemptInformation = new Dictionary<string, int>
        {
            { "time", userCurrentData.time },
            { "score", userCurrentData.score }
        };

        yield return DatabaseManager.instance.dbReference
                                        .Child("users")
                                        .Child(AuthManager.user.UserId)
                                        .Child("progression")
                                        .Child(languageName)
                                        .Child("attempts").Push()
                                        .SetValueAsync(attemptInformation);
    }

    public void UpdateCurrentLevel(int level)
    {
        StartCoroutine(_UpdateCurrentLevel(level));
    }

    private IEnumerator _UpdateCurrentLevel(int level)
    {
        Task dbTask =
            DatabaseManager.instance.dbReference
                .Child("users")
                .Child(AuthManager.user.UserId)
                .Child("progression")
                .Child(languageName)
                .Child("current-level")
                .SetValueAsync(level);

        yield return new WaitUntil(() => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogError($"Failed to update database progression level: {dbTask.Exception}");
        }
        else
        {
            Debug.Log("Update database progression level successful.");
        }
    }

    public void FetchUserFirstAttempt(Action<AttemptData> onCallback, Action<string> onError)
    {
        DatabaseManager.instance.dbReference.Child("users").Child(AuthManager.user.UserId).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot userDataSnapshot = task.Result;
                    DataSnapshot attemptDataSnapshot = userDataSnapshot.Child("progression").Child(languageName).Child("attempts").Children.First();
                    DataSnapshot scoreSnapshot = attemptDataSnapshot.Child("score");
                    DataSnapshot timeSnapshot = attemptDataSnapshot.Child("time");

                    int score = 0, time = 0;

                    if (scoreSnapshot.Exists)
                        score = Convert.ToInt32(scoreSnapshot.Value);

                    if (timeSnapshot.Exists)
                        time = Convert.ToInt32(timeSnapshot.Value);

                    onCallback?.Invoke(new AttemptData(time, score));
                }
                else if (task.IsFaulted)
                {
                    onError("Can't fetch data.");
                }
            });
    }

    public void FetchUserLatestAttempt(Action<AttemptData> onCallback, Action<string> onError)
    {
        DatabaseManager.instance.dbReference.Child("users").Child(AuthManager.user.UserId).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot userDataSnapshot = task.Result;
                    DataSnapshot attemptDataSnapshot = userDataSnapshot.Child("progression").Child(languageName).Child("attempts").Children.Last();
                    DataSnapshot scoreSnapshot = attemptDataSnapshot.Child("score");
                    DataSnapshot timeSnapshot = attemptDataSnapshot.Child("time");

                    int score = 0, time = 0;

                    if (scoreSnapshot.Exists)
                        score = Convert.ToInt32(scoreSnapshot.Value);

                    if (timeSnapshot.Exists)
                        time = Convert.ToInt32(timeSnapshot.Value);

                    onCallback?.Invoke(new AttemptData(time, score));
                }
                else if (task.IsFaulted)
                {
                    onError("Can't fetch data.");
                }
            });
    }

    public void FetchTopUsersByScore(Action<Dictionary<string, int>> onCallback, Action<string> onError)
    {
        DatabaseManager.instance.dbReference.Child("users").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot usersDataSnapshot = task.Result;

                    Dictionary<string, int> datas = new Dictionary<string, int>();

                    foreach (DataSnapshot userDataSnapshot in usersDataSnapshot.Children)
                    {
                        string username = userDataSnapshot.Child("username").Value.ToString();
                        DataSnapshot attemptDataSnapshot = userDataSnapshot.Child("progression").Child(languageName).Child("attempts").Children.Last();
                        DataSnapshot scoreSnapshot = attemptDataSnapshot.Child("score");

                        int score = 0;

                        if (scoreSnapshot.Exists)
                            score = Convert.ToInt32(attemptDataSnapshot.Child("score").Value);

                        datas.Add(username, score);
                    }

                    // Sort data
                    List<KeyValuePair<string, int>> sortedList = datas.ToList();
                    sortedList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                    datas = sortedList.ToDictionary(pair => pair.Key, pair => pair.Value);

                    onCallback?.Invoke(datas);
                }
                else if (task.IsFaulted)
                {
                    onError("Can't fetch data.");
                }
            });
    }

    public void FetchTopUsersByTime(Action<Dictionary<string, int>> onCallback, Action<string> onError)
    {
        DatabaseManager.instance.dbReference.Child("users").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot usersDataSnapshot = task.Result;

                    Dictionary<string, int> datas = new Dictionary<string, int>();

                    foreach (DataSnapshot userDataSnapshot in usersDataSnapshot.Children)
                    {
                        string username = userDataSnapshot.Child("username").Value.ToString();
                        DataSnapshot attemptDataSnapshot = userDataSnapshot.Child("progression").Child(languageName).Child("attempts").Children.Last();
                        DataSnapshot timeSnapshot = attemptDataSnapshot.Child("score");

                        int time = 0;

                        if (timeSnapshot.Exists)
                            time = Convert.ToInt32(attemptDataSnapshot.Child("time").Value);

                        datas.Add(username, time);
                    }

                    // Sort data
                    List<KeyValuePair<string, int>> sortedList = datas.ToList();
                    sortedList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
                    datas = sortedList.ToDictionary(pair => pair.Key, pair => pair.Value);

                    onCallback?.Invoke(datas);
                }
                else if (task.IsFaulted)
                {
                    onError("Can't fetch data.");
                }
            });
    }

    public void FetchFirstAverageAttempt(Action<AttemptData> onCallback, Action<string> onError)
    {
        DatabaseManager.instance.dbReference.Child("users").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot usersDataSnapshot = task.Result;

                    int totalTime = 0, totalScore = 0, totalData = 0;

                    foreach (DataSnapshot userDataSnapshot in usersDataSnapshot.Children)
                    {
                        string username = userDataSnapshot.Child("username").Value.ToString();
                        DataSnapshot attemptsDataSnapshot = userDataSnapshot.Child("progression").Child(languageName).Child("attempts");

                        if (!attemptsDataSnapshot.Exists)
                            continue;

                        DataSnapshot attemptDataSnapshot = attemptsDataSnapshot.Children.First();

                        totalTime += Convert.ToInt32(attemptDataSnapshot.Child("time").Value);
                        totalScore += Convert.ToInt32(attemptDataSnapshot.Child("score").Value);
                        totalData++;
                    }

                    if (totalData == 0)
                    {
                        onCallback?.Invoke(new AttemptData(0, 0));
                    }
                    else
                    {
                        int averageTime = totalTime / totalData;
                        int averageScore = totalScore / totalData;

                        onCallback?.Invoke(new AttemptData(averageTime, averageScore));
                    }
                }
                else if (task.IsFaulted)
                {
                    onError("Can't fetch data.");
                }
            });
    }

    public void FetchLatestAverageAttempt(Action<AttemptData> onCallback, Action<string> onError)
    {
        DatabaseManager.instance.dbReference.Child("users").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot usersDataSnapshot = task.Result;

                    int totalTime = 0, totalScore = 0, totalData = 0;

                    foreach (DataSnapshot userDataSnapshot in usersDataSnapshot.Children)
                    {
                        string username = userDataSnapshot.Child("username").Value.ToString();
                        DataSnapshot attemptsDataSnapshot = userDataSnapshot.Child("progression").Child(languageName).Child("attempts");

                        if (!attemptsDataSnapshot.Exists)
                            continue;

                        DataSnapshot attemptDataSnapshot = attemptsDataSnapshot.Children.Last();

                        totalTime += Convert.ToInt32(attemptDataSnapshot.Child("time").Value);
                        totalScore += Convert.ToInt32(attemptDataSnapshot.Child("score").Value);
                        totalData++;
                    }

                    if (totalData == 0)
                    {
                        onCallback?.Invoke(new AttemptData(0, 0));
                    }
                    else
                    {
                        int averageTime = totalTime / totalData;
                        int averageScore = totalScore / totalData;

                        onCallback?.Invoke(new AttemptData(averageTime, averageScore));
                    }
                }
                else if (task.IsFaulted)
                {
                    onError("Can't fetch data.");
                }
            });
    }
}
