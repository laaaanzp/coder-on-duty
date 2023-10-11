using System.Collections.Generic;
using System.Data;
using Tymski;
using Mono.Data.Sqlite;
using UnityEngine;


public class UserData
{
    public string name;
    public int score;
    public int time;
    public float accuracy;
    public float stars;
    public string rating;
}


public class LanguageDatabase : MonoBehaviour
{
    [SerializeField] public string languageName;
    [SerializeField] public SceneReference[] scenes;

    private string connectionString;

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
    public string overallRating
    {
        get
        {
            if (overallStars >= 3)
            {
                return "Expert";
            }
            else if (overallStars >= 2)
            {
                return "Advanced";
            }
            else
            {
                return "Beginner";
            }
        }
    }
    // Instances
    public static Dictionary<string, LanguageDatabase> instances = new Dictionary<string, LanguageDatabase>();


    void Awake()
    {
        instances.TryAdd(languageName, this);
        connectionString = $"URI=file:{Application.dataPath}/{languageName}.db";
    }

    void Start()
    {
        CreateDB();
    }

    private void CreateDB()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS Attempts (name VARCHAR(16), score INT, time INT, accuracy FLOAT, stars FLOAT, rating VARCHAR(20));";
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TABLE IF NOT EXISTS Levels (name VARCHAR(50), score INT, time INT, accuracy FLOAT, stars FLOAT);";
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    private void ResetLevels()
    {
        foreach (SceneReference scene in scenes)
        {
            SetLevelDataByName(scene.SceneName, 0, 0, 0, 0);
        }
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

        ResetLevels();
    }

    public void AddAttempt()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                Debug.Log("Adding attempt...");
                command.CommandText = $"INSERT INTO Attempts VALUES (@name, @score, @time, @accuracy, @stars, @rating);";

                command.Parameters.AddWithValue("@name", currentName);
                command.Parameters.AddWithValue("@score", currentScore);
                command.Parameters.AddWithValue("@time", currentTime);
                command.Parameters.AddWithValue("@accuracy", overallAccuracy);
                command.Parameters.AddWithValue("@stars", overallStars);
                command.Parameters.AddWithValue("@rating", overallRating);

                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public List<UserData> GetTopUserDataByScore()
    {
        List<UserData> userDatas = new List<UserData>();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM Attempts";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UserData userData = new UserData();

                        userData.name = reader.GetString("name");
                        userData.score = reader.GetInt32("score");
                        userData.time = reader.GetInt32("time");
                        userData.accuracy = reader.GetFloat("accuracy");
                        userData.stars = reader.GetFloat("stars");
                        userData.rating = reader.GetString("rating");

                        userDatas.Add(userData);
                    }

                    userDatas.Sort((a, b) => b.score.CompareTo(a.score));
                }

                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        return userDatas;
    }

    public List<UserData> GetTopUserDataByTime()
    {
        List<UserData> userDatas = new List<UserData>();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM Attempts";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UserData userData = new UserData();

                        userData.name = reader.GetString("name");
                        userData.score = reader.GetInt32("score");
                        userData.time = reader.GetInt32("time");
                        userData.accuracy = reader.GetFloat("accuracy");
                        userData.stars = reader.GetFloat("stars");
                        userData.rating = reader.GetString("rating");

                        userDatas.Add(userData);
                    }

                    userDatas.Sort((a, b) => a.time.CompareTo(b.time));
                }

                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        return userDatas;
    }

    public List<UserData> GetAttemptsHistory()
    {
        List<UserData> userDatas = new List<UserData>();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM Attempts";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UserData userData = new UserData();

                        userData.name = reader.GetString("name");
                        userData.score = reader.GetInt32("score");
                        userData.time = reader.GetInt32("time");
                        userData.accuracy = reader.GetFloat("accuracy");
                        userData.stars = reader.GetFloat("stars");
                        userData.rating = reader.GetString("rating");

                        userDatas.Add(userData);
                    }
                }

                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        return userDatas;
    }

    public UserData GetLevelDataByName(string levelName)
    {
        UserData userData = new UserData();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM Levels WHERE name = \"{levelName}\";";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        userData.name = reader.GetString("name");
                        userData.score = reader.GetInt32("score");
                        userData.time = reader.GetInt32("time");
                        userData.accuracy = reader.GetFloat("accuracy");
                        userData.stars = reader.GetFloat("stars");
                    }
                    else
                    {
                        AddLevel(levelName);
                        userData.name = "";
                        userData.score = 0;
                        userData.time = 0;
                        userData.accuracy = 0;
                        userData.stars = 0;
                    }

                }

                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        return userData;
    }

    public void AddLevel(string levelName)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"INSERT INTO Levels VALUES (@name, @score, @time, @accuracy, @stars);";

                command.Parameters.AddWithValue("@name", levelName);
                command.Parameters.AddWithValue("@score", 0);
                command.Parameters.AddWithValue("@time", 0);
                command.Parameters.AddWithValue("@accuracy", 0);
                command.Parameters.AddWithValue("@stars", 0);

                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public void SetLevelDataByName(string levelName, int score, int time, float accuracy, float stars)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"UPDATE Levels SET score = @score, time = @time, accuracy = @accuracy, stars = @stars WHERE name = @levelName;";

                command.Parameters.AddWithValue("@levelName", levelName);
                command.Parameters.AddWithValue("@score", score);
                command.Parameters.AddWithValue("@time", time);
                command.Parameters.AddWithValue("@accuracy", accuracy);
                command.Parameters.AddWithValue("@stars", stars);

                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }
}
