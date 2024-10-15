using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GlobalsSingleton : MonoBehaviour
{
    private static GlobalsSingleton instance;
    public UnityEvent init;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        init?.Invoke();
    }
}
