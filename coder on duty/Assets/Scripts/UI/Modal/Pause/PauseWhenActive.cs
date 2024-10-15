using UnityEngine;

public class PauseWhenActive : MonoBehaviour
{
    private float defaultTimeScale;


    void OnEnable()
    {
        defaultTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    void OnDisable()
    {
        Time.timeScale = defaultTimeScale;
    }
}
