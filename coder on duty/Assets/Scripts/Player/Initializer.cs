using UnityEngine;
using UnityEngine.Events;

public class Initializer : MonoBehaviour
{
    public UnityEvent initalizers;


    void Awake()
    {
        initalizers?.Invoke();
    }
}
