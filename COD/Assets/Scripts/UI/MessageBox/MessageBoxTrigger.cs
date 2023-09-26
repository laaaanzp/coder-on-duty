using UnityEngine;
using UnityEngine.Events;

public class MessageBoxTrigger : MonoBehaviour
{
    [SerializeField] private MessageBoxType messageBoxType;
    [SerializeField] private string title;
    [SerializeField] [Multiline] private string message;

    [Header("Feedback Actions")]
    [SerializeField] UnityEvent yesAction;
    [SerializeField] UnityEvent noAction;
    [SerializeField] UnityEvent cancelAction;


    public void Open()
    {
        switch (messageBoxType)
        {
            case MessageBoxType.OK:
                MessageBoxControl.TriggerShowOk(title, message);
                break;
            case MessageBoxType.YES_NO:
                MessageBoxControl.TriggerShowYesNo(title, message, yesAction, noAction);
                break;
            case MessageBoxType.YES_NO_CANCEL:
                MessageBoxControl.TriggerShowYesNoCancel(title, message, yesAction, noAction, cancelAction);
                break;
        }
    }
}
