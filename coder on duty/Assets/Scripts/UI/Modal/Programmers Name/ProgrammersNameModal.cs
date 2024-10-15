using System;
using TMPro;
using UnityEngine;

public class ProgrammersNameModal : MonoBehaviour
{
    [SerializeField] private ModalControl modalControl;
    [SerializeField] private TMP_InputField programmersNameInputField;

    private Action<string> onInput;

    void Awake()
    {
        modalControl.onClose += () => { Destroy(gameObject); };
    }

    public void Open(Action<string> onCallback)
    {
        onInput = onCallback;
        modalControl.Open();
    }

    public void Submit()
    {
        string programmersName = programmersNameInputField.text;

        if (programmersName.Length < 4 || programmersName.Length > 16 || string.IsNullOrWhiteSpace(programmersName))
        {
            MessageBoxControl.ShowOk("ERROR", "The name should be 4 to 16 characters long.");
        }
        else
        {
            onInput?.Invoke(programmersName);
            modalControl.Close();
        }
    }
}
