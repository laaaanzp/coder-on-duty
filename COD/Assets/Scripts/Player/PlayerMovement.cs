using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController characterController;

    [Header("Normal Speed")]
    [SerializeField] public float normalSpeed = 4f;
    private float normalSpeedFOV;

    [Header("Sprint Speed")]
    [SerializeField] public float sprintSpeed = 6f;
    [SerializeField] public float sprintSpeedFOV = 75f;

    [Header("Gravity")]
    [SerializeField] public bool applyGravity = true;
    [SerializeField] public float gravity = -9.81f;
    private Vector3 velocity;

    private float currentSpeed;


    void Awake()
    {
        currentSpeed = normalSpeed;
        normalSpeedFOV = Camera.main.fieldOfView;
    }

    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked) 
            return;

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * y;

        characterController.Move(move * currentSpeed * Time.deltaTime);

        if (applyGravity)
        {
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Sprint();
        }
        else
        {
            Walk();
        }
    }

    void Sprint()
    {
        currentSpeed = sprintSpeed;

        if (Camera.main.fieldOfView < sprintSpeedFOV)
        {
            Camera.main.fieldOfView += 100f * Time.deltaTime;
        }
        else if (Camera.main.fieldOfView > sprintSpeedFOV)
        {
            Camera.main.fieldOfView = sprintSpeedFOV;
        }
    }

    void Walk()
    {
        currentSpeed = normalSpeed;

        if (Camera.main.fieldOfView > normalSpeedFOV)
        {
            Camera.main.fieldOfView -= 100f * Time.deltaTime;
        }
        else if (Camera.main.fieldOfView < normalSpeedFOV)
        {
            Camera.main.fieldOfView = normalSpeedFOV;
        }
    }
}
