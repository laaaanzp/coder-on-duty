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

        List<AttemptData> userDatas = languageDatabase.GetTopUserDataByScore();

        LoadData(userDatas);
    }

    private void SetLoading()
    {
        foreach (LeaderboardEntry entry in leaderboardEntries)
        {
            entry.SetValues("-", "-");
        }
    }

    private void LoadData(List<AttemptData> userDatas)
    {
        int iterationCount = 0;
        string filterBy = filterByDropdown.options[filterByDropdown.value].text;
        List<string> names = new List<string>();

        foreach (AttemptData attemptData in userDatas)
        {
            if (iterationCount >= leaderboardEntries.Count)
            {
                break;
            }
            if (names.Contains(attemptData.programmerName))
            {
                continue;
            }
            else
            {
                names.Add(attemptData.programmerName);
            }

            string name = attemptData.programmerName;
            string value = attemptData.score.ToString();

            leaderboardEntries[iterationCount].SetValues(name, value);

            iterationCount++;
        }
    }
}
