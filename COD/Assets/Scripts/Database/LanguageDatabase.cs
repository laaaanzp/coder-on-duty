using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using System;

public class AttemptData
{
    public string programmerName;
    public int score;
    public int stars;
    public int totalCorrectAnswers;
    public int totalAnswers;
    public int totalLevels;
    public DateTime dateTime;

    public float accuracy
    {
        get => Mathf.Max(((float)totalCorrectAnswers / (float)totalAnswers) * 100, 100);
    }
    
    public float averageStars
    {
        get => Mathf.Max(Mathf.RoundToInt(stars / totalLevels), 3);
    }
}

public class LevelData
{
    public string name;
    public int score;
    public int stars;
    public int totalCorrectAnswers;
    public int totalAnswers;

    public float accuracy
    {
        get => Mathf.Max(((float)totalCorrectAnswers / (float)totalAnswers) * 100, 100);
    }
}


public class LanguageDatabase : MonoBehaviour
{
    [SerializeField] public string languageName;
    [SerializeField] public string[] levelNames;

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
    public string currentLevelName
    {
        get => levelNames[currentLevel - 1];
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
                command.CommandText = "CREATE TABLE IF NOT EXISTS Attempts (name VARCHAR(16), score INT, stars INT, totalCorrectAnswers INT, totalAnswers INT, totalLevels INT, date DATETIME);";
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TABLE IF NOT EXISTS Levels (name VARCHAR(50), score INT, stars INT, totalCorrectAnswers INT, totalAnswers INT);";
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    private void ResetLevels()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Levels";
                command.ExecuteNonQuery();

                foreach (string levelName in levelNames)
                {
                    command.CommandText = "INSERT INTO Levels VALUES (@name, 0, 0, 0, 0);";
                    command.Parameters.AddWithValue("@name", levelName);
                    command.ExecuteNonQuery();
                }
            }

