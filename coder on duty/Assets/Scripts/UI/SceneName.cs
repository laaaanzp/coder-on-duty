using TMPro;
using UnityEngine;

public class SceneName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelNameText;

    void Awake()
    {
        LanguageDatabase languageDatabase = DatabaseManager.instance.currentLanguage;

        string languageName = languageDatabase.name;
        levelNameText.text = $"({languageName}) Level {languageDatabase.currentLevel} - {languageDatabase.currentLevelName}";
    }
}
