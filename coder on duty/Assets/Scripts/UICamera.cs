using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICamera : MonoBehaviour
{
    [SerializeField] private GameObject targetCamera;
    [SerializeField] private Canvas canvas;

    void Update()
    {
        canvas.enabled = !targetCamera.activeInHierarchy;
    }
}
