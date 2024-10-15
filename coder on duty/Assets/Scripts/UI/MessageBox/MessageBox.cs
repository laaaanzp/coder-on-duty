using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    [SerializeField] private ModalControl modalControl;
    [SerializeField] private RectTransform messageRectTransform;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Buttons")]
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private Button cancelButton;

    private UnityEvent ueYesAction;
    private UnityEvent ueNoAction;
    private UnityEvent ueCancelAction;

    private Action aYesAction;
    private Action aNoAction;
    private Action aCancelAction;


    void Awake()
    {
        modalControl.onClose += () => { Destroy(gameObject); };
    }

    private void FixLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(messageRectTransform);
    }

    public void ShowOk(string title, string content, UnityEvent okAction = null)
    {
        this.ueYesAction = okAction;
        yesButton.GetComponentInChildren<TextMeshProUGUI>().text = "OK";

        noButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);

        SetTitleAndContentText(title, content);
        Open();
    }

    public void ShowYesNo(string title, string message, UnityEvent yesAction = null, UnityEvent noAction = null)
    {
        this.ueYesAction = yesAction;
        this.ueNoAction = noAction;

        cancelButton.gameObject.SetActive(false);

        SetTitleAndContentText(title, message);
        Open();
    }

    public void ShowYesNoCancel(string title, string message, UnityEvent yesAction = null, UnityEvent noAction = null, UnityEvent cancelAction = null)
    {
        this.ueYesAction = yesAction;
        this.ueNoAction = noAction;
        this.ueCancelAction = cancelAction;

        SetTitleAndContentText(title, message);
        Open();
    }

    public void ShowOk(string title, string content, Action okAction = null)
    {
        aYesAction = okAction;
        yesButton.GetComponentInChildren<TextMeshProUGUI>().text = "OK";

        noButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);

        SetTitleAndContentText(title, content);
        Open();
    }

    public void ShowYesNo(string title, string message, Action yesAction = null, Action noAction = null)
    {
        aYesAction = yesAction;
        aNoAction = noAction;

        cancelButton.gameObject.SetActive(false);

        SetTitleAndContentText(title, message);
        Open();
    }

    public void ShowYesNoCancel(string title, string message, Action yesAction = null, Action noAction = null, Action cancelAction = null)
    {
        aYesAction = yesAction;
        aNoAction = noAction;
        aCancelAction = cancelAction;

        SetTitleAndContentText(title, message);
        Open();
    }

    private void SetTitleAndContentText(string title, string message)
    {
        titleText.text = title;
        messageText.text = message;
    }

    private void Open()
    {
        modalControl.Open();
        FixLayout();
    }

    private void Close()
    {
        modalControl.Close();
    }

    public void YesAction()
    {
        ueYesAction?.Invoke();
        aYesAction?.Invoke();
        Close();
    }

    public void NoAction()
    {
        ueNoAction?.Invoke();
        aNoAction?.Invoke();
        Close();
    }

    public void CancelAction()
    {
        ueCancelAction?.Invoke();
        aCancelAction?.Invoke();
        Close();
    }
}
