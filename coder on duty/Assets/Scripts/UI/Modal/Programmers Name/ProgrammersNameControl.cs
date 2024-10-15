using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgrammersNameControl : MonoBehaviour
{
    [SerializeField] private GameObject programmersNamePrefab;
    [SerializeField] private Transform programmersNameParent;

    private static GameObject _programmersNamePrefab;
    private static Transform _programmersNameParent;


    void Awake()
    {
        _programmersNamePrefab = programmersNamePrefab;
        _programmersNameParent = programmersNameParent;
    }

    public static void Show(Action<string> onCallback)
    {
        GameObject programmersName = Instantiate(_programmersNamePrefab, _programmersNameParent);
        ProgrammersNameModal programmersNameModal = programmersName.GetComponent<ProgrammersNameModal>();

        programmersNameModal.Open(onCallback);
    }
}
