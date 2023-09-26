using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCooldown : MonoBehaviour
{
    [SerializeField] private float cooldown = 10.0f;

    private Button button;
    private TextMeshProUGUI buttonText;
    private string originalButtonText;

    private bool isOnCooldown;
    private float remainingCooldown;


    void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        button.onClick.AddListener(OnClick);
    }

    void Update()
    {
        if (!isOnCooldown)
            return;

        remainingCooldown -= Time.deltaTime;

        UpdateText();

        if (remainingCooldown <= 0)
        {
            button.interactable = true;
            isOnCooldown = false;
            buttonText.text = originalButtonText;
        }
    }

    private void OnClick()
    {
        button.interactable = false;
        originalButtonText = buttonText.text;
        remainingCooldown = cooldown;
        isOnCooldown = true;
    }

    private void UpdateText()
    {
        buttonText.text = $"{(int)Mathf.Round(remainingCooldown)}s";
    }
}
