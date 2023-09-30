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

        if (filterBy == "Time")
        {
            languageDatabase.FetchTopUsersByTime(LoadData, null);
        }
        else
        {
            languageDatabase.FetchTopUsersByScore(LoadData, null);
        }
    }

    private void SetLoading()
    {
        foreach (LeaderboardEntry entry in leaderboardEntries)
        {
            entry.SetValues("Loading...", "Loading...");
        }
    }

    private void LoadData(Dictionary<string, int> datas)
    {
        int iterationCount = 0;
        string filterBy = filterByDropdown.options[filterByDropdown.value].text;

        foreach (KeyValuePair<string, int> data in datas)
        {
            if (iterationCount >= leaderboardEntries.Count)
            {
                break;
            }

            string name = data.Key;
            int value = data.Value;

            string valueAsString;

            if (filterBy == "Time")
            {
                valueAsString = SecondsToTimeString(value);
            }
            else
            {
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
