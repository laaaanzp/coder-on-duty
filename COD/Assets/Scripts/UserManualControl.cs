using UnityEngine;
using UnityEngine.UI;

public class UserManualControl : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] Button nextButton;
    [SerializeField] Button previousButton;

    private Sprite[] spriteImages;
    private int currentIndex;


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

        if (currentIndex == 0)
        {
            previousButton.interactable = false;
        }
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
}

