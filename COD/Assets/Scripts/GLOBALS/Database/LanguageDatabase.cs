using Firebase.Database;
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
    public static LanguageDatabase csharpInstance;
    public static LanguageDatabase javaInstance;


    void Awake()
    {
        if (languageName == "java")
            javaInstance = this;

        else
            csharpInstance = this;
    }

    public bool isPlayingTheLastLevel
    {
        get => currentLevel == scenes.Length;
    }

    public void ReloadData()
    {
        StartCoroutine(FetchUserFirstData());
        StartCoroutine(FetchUserLatestData());

        StartCoroutine(FetchAverageFirstData());
        StartCoroutine(FetchAverageLatestData());
    }

    public void OnLogin()
    {
        ReloadData();
        StartCoroutine(_OnLogin());
    }

    public void OnSignout()
    {
    }

    private IEnumerator _OnLogin()
    {
        DatabaseReference db = DatabaseManager.instance.dbReference
                                .Child("users")
                                .Child(AuthManager.user.UserId)
                                .Child("progression")
                                .Child(languageName);

        Task<DataSnapshot> task = db.GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.Log(task.Exception);
        }
        else
        {
            DataSnapshot dataSnapshot = task.Result;
            currentLevel = Convert.ToInt32(dataSnapshot.Child("current-level").Value);
            Debug.Log($"Current Level: {currentLevel}");
            userCurrentData.time = Convert.ToInt32(dataSnapshot.Child("current-time").Value);
            userCurrentData.score = Convert.ToInt32(dataSnapshot.Child("current-score").Value);
        }
    }

    public static void Play(string languageName)
    {
        LanguageDatabase languageDatabase = languageName == "java" ? javaInstance : csharpInstance;
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

    public IEnumerator FetchAverageFirstData()
    {
        Task<DataSnapshot> task = DatabaseManager.instance.dbReference.Child("users").GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        DataSnapshot userDatas = task.Result;

        int totalTime = 0;
        int totalScore = 0;
        int totalAttempts = 0;

        foreach (DataSnapshot userData in userDatas.Children)
        {
            DataSnapshot progressData = userData.Child("progression");
            DataSnapshot attemptsData = progressData.Child("attempts");

            if (!attemptsData.HasChildren)
                continue;

            DataSnapshot firstAttempt = attemptsData.Children.First();

            int time = Convert.ToInt32(firstAttempt.Child("time").Value);
            int score = Convert.ToInt32(firstAttempt.Child("score").Value);

            totalTime += time;
            totalScore += score;
            totalAttempts++;
        }

        if (totalAttempts == 0)
        {
            averageFirstData = new AttemptData() { time = 0, score = 0 };
        }
        else
        {
            int averageTime = totalTime / totalAttempts;
            int averageScore = totalScore / totalAttempts;

            averageFirstData = new AttemptData() { time = averageTime, score = averageScore };
        }
    }

    public IEnumerator FetchAverageLatestData()
    {
        Task<DataSnapshot> task = DatabaseManager.instance.dbReference.Child("users").GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        DataSnapshot userDatas = task.Result;

        int totalTime = 0;
        int totalScore = 0;
        int totalAttempts = 0;

        foreach (DataSnapshot userData in userDatas.Children)
        {
            DataSnapshot progressData = userData.Child("progression");
            DataSnapshot attemptsData = progressData.Child("attempts");

            if (!attemptsData.HasChildren)
                continue;

            DataSnapshot latestAttempt = attemptsData.Children.Last();

            int time = Convert.ToInt32(latestAttempt.Child("time").Value);
            int score = Convert.ToInt32(latestAttempt.Child("score").Value);

            totalTime += time;
            totalScore += score;
            totalAttempts++;
        }

        if (totalAttempts == 0)
        {
            averageLatestData = new AttemptData() { time = 0, score = 0 };
        }
        else
        {
            int averageTime = totalTime / totalAttempts;
            int averageScore = totalScore / totalAttempts;

            averageLatestData = new AttemptData() { time = averageTime, score = averageScore };
        }
    }

    public IEnumerator FetchUserFirstData()
    {
        Task<DataSnapshot> task = DatabaseManager.instance.dbReference.Child("users").Child(AuthManager.user.UserId).GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        DataSnapshot userData = task.Result;

        DataSnapshot progressData = userData.Child("progression");
        DataSnapshot attemptsData = progressData.Child("attempts");

        if (attemptsData.HasChildren)
        {
            DataSnapshot firstAttempt = attemptsData.Children.First();

            int time = Convert.ToInt32(firstAttempt.Child("time").Value);
            int score = Convert.ToInt32(firstAttempt.Child("score").Value);

            userFirstData = new AttemptData() { time = time, score = score };
        }
        else
        {
            userFirstData = null;
        }
    }

    public IEnumerator FetchUserLatestData()
    {
        Task<DataSnapshot> task = DatabaseManager.instance.dbReference.Child("users").Child(AuthManager.user.UserId).GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        DataSnapshot userData = task.Result;

        DataSnapshot progressData = userData.Child("progression");
        DataSnapshot attemptsData = progressData.Child("attempts");

        if (attemptsData.HasChildren)
        {
            DataSnapshot latestAttempt = attemptsData.Children.Last();

            int time = Convert.ToInt32(latestAttempt.Child("time").Value);
            int score = Convert.ToInt32(latestAttempt.Child("score").Value);

            userLatestData = new AttemptData() { time = time, score = score };
        }
        else
        {
            userLatestData = null;
        }
    }
}
