using UnityEngine;

public class SpectatorCamera : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float speedH;
    [SerializeField] private float speedV;

    private float yaw, pitch;
    private static SpectatorCamera instance;

    void Awake()
    {
        instance = this;
    }

    public static bool SetSpeed(float speed)
    {
        try
        {
            instance.movementSpeed = speed;
            return true;
        }
        catch
        {
            return false;
        }
    }

    void Update()
    {
        Cursor.lockState =  ModalObserver.activeModalCount == 0 ? CursorLockMode.Locked : CursorLockMode.None;

        if (Cursor.lockState != CursorLockMode.Locked) 
            return;

        float h = Input.GetAxis("Horizontal") * Time.deltaTime * movementSpeed * (Input.GetKey(KeyCode.LeftShift) ? 2 : 1);
        float v = Input.GetAxis("Vertical") * Time.deltaTime * movementSpeed * (Input.GetKey(KeyCode.LeftShift) ? 2 : 1); ;

        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.eulerAngles = new Vector3(pitch, yaw, 0);
        transform.position += transform.TransformDirection(Vector3.forward) * v + transform.TransformDirection(Vector3.right) * h;
    }
}
