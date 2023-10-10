using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public string name;
    public int time;
    public int score;

    public UserData(string name, int time, int score)
    {
        this.name = name;
        this.time = time;
        this.score = score;
    }
}


public class LanguageDatabase : MonoBehaviour
{
    [SerializeField] public string languageName;
    [SerializeField] public SceneReference[] scenes;

    public string currentName
    {
        get
        {
            return SecurePlayerPrefs.GetString($"{languageName}-name", "");
        }
        set
        {
            SecurePlayerPrefs.SetString($"{languageName}-name", value);
            SecurePlayerPrefs.Save();
        }
    }

    public int currentLevel
    {
        get
        {
            currentLevel = 1;
            return SecurePlayerPrefs.GetInt($"{languageName}-level", 1);
        }
        set
        {
            SecurePlayerPrefs.SetInt($"{languageName}-level", value);
            SecurePlayerPrefs.Save();
        }
    }
    public int currentTime
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
    public int currentScore
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
    public float currentTotalAccuracy
    {
        get
        {
            return SecurePlayerPrefs.GetFloat($"{languageName}-accuracy", 100);
        }
        set
        {
            SecurePlayerPrefs.SetFloat($"{languageName}-accuracy", value);
            SecurePlayerPrefs.Save();
        }
    }

    public int currentTotalStars
    {
        get
        {
            return SecurePlayerPrefs.GetInt($"{languageName}-stars", 0);
        }
        set
        {
            SecurePlayerPrefs.SetInt($"{languageName}-stars", value);
            SecurePlayerPrefs.Save();
        }
    }

    public float overallAccuracy
    {
        get => currentTotalAccuracy / scenes.Length;
    }

    public float overallStars
    {
        get => currentTotalStars / scenes.Length;
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
    
    public static void Play(string languageName)
    {
        LanguageDatabase languageDatabase = GetInstance(languageName);

        if (languageDatabase.currentLevel == 1)
        {
            ProgrammersNameControl.Show(name =>
            {
                languageDatabase.currentName = name;
                _Play(languageName);
            });
        }
        else
        {
            _Play(languageName);
        }
    }

    private static void _Play(string languageName)
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
        currentTotalAccuracy = 0f;
        currentTotalStars = 0;
        currentName = "";
    }

    public void AddAttempt()
    {
        Dictionary<string, object> attemptInformation = new Dictionary<string, object>
        {
            { "time", currentTime },
            { "score", currentScore },
            { "name", currentName }
        };

        DatabaseManager.instance.dbReference
            .Child("attempts")
            .Child(languageName).Push()
            .SetValueAsync(attemptInformation);
    }

    
    public void FetchTopUsersByScore(Action<List<UserData>> onCallback, Action<string> onError)
    {
        DatabaseManager.instance.dbReference.Child("attempts").Child(languageName).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot usersDataSnapshot = task.Result;

                    List<UserData> userDatas = new List<UserData>();

                    foreach (DataSnapshot userDataSnapshot in usersDataSnapshot.Children)
                    {
                        DataSnapshot timeSnapshot = userDataSnapshot.Child("time");
                        DataSnapshot scoreSnapshot = userDataSnapshot.Child("score");

                        string name = userDataSnapshot.Child("name").Value.ToString();
                        int time = Convert.ToInt32(timeSnapshot.Value);
                        int score = Convert.ToInt32(scoreSnapshot.Value);

                        userDatas.Add(new UserData(name, time, score));
                    }

                    userDatas.Sort((a, b) => b.score.CompareTo(a.score));

                    onCallback?.Invoke(userDatas);
                }
                else if (task.IsFaulted)
                {
                    onError("Can't fetch data.");
                }
            });
    }

    public void FetchTopUsersByTime(Action<List<UserData>> onCallback, Action<string> onError)
    {
        DatabaseManager.instance.dbReference.Child("attempts").Child(languageName).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot usersDataSnapshot = task.Result;

                    List<UserData> userDatas = new List<UserData>();

                    foreach (DataSnapshot userDataSnapshot in usersDataSnapshot.Children)
                    {
                        DataSnapshot timeSnapshot = userDataSnapshot.Child("time");
                        DataSnapshot scoreSnapshot = userDataSnapshot.Child("score");

                        string name = userDataSnapshot.Child("name").Value.ToString();
                        int time = Convert.ToInt32(timeSnapshot.Value);
                        int score = Convert.ToInt32(scoreSnapshot.Value);

                        userDatas.Add(new UserData(name, time, score));
                    }

                    userDatas.Sort((a, b) => a.time.CompareTo(b.time));

                    onCallback?.Invoke(userDatas);
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
