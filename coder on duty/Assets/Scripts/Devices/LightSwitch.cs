using UnityEngine;

public class LightSwitch : MonoBehaviour, IElectricDevice
{
    public GameObject lightDevice;


    public void Toggle()
    {
        lightDevice.SetActive(!lightDevice.activeInHierarchy);
    }
}
