using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelNameText;

    void Awake()
    {
        int levelIndex = 1;

        try
        {
            levelIndex = DatabaseManager.instance.currentLanguage.currentLevel;
        } catch { }

        levelNameText.text = $"{levelIndex} - {SceneManager.GetActiveScene().name}";
    }
}
