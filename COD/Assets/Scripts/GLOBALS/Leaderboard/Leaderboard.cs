using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private Transform entriesContainer;
    [SerializeField] private TextMeshProUGUI valueText;

    [Header("Filters")]
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private TMP_Dropdown filterByDropdown;

    private List<LeaderboardEntry> leaderboardEntries;


    void Start()
    {
        leaderboardEntries = new List<LeaderboardEntry>();

        foreach (Transform child in entriesContainer)
        {
            LeaderboardEntry entry = child.GetComponent<LeaderboardEntry>();

            leaderboardEntries.Add(entry);
        }

        Reload();
    }

    public void Reload()
    {
        SetLoading();
        string languageName = languageDropdown.options[languageDropdown.value].text;
        string filterBy = filterByDropdown.options[filterByDropdown.value].text;

        valueText.text = filterBy;

        if (languageName.ToLower() == "c#")
            languageName = "csharp";

        LanguageDatabase languageDatabase = LanguageDatabase.GetInstance(languageName.ToLower());

        List<UserData> userDatas;

        if (filterBy == "Time")
        {
            userDatas = languageDatabase.GetTopUserDataByTime();
        }
        else
        {
            userDatas = languageDatabase.GetTopUserDataByScore();
        }

        LoadData(userDatas);
    }

    private void SetLoading()
    {
        foreach (LeaderboardEntry entry in leaderboardEntries)
        {
            entry.SetValues("-", "-");
        }
    }

    private void LoadData(List<UserData> userDatas)
    {
        int iterationCount = 0;
        string filterBy = filterByDropdown.options[filterByDropdown.value].text;
        List<string> names = new List<string>();

        foreach (UserData userData in userDatas)
        {
            if (iterationCount >= leaderboardEntries.Count)
            {
                break;
            }
            if (names.Contains(userData.name))
            {
                continue;
            }
            else
            {
                names.Add(userData.name);
            }

            string name = userData.name;
            int value;

            string valueAsString;

            if (filterBy == "Time")
            {
                value = userData.time;
                valueAsString = SecondsToTimeString(value);
            }
            else
            {
                value = userData.score;
                valueAsString = value.ToString();
            }

            leaderboardEntries[iterationCount].SetValues(name, valueAsString);

            iterationCount++;
        }
    }

    private string SecondsToTimeString(int time)
    {
        int minutes = time / 60;
        int seconds = time % 60;

        string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);

        return timeString;
    }
}
