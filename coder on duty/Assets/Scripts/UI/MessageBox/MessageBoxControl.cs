using System;
using UnityEngine;
using UnityEngine.Events;

public class MessageBoxControl : MonoBehaviour
{
    [SerializeField] private GameObject messageBoxPrefab;
    [SerializeField] private Transform messageBoxParent;

    private static GameObject _messageBoxPrefab;
    private static Transform _messageBoxParent;


    void Awake()
    {
        _messageBoxPrefab = messageBoxPrefab;
        _messageBoxParent = messageBoxParent;
    }

    private static MessageBox Open()
    {
        GameObject messageBox = Instantiate(_messageBoxPrefab, _messageBoxParent);
        return messageBox.GetComponent<MessageBox>();
    }

    public static void TriggerShowOk(string title, string message, UnityEvent okAction = null)
    {
        MessageBox messageBox = Open();
        messageBox.ShowOk(title, message, okAction);
    }

    public static void TriggerShowYesNo(string title, string message, UnityEvent yesAction = null, UnityEvent noAction = null)
    {
        MessageBox messageBox = Open();
        messageBox.ShowYesNo(title, message, yesAction, noAction);
    }

    public static void TriggerShowYesNoCancel(string title, string message, UnityEvent yesAction = null, UnityEvent noAction = null, UnityEvent cancelAction = null)
    {
        MessageBox messageBox = Open();
        messageBox.ShowYesNoCancel(title, message, yesAction, noAction, cancelAction);
    }

    public static void ShowOk(string title, string message, Action okAction = null)
    {
        MessageBox messageBox = Open();
        messageBox.ShowOk(title, message, okAction);
    }

    public static void ShowYesNo(string title, string message, Action yesAction = null, Action noAction = null)
    {
        MessageBox messageBox = Open();
        messageBox.ShowYesNo(title, message, yesAction, noAction);
    }

    public static void ShowYesNoCancel(string title, string message, Action yesAction = null, Action noAction = null, Action cancelAction = null)
    {
        MessageBox messageBox = Open();
        messageBox.ShowYesNoCancel(title, message, yesAction, noAction, cancelAction);
    }
}
