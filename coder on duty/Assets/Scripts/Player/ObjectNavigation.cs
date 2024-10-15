using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ObjectNavigation : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float lineUpdateSpeed = 0.5f;
    [SerializeField] private float lineHeightOffset = 1.5f;

    public GameObject currentInstance;
    private Coroutine drawLineCoroutine;

    public static ObjectNavigation instance;


    void Awake()
    {
        instance = this;
    }

    public static void Navigate(GameObject obj)
    {
        instance._Navigate(obj);
    }

    public static void StopNavigate()
    {
        instance._StopNavigate();
    }

    public void _Navigate(GameObject obj)
    {
        currentInstance = obj;

        if (drawLineCoroutine != null)
        {
            StopCoroutine(drawLineCoroutine);
        }

        drawLineCoroutine = StartCoroutine(DrawLineToGameObject());
    }

    public void _StopNavigate()
    {
        currentInstance = null;
        lineRenderer.enabled = false;

        if (drawLineCoroutine != null)
        {
            StopCoroutine(drawLineCoroutine);
        }
    }

    private IEnumerator DrawLineToGameObject()
    {
        WaitForSeconds wait = new WaitForSeconds(lineUpdateSpeed);
        
        NavMeshPath path = new NavMeshPath();

        while (currentInstance != null)
        {
            if (NavMesh.CalculatePath(transform.position, currentInstance.transform.position, NavMesh.AllAreas, path))
            {
                lineRenderer.enabled = true;
                lineRenderer.positionCount = path.corners.Length;

                for (int i = 0; i  < path.corners.Length; i++)
                {
                    lineRenderer.SetPosition(i, path.corners[i] + Vector3.up * lineHeightOffset);
                }
            }
            else
            {
                lineRenderer.enabled = false;
                // Debug.LogError("Unable to calculate path");
            }

            yield return wait;
        }
    }
}
