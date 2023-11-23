using System;
using Tymski;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public UnityEvent onSceneLoad;

    private static SceneSwitcher instance;

    public SceneReference mainMenuScene;


    void Awake()
    {
        instance = this;
    }

    private void Transition(Action onFinish)
    {
        CanvasGroup imageOverlay = 
            GameObject.FindGameObjectWithTag("SceneSwitcherOverlay")
                      .GetComponent<CanvasGroup>();

        imageOverlay.blocksRaycasts = true;
        imageOverlay.LeanAlpha(1f, 1f)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                onFinish?.Invoke();
                onSceneLoad?.Invoke();

                imageOverlay.LeanAlpha(0f, 1f)
                    .setIgnoreTimeScale(true)
                    .setOnComplete(() =>
                    {
                        imageOverlay.blocksRaycasts = false;
                    });
            });
    }

    public static void LoadScene(int sceneId)
    {
        instance.Transition(() =>
        {
            SceneManager.LoadScene(sceneId);
        });
    }

    public static void LoadScene(string sceneName)
    {
        instance.Transition(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }

    public static void LoadScene(SceneReference sceneReference)
    {
        instance.Transition(() =>
        {
            SceneManager.LoadScene(sceneReference);
        });
    }

    public static void Restart()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void LoadMenu()
    {
        LoadScene(0);
    }

    public static void LoadEndScreen()
    {
        LoadScene(3);
    }

    public static void QuitApplication()
    {
        Application.Quit();
    }
}
