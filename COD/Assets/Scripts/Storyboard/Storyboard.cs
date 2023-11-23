using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Storyboard : MonoBehaviour
{
    [SerializeField] private Dialouge[] dialouges;
    [SerializeField] private float inputDelay;
    [SerializeField] private float dialougeDelay;
    [SerializeField] private Image storyboardImage;
    [SerializeField] private RectTransform rect;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI messageText;

    private int index = 0;
    private float currentTime = 0f;
    private bool isExiting = false;
    private bool isPlaying = false;


    void Awake()
    {
        HandleUserInput();
    }

    public void HandleUserInput()
    {
        Dialouge currentDialouge = dialouges[index];

        if (!isPlaying)
        {
            isPlaying = true;
            StartCoroutine(NextDialouge(currentDialouge));
        }
        else
        {
            StopAllCoroutines();
            messageText.text = currentDialouge.message;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            isPlaying = false;
            index++;
        }

        currentTime = 0f;
    }

    IEnumerator NextDialouge(Dialouge dialouge)
    {
        canvasGroup.alpha = 0f;

        if (dialouge.image != null)
        {
            if (dialouge.transitionImage)
            {
                TransitionImage(dialouge.image);
                yield return new WaitForSeconds(1.5f);
            }
            else
            {
                storyboardImage.sprite = dialouge.image;
            }
        }

        if (string.IsNullOrWhiteSpace(dialouge.name))
        {
            try
            {
                nameText.text = DatabaseManager.instance.currentLanguage.currentName;
            }
            catch
            {
                nameText.text = "Lanz"; 
            }
        }
        else
        {
            nameText.text = dialouge.name;
        }

        canvasGroup.alpha = 1f;

        messageText.text = "";

        foreach (char c in dialouge.message)
        {
            messageText.text += c;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            yield return new WaitForSeconds(dialougeDelay);
        }

        isPlaying = false;
        index++;
    }

    private void TransitionImage(Sprite image)
    {
        LeanTween.color(storyboardImage.rectTransform, new Color(1f, 1f, 1f, 0f), 0.5f).setOnComplete(() =>
        {
            storyboardImage.sprite = image;
            LeanTween.color(storyboardImage.rectTransform, new Color(1f, 1f, 1f, 1f), 0.5f);
        });
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime < inputDelay || isExiting)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (index == dialouges.Length)
            {
                SceneSwitcher.LoadScene(1);
                isExiting = true;
                return;
            }

            HandleUserInput();
        }
    }
}
