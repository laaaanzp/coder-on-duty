using UnityEngine;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private string languageName;


    public void Play()
    {
        LanguageDatabase.Play(languageName);
    }
}