using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollBar;

    private float scrollPos = 0;
    private float[] pos;

    void Update()
    {
        pos = new float[transform.childCount];

        float distance = 1f / (pos.Length - 1);

        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }

        if (Input.GetMouseButton(0))
        {
            scrollPos = scrollBar.value;
        }
        else
        {
            for (int i = 0; i < pos.Length;i++)
            {
                if (scrollPos < pos[i] + (distance / 2) && scrollPos > pos[i] - (distance / 2))
                {
                    scrollBar.value = Mathf.Lerp(scrollBar.value, pos[i], 0.1f);
                }
            }
        }

        for (int i = 0; i < pos.Length; i++) 
        {
            if (scrollPos < pos[i] + (distance / 2) && scrollPos > pos[i] - (distance / 2))
            {
                Transform childTransform = transform.GetChild(i);
                childTransform.localScale = Vector2.Lerp(childTransform.localScale, new Vector2(1f, 1f), 0.1f);

                for (int j = 0; j < pos.Length; j++)
                {
                    childTransform = transform.GetChild(j);

                    childTransform.localScale = Vector2.Lerp(childTransform.localScale, new Vector2(0.8f, 0.8f), 0.1f);
                }
            }
        }

        Debug.Log(scrollBar.value);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            scrollBar.value -= 0.1f;
            scrollPos = scrollBar.value;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            scrollBar.value += 0.1f;
            scrollPos = scrollBar.value;
        }
    }
}
