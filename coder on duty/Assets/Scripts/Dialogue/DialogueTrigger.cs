using System;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    public NPCDialogue dialogue;
    public UnityEvent onFinish;

    public void TriggerDialogue()
    {
        DialogueManager.StartDialogue(gameObject.name, dialogue, () => { onFinish?.Invoke(); });
    }
}
