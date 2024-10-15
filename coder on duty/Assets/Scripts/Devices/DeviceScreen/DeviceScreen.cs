using UnityEngine;

public class DeviceScreen : MonoBehaviour
{
    public Material turnedOnMaterial;
    public Material turnedOffMaterial;
    public int screenMaterialIndex;
    public bool isTurnedOn;


    public void ChangeScreen(Material screenMaterial)
    {
        Renderer screenRenderer = GetComponent<Renderer>();
        Material[] materials = screenRenderer.sharedMaterials;

        materials[screenMaterialIndex] = screenMaterial;
        screenRenderer.materials = materials;
    }

    public void TurnOnScreen()
    {
        ChangeScreen(turnedOnMaterial);
    }

    public void TurnOffScreen()
    {
        ChangeScreen(turnedOffMaterial);
    }

    public void ReloadScreen()
    {
        if (isTurnedOn)
            TurnOnScreen();

        else
            TurnOffScreen();
    }

    void OnValidate()
    {
        ReloadScreen();
    }
}
