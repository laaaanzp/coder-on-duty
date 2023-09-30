using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ticket : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button navigateButton;

    [HideInInspector] public bool isFixed;
    [HideInInspector] public GameObject owner;
    [HideInInspector] public int score;

    private Action<Ticket> onFix;


    public void Initialize(GameObject owner, string title, int score, Action<Ticket> onFix)
    {
        this.owner = owner;
        titleText.text = title;
        scoreText.text = $"<b>Score:</b> {score}";
        this.onFix = onFix;
        this.score = score;

        navigateButton.onClick.AddListener(Navigate);
    }

    public void Finish()
    {
        if (isFixed)
            return;

        isFixed = true;
        onFix?.Invoke(this);
        Destroy(gameObject);
    }

    public void Navigate()
    {
        ObjectNavigation.Navigate(this.owner);
    }
}
