using System;
using UnityEngine;

public class TypeChooser : MonoBehaviour
{
    [SerializeField] private TemplateChooser templateChooser;
    [SerializeField] private ModalControl templateChooserModal;
    private string languageName;
    private Action<string> onCallback;


    public void Open(string languageName, Action<string> onCallback)
    {
        this.languageName = languageName;
        this.onCallback = onCallback;
        templateChooserModal.Open();
    }

    public void Close()
    {
        templateChooserModal.Close();
    }

    public void Inheritance()
    {
        templateChooser.Open(languageName, "Inheritance", onCallback);
        Close();
    }

    public void Encapsulation()
    {
        templateChooser.Open(languageName, "Encapsulation", onCallback);
        Close();
    }

    public void Abstraction()
    {
        templateChooser.Open(languageName, "Abstraction", onCallback);
        Close();
    }

    public void Polymorphism()
    {
        templateChooser.Open(languageName, "Polymorphism", onCallback);
        Close();
    }
}
