using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private ModalControl modalControl;

    private Queue<string> sentences;
    private Action callback;
    private bool isTyping;

    private static DialogueManager instance;

    void Awake()
    {
        sentences = new Queue<string>();
        instance = this;
        modalControl.Close();
    }

    public static void StartDialogue(string name, NPCDialogue dialogue, Action callback = null)
    {
        instance.nameText.text = name;
        instance.callback = callback;

        instance.sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            instance.sentences.Enqueue(sentence);
        }

        instance.modalControl.Open();

        instance.DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (isTyping)
            return;

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();

        isTyping = true;
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        AudioController.PlayTyping();

        messageText.text = "";

        foreach (char c in sentence)
        {
            messageText.text += c;
            yield return new WaitForSecondsRealtime(0.01f);
        }

        yield return new WaitForSecondsRealtime(0.5f);

        isTyping = false;
    }

    private void EndDialogue()
    {
        modalControl.Close();
        callback?.Invoke();
        // callback = null;
    }
}
