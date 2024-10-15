using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public List<LightSwitch> lightSwitches;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            foreach (LightSwitch lightSwitch in lightSwitches)
            {
                lightSwitch.Toggle();
            }
        }
    }
}
