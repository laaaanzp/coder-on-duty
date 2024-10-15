using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject title;
    public Image backgroundImage;
    public LeanTweenType leanType;
    [SerializeField] private AudioSource menuAudioSource;

    public static float duration = 1f;

    private CanvasGroup titleCanvasGroup;


    void Awake()
    {
        titleCanvasGroup = title.GetComponent<CanvasGroup>();
    }

    void Start()
    {
        AnimateTitle();
    }

    public void TransitionAudio()
    {
        float currentVolume = menuAudioSource.volume;
        LeanTween.value(1f, 0f, 1f).setOnUpdate((float value) =>
        {
            menuAudioSource.volume = value * currentVolume;
        });
    }

    private void AnimateTitle()
    {
        titleCanvasGroup.LeanAlpha(0f, 0f);
        titleCanvasGroup.LeanAlpha(1f, duration)
            .setOnComplete(MoveAndResizeTitle);
    }

    private void MoveAndResizeTitle()
    {
        LeanTween.color(backgroundImage.rectTransform, new Color(0f, 0f, 0f, 0.3f), duration)
            .setDelay(1f)
            .setEase(leanType);

        title.LeanMoveLocalY(300f, duration)
            .setDelay(1f)
            .setEase(leanType);

        title.LeanScale(new Vector3(.8f, .8f, .8f), duration)
            .setDelay(1f)
            .setEase(leanType)
            .setOnComplete(() =>
            {
                StartCoroutine(ShowButtons());
            });
    }

    private IEnumerator ShowButtons()
    {
        MenuOption[] menuOptions = GetComponentsInChildren<MenuOption>(includeInactive: true);

        foreach (MenuOption menuOption in menuOptions)
        {
            menuOption.gameObject.SetActive(true);
            menuOption.Show();
            yield return new WaitForSeconds(.2f);
        }

        duration = 0f;
    }
}
