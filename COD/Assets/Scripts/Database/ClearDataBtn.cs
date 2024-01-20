using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearDataBtn : MonoBehaviour
{
    public void ClearCS()
    {
        MessageBoxControl.ShowYesNo(
            "CLEAR C# PROGRESS?",
            "Do you want to clear your current C# progress?",
            () =>
            {
                SecurePlayerPrefs.Init();
                SecurePlayerPrefs.SetString("csharp-name", "");
                SecurePlayerPrefs.SetInt("csharp-level", 1);
                SecurePlayerPrefs.SetInt("csharp-score", 0);
                SecurePlayerPrefs.SetInt("csharp-time", 0);
                SecurePlayerPrefs.SetInt("csharp-accuracy", 0);
                SecurePlayerPrefs.SetInt("csharp-stars", 0);

                string[] types = { "DragAndArrangePuzzle", "DragAndDropPuzzle", "FindAndSelect" };

                foreach (string type in types)
                {
                    PlayerPrefs.SetInt($"csharp-{type}-total-correct", -1);
                    PlayerPrefs.SetInt($"csharp-{type}-time-remaining", -1);
                    PlayerPrefs.SetInt($"csharp-{type}-total-slots", -1);
                }

                PlayerPrefs.Save();

                LanguageDatabase.GetInstance("csharp").ResetProgress();
            });
    }

    public void ClearJava()
    {
        MessageBoxControl.ShowYesNo(
            "CLEAR C# PROGRESS?",
            "Do you want to clear your current C# progress?",
            () =>
            {
                SecurePlayerPrefs.Init();
                SecurePlayerPrefs.SetString("java-name", "");
                SecurePlayerPrefs.SetInt("java-level", 1);
                SecurePlayerPrefs.SetInt("java-score", 0);
                SecurePlayerPrefs.SetInt("java-time", 0);
                SecurePlayerPrefs.SetInt("java-accuracy", 0);
                SecurePlayerPrefs.SetInt("java-stars", 0);

                string[] types = { "DragAndArrangePuzzle", "DragAndDropPuzzle", "FindAndSelect" };

                foreach (string type in types)
                {
                    PlayerPrefs.SetInt($"java-{type}-total-correct", -1);
                    PlayerPrefs.SetInt($"java-{type}-time-remaining", -1);
                    PlayerPrefs.SetInt($"java-{type}-total-slots", -1);
                }

                PlayerPrefs.Save();

                LanguageDatabase.GetInstance("java").ResetProgress();
            });
    }
}
