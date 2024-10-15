using UnityEngine;

public class TestScript : MonoBehaviour
{

    public void DebugMessage(string message)
    {
        Debug.Log($"{gameObject.name}: {message}");
    }

    public static void DebugMessageStatic(string message)
    {
        Debug.Log(message);
    }
}
