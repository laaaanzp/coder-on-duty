using UnityEngine;
using UnityEngine.EventSystems;

public class ArrangeableNode : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform currentTransform;
    private GameObject mainContent;
    private Vector3 currentPossition;

    private int totalChild;

    public void OnPointerDown(PointerEventData eventData)
    {
        currentPossition = currentTransform.position;
        mainContent = currentTransform.parent.gameObject;
        totalChild = mainContent.transform.childCount;
    }

    public void OnDrag(PointerEventData eventData)
    {
        currentTransform.position =
            new Vector3(currentTransform.position.x, eventData.position.y, currentTransform.position.z);

        for (int i = 0; i < totalChild; i++)
        {
            if (i != currentTransform.GetSiblingIndex())
            {
                Transform otherTransform = mainContent.transform.GetChild(i);

                int distance = (int)Vector3.Distance(currentTransform.position, otherTransform.position);

                if (distance <= 10)
                {
                    Vector3 otherTransformOldPosition = otherTransform.position;
                    Vector3 otherTransformNewPosition = new Vector3(otherTransform.position.x, currentPossition.y, otherTransform.position.z);

                    otherTransform.position = otherTransformNewPosition;

                    currentTransform.position = new Vector3(currentTransform.position.x, otherTransformOldPosition.y, currentTransform.position.z);
                    currentTransform.SetSiblingIndex(otherTransform.GetSiblingIndex());
                    currentPossition = currentTransform.position;
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        currentTransform.position = currentPossition;
    }
}
