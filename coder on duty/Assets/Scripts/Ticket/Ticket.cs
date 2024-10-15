using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ticket : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button navigateButton;

    [HideInInspector] public bool isFinished = false;
    [HideInInspector] public bool isFixed;
    [HideInInspector] public GameObject owner;

    private Action<Ticket> onFix;

    public void Initialize(GameObject owner, string title, Action<Ticket> onFix)
    {
        this.owner = owner;
        titleText.text = title;
        this.onFix = onFix;

        navigateButton.onClick.AddListener(() =>
        {
            Navigate();
            TicketModalControl.Close();
        });
    }

    public void Finish()
    {
        if (isFinished)
            return;

        isFinished = true;
        onFix?.Invoke(this);
        Destroy(gameObject);
    }

    public void Navigate()
    {
        ObjectNavigation.Navigate(this.owner);
    }
}
