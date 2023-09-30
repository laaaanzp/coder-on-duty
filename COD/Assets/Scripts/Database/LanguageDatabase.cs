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

public class UserData
{
    public int level;
    public int time;
    public int score;

    public UserData(int level, int time, int score)
    {
        this.level = level;
        this.time = time;
        this.score = score;
    }
}


public class LanguageDatabase : MonoBehaviour
{
    [SerializeField] public string languageName;
    [SerializeField] public SceneReference[] scenes;

    [HideInInspector] public int currentLevel
    {
        get
        {
            return SecurePlayerPrefs.GetInt($"{languageName}-level", 1);
        }
        set
        {
            SecurePlayerPrefs.SetInt($"{languageName}-level", value);
            SecurePlayerPrefs.Save();
        }
    }
    [HideInInspector] public int currentTime
    {
        get
        {
            return SecurePlayerPrefs.GetInt($"{languageName}-time", 0);
        }
        set
        {
            SecurePlayerPrefs.SetInt($"{languageName}-time", value);
            SecurePlayerPrefs.Save();
        }
    }
    [HideInInspector] public int currentScore
    {
        get
        {
            return SecurePlayerPrefs.GetInt($"{languageName}-score", 0);
        }
        set
        {
            SecurePlayerPrefs.SetInt($"{languageName}-score", value);
            SecurePlayerPrefs.Save();
        }
    }

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
        get => currentLevel == scenes.Length;
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

    public void LoadUserData()
    {
        FetchUserData(userData =>
        {
            currentLevel = userData.level;
            currentTime = userData.time;
            currentScore = userData.score;
        });
    }

    public void OnLogin()
    {
        ResetProgress();
    }

    public void OnSignout()
    {
    }

    public void FetchUserData(Action<UserData> onCallback)
    {
        DatabaseManager.instance.dbReference
            .Child("users")
            .Child(AuthManager.user.UserId)
            .Child("progression")
            .Child(languageName)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot userDataSnapshot = task.Result;

                    if (userDataSnapshot.Exists)
                    {
                        int currentLevel = Convert.ToInt32(userDataSnapshot.Child("current-level").Value);
                        int currentScore = Convert.ToInt32(userDataSnapshot.Child("current-score").Value);
                        int currentTime = Convert.ToInt32(userDataSnapshot.Child("current-time").Value);

                        UserData userData = new UserData(currentLevel, currentTime, currentScore);
                        onCallback?.Invoke(userData);
                    }
                    else
                    {
                        onCallback?.Invoke(new UserData(1, 0, 0));
                    }
                }
                else
                {
                    onCallback?.Invoke(new UserData(1, 0, 0));
                }
            });
    }

    public static void Play(string languageName)
    {
        LanguageDatabase languageDatabase = GetInstance(languageName);
        DatabaseManager.instance.currentLanguage = languageDatabase;
        SceneReference scene = languageDatabase.scenes[languageDatabase.currentLevel - 1];
        SceneSwitcher.LoadScene(scene);
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
        }
    }

    public void ResetProgress()
    {
        currentLevel = 1;
        currentTime = 0;
        currentScore = 0;
    }

    public void AddAttempt()
    {
        Dictionary<string, int> attemptInformation = new Dictionary<string, int>
        {
            { "time", currentTime },
            { "score", currentScore }
        };

        DatabaseManager.instance.dbReference
            .Child("users")
            .Child(AuthManager.user.UserId)
            .Child("progression")
            .Child(languageName)
            .Child("attempts").Push()
            .SetValueAsync(attemptInformation);
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
