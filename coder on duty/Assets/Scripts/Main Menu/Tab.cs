using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{
    public GameObject content;
    public string description;
    
    [HideInInspector] public GameObject imageStatus;
    [HideInInspector] public Image backgroundImage;

    private TabControl tabControl;


    void Awake()
    {
        tabControl = GetComponentInParent<TabControl>();
        imageStatus = transform.GetChild(0).gameObject;
        backgroundImage = GetComponent<Image>();
    }

    public void OnMouseClick()
    {
        tabControl.OnTabClick(this);
    }

    public void OnMouseHover()
    {
        tabControl.OnTabHover(this);
    }

    public void OnMouseExit()
    {
        tabControl.ClearDescription();
    }
}
