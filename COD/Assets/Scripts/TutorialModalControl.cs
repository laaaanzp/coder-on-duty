using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialModalControl : MonoBehaviour
{
    [SerializeField] private ModalControl tutorialModal;

    private static TutorialModalControl instance;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public static void Open()
    {
        instance.tutorialModal.Open();
    }
}