            connection.Close();
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
        get => currentLevel == levelNames.Length;
    }

    public float progressPercentage
    {
        get
        {
            try
            {
                float result = ((float)currentLevel - 1) / levelNames.Length;

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

        if (languageDatabase.currentLevel == 1)
        {
            // Storyboard
            // SceneSwitcher.LoadScene(2);

            // Temporary
            SceneSwitcher.LoadScene(1);
        }
        else
        {
            // Level
            SceneSwitcher.LoadScene(1);
        }
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
        currentName = "";

        ResetLevels();
    }

    public List<LevelData> GetAllLevelData()
    {
        List<LevelData> levelDatas = new List<LevelData>();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Levels";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LevelData levelData = new LevelData();

                        levelData.name = reader.GetString("name");
                        levelData.score = reader.GetInt32("score");
                        levelData.stars = reader.GetInt32("stars");
                        levelData.totalCorrectAnswers = reader.GetInt32("totalCorrectAnswers");
                        levelData.totalAnswers = reader.GetInt32("totalAnswers");

                        levelDatas.Add(levelData);
                    }
                }
            }

        }

        return levelDatas;
    }

    public void AddAttempt()
    {
        List<LevelData> levelDatas = GetAllLevelData();

        int totalLevels = levelDatas.Count;

        int totalScore = 0;
        int totalStars = 0;
        int totalCorrectAnswers = 0;
        int totalAnswers = 0;
        
        foreach (LevelData levelData in levelDatas)
        {
            totalScore += levelData.score;
            totalStars += levelData.stars;
            totalCorrectAnswers += levelData.totalCorrectAnswers;
            totalAnswers += levelData.totalAnswers;
        }

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"INSERT INTO Attempts VALUES (@name, @score, @stars, @totalCorrectAnswers, @totalAnswers, @totalLevels, @date);";

                command.Parameters.AddWithValue("@name", currentName);
                command.Parameters.AddWithValue("@score", totalScore);
                command.Parameters.AddWithValue("@stars", totalStars);
                command.Parameters.AddWithValue("@totalCorrectAnswers", totalCorrectAnswers);
                command.Parameters.AddWithValue("@totalAnswers", totalAnswers);
                command.Parameters.AddWithValue("@totalLevels", totalLevels);
                command.Parameters.AddWithValue("@date", DateTime.Now);

                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public AttemptData GetLatestData()
    {
        AttemptData attemptData = new AttemptData();

        attemptData.programmerName = "Lanz";
        attemptData.score = 59400;
        attemptData.stars = 30;
        attemptData.totalCorrectAnswers = 165;
        attemptData.totalAnswers = 165;
        attemptData.totalLevels = 11;
        attemptData.dateTime = DateTime.Now;

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT ROWID, * FROM Attempts ORDER BY ROWID DESC LIMIT 1";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        attemptData.programmerName = reader.GetString("name");
                        attemptData.score = reader.GetInt32("score");
                        attemptData.stars = reader.GetInt32("stars");
                        attemptData.totalCorrectAnswers = reader.GetInt32("totalCorrectAnswers");
                        attemptData.totalAnswers = reader.GetInt32("totalAnswers");
                        attemptData.totalLevels = reader.GetInt32("totalLevels");
                        attemptData.dateTime = reader.GetDateTime("date");
                    }
                }
            }
        }

        return attemptData;
    }

    public List<AttemptData> GetAllAttempts()
    {
        List<AttemptData> attemptDatas = new List<AttemptData>();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT ROWID, * FROM Attempts ORDER BY ROWID";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AttemptData attemptData = new AttemptData();

                        attemptData.programmerName = reader.GetString("name");
                        attemptData.score = reader.GetInt32("score");
                        attemptData.stars = reader.GetInt32("stars");
                        attemptData.totalCorrectAnswers = reader.GetInt32("totalCorrectAnswers");
                        attemptData.totalAnswers = reader.GetInt32("totalAnswers");
                        attemptData.totalLevels = reader.GetInt32("totalLevels");
                        attemptData.dateTime = reader.GetDateTime("date");

                        attemptDatas.Add(attemptData);
                    }
                }
            }
        }

        return attemptDatas;
    }

    public List<AttemptData> GetTopUserDataByScore()
    {
        List<AttemptData> attemptDatas = GetAllAttempts();

        for (int i = 1; i <= 10; i++)
        {
            AttemptData attemptData = new AttemptData();
            attemptData.programmerName = "Bot " + i;
            attemptData.score = i * 100;
            attemptData.stars = 10 + 1 * i;
            attemptData.totalCorrectAnswers = 100 + 2 * i;
            attemptData.totalAnswers = 165;
            attemptData.totalLevels = 11;
            attemptData.dateTime = DateTime.Now;

            attemptDatas.Add(attemptData);
        }
        attemptDatas.Sort((a, b) => b.score.CompareTo(a.score));

        return attemptDatas;
    }

    public LevelData GetLevelDataByName(string levelName)
    {
        LevelData levelData = new LevelData();

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
                        levelData.name = reader.GetString("name");
                        levelData.score = reader.GetInt32("score");
                        levelData.stars = reader.GetInt32("stars");
                        levelData.totalCorrectAnswers = reader.GetInt32("totalCorrectAnswers");
                        levelData.totalAnswers = reader.GetInt32("totalAnswers");
                    }
                    else
                    {
                        SetLevelDataByName(levelName, 0, 0, 0, 0);
                        levelData.name = "";
                        levelData.score = 0;
                        levelData.stars = 0;
                        levelData.totalCorrectAnswers = 0;
                        levelData.totalAnswers = 0;
                    }

                }

                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        return levelData;
    }

    public void SetCurrentLevelData(int score, int stars, int totalCorrectAnswers, int totalAnswers)
    {
        SetLevelDataByName(currentLevelName, score, stars, totalCorrectAnswers, totalAnswers);
    }

    public void SetLevelDataByName(string levelName, int score, int stars, int totalCorrectAnswers, int totalAnswers)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"UPDATE Levels SET score=@score, stars=@stars, totalCorrectAnswers=@totalCorrectAnswers, totalAnswers=@totalAnswers WHERE name = @levelName;";

                command.Parameters.AddWithValue("@levelName", levelName);
                command.Parameters.AddWithValue("@score", score);
                command.Parameters.AddWithValue("@stars", stars);
                command.Parameters.AddWithValue("@totalCorrectAnswers", totalCorrectAnswers);
                command.Parameters.AddWithValue("@totalAnswers", totalAnswers);

                if (command.ExecuteNonQuery() == 0)
                {
                    command.CommandText = $"INSERT INTO Levels VALUES(@levelName, @score, @stars, @totalCorrectAnswers, @totalAnswers)";

                    command.Parameters.AddWithValue("@levelName", levelName);
                    command.Parameters.AddWithValue("@score", score);
                    command.Parameters.AddWithValue("@stars", stars);
                    command.Parameters.AddWithValue("@totalCorrectAnswers", totalCorrectAnswers);
                    command.Parameters.AddWithValue("@totalAnswers", totalAnswers);

                    command.ExecuteNonQuery();
                }
            }

            connection.Close();
        }
    }
}
