using UnityEngine;

public class CodeCompilerControl : MonoBehaviour
{
    public ModalControl codeCompilerModal;

    private static ModalControl instance;


    void Awake()
    {
        if (instance != null)
            return;

        instance = codeCompilerModal;
    }

    public static void Open()
    {
        instance.Open();
    }
}
