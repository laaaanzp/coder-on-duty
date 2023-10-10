using System.IO;
using TMPro;
using Tymski;
using UnityEngine;

public class LanguageProgressBar : MonoBehaviour
{
    [SerializeField] private string languageName;
    [SerializeField] private TextMeshProUGUI levelNameText;


    void OnEnable()
    {
        float progressPercentage = LanguageDatabase.GetInstance(languageName).progressPercentage;
        transform.localScale = new Vector3(progressPercentage, 1f, 1f);

        int currentLevel = LanguageDatabase.GetInstance(languageName).currentLevel;

        SceneReference scene = LanguageDatabase.GetInstance(languageName).scenes[currentLevel - 1];

        string levelName = Path.GetFileName(scene.ScenePath).Replace(".unity", "");

        levelNameText.text = $"Current Level: {currentLevel} - {levelName}";
    }
}
