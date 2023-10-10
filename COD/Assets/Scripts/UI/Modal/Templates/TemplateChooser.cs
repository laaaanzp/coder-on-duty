using System;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class TemplateChooser : MonoBehaviour
{
    [SerializeField] private ModalControl templateChooserModal;
    [SerializeField] private Transform templateItemParent;
    [SerializeField] private GameObject templateItemPrefab;


    public void Open(string languageName, string typeName, Action<string> onCallback)
    {
        ClearItems();

        TextAsset[] templates = Resources.LoadAll<TextAsset>($"Templates/{languageName}/{typeName}");

        foreach (TextAsset template in templates)
        {
            AddItem(template.name, template.text, onCallback);
        }

        AddCloseButton();

        templateChooserModal.Open();
    }

    public void Close()
    {
        templateChooserModal.Close();
    }

    private void AddItem(string name, string code, Action<string> onCallback)
    {
        GameObject itemButton = Instantiate(templateItemPrefab, templateItemParent);
        UnityEngine.UI.Button button = itemButton.GetComponent<UnityEngine.UI.Button>();
        button.GetComponentInChildren<TextMeshProUGUI>().text = name;

        button.onClick.AddListener(() =>
        {
            MessageBoxControl.ShowYesNo("WARNING", "This will overwrite the current code. Continue?",
                () =>
                {
                    onCallback?.Invoke(code);
                    Close();
                });
        });
    }

    private void AddCloseButton()
    {
        GameObject itemButton = Instantiate(templateItemPrefab, templateItemParent);
        UnityEngine.UI.Button button = itemButton.GetComponent<UnityEngine.UI.Button>();
        button.GetComponentInChildren<TextMeshProUGUI>().text = "CLOSE";
        button.GetComponent<Image>().color = new Color(1f, 0.294f, 0.294f, 1f);

        button.onClick.AddListener(() =>
        {
            Close();
        });
    }    

    private void ClearItems()
    {
        foreach (Transform templateItem in templateItemParent.transform)
        {
            Destroy(templateItem.gameObject);
        }
    }
}
