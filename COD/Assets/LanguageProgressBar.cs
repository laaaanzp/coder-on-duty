using UnityEngine;

public class LanguageProgressBar : MonoBehaviour
{
    [SerializeField] private string languageName;

    void Start()
    {
        float progressPercentage = LanguageDatabase.GetInstance(languageName).progressPercentage;
        transform.localScale = new Vector3(progressPercentage, 1f, 1f);
    }
}
