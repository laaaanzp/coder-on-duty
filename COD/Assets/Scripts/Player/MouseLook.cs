using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public static float sensitivity;
    [SerializeField] private Transform playerBody;

    private float xRotation = 0f;


    void Awake()
    {
        sensitivity = PlayerPrefs.GetFloat("mouse-sensitivity", 100f);
        QualitySettings.vSyncCount = 1;
    }

    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked) 
            return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector2.up * mouseX);
    }
}
