using UnityEngine;

[System.Serializable]
public class NPCDialogue
{
    [TextArea(3, 10)] public string[] sentences;

    public NPCDialogue(string[] sentences)
    {
        this.sentences = sentences;
    }
}
