using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistoryRecord : MonoBehaviour
{
    [SerializeField] private string languageName;
    [SerializeField] private Transform historyRecordContainer;
    [SerializeField] private ScrollRect historyRecordContainerScrollRect;
    [SerializeField] private GameObject[] historyEntryPrefabs;
    private int index = 0;


    void Start()
    {
        List<AttemptData> attemptDatas = LanguageDatabase.GetInstance(languageName).GetAllAttempts();

        attemptDatas.ForEach(attempData =>
        {
            GameObject entry = Instantiate(historyEntryPrefabs[index], historyRecordContainer);
            HistoryRecordEntry historyRecord = entry.GetComponent<HistoryRecordEntry>();
            historyRecord.SetValue(attempData);
            index = index == 0 ? 1 : 0;
        });

        LayoutRebuilder.ForceRebuildLayoutImmediate(historyRecordContainer.GetComponent<RectTransform>());
        historyRecordContainerScrollRect.normalizedPosition = new Vector2(0, 1);
    }
}
