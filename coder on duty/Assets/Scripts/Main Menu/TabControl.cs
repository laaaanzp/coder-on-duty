using TMPro;
using UnityEngine;

public class TabControl : MonoBehaviour
{
    public TextMeshProUGUI textDescription;

    [Header("Color")]
    public Color selectedColor;
    public Color unselectedColor;

    private Tab[] tabs;


    private void Awake()
    {
        tabs = GetComponentsInChildren<Tab>();
    }

    public void OnTabClick(Tab selectedTab)
    {
        foreach (Tab tab in tabs)
        {
            bool isTheSelectedTab = tab == selectedTab;

            SetState(tab, isTheSelectedTab);
            tab.content.SetActive(isTheSelectedTab);
        }
    }

    public void OnTabHover(Tab tab)
    {
        textDescription.text = tab.description;
    }

    public void ClearDescription()
    {
        textDescription.text = "";
    }

    private void SetState(Tab tab, bool selected)
    {
        tab.imageStatus.SetActive(selected);
        tab.backgroundImage.color = selected ? selectedColor : unselectedColor;
    }
}
