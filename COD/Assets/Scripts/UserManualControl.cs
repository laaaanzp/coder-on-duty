using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UserManualControl : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] Button nextButton;
    [SerializeField] Button previousButton;
    [SerializeField] GameObject tryCodeButton;

    private Sprite[] spriteImages;
    private int currentIndex;
    private string code;


    void Awake()
    {
        previousButton.interactable = false;
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        string levelName = DatabaseManager.instance.currentLanguage.currentLevelName;

        spriteImages = Resources.LoadAll<Sprite>($"User Manuals/{languageName}/{levelName}/");

        currentIndex = 0;
        image.sprite = spriteImages[0];
    }

    public void Next()
    {
        currentIndex++;
        image.sprite = spriteImages[currentIndex];

        previousButton.interactable = true;

        CheckIfCodeExists();

        if (currentIndex + 1 == spriteImages.Length)
        {
            nextButton.interactable = false;
        }
    }

    public void Previous()
    {
        currentIndex--;
        image.sprite = spriteImages[currentIndex];

        nextButton.interactable = true;

        CheckIfCodeExists();

        if (currentIndex == 0)
        {
            previousButton.interactable = false;
        }
    }

    private void CheckIfCodeExists()
    {
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        string levelName = DatabaseManager.instance.currentLanguage.currentLevelName;

        string codePath = $"User Manuals/codes/{languageName}/{levelName}/{currentIndex + 1}";
        TextAsset codeTextAsset = Resources.Load<TextAsset>(codePath);

        if (codeTextAsset != null)
            code = codeTextAsset.text;

        tryCodeButton.SetActive(codeTextAsset != null);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) && previousButton.interactable)
        {
            Previous();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && nextButton.interactable)
        {
            Next();
        }
    }

    public void TryCode()
    {
        CSCodeCompiler.Open(code);
    }
}

