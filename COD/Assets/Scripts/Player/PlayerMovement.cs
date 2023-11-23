using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;

    [SerializeField] private float footstepDelay = 0.75f;
    private float currentDelayTime = 0f;

    [Header("Normal Speed")]
    [SerializeField] private float normalSpeed = 4f;
    private float normalSpeedFOV;

    [Header("Sprint Speed")]
    [SerializeField] private float sprintSpeed = 5f;
    [SerializeField] private float sprintSpeedFOV = 75f;

    [Header("Gravity")]
    [SerializeField] private bool applyGravity = true;
    [SerializeField] private float gravity = -9.81f;
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

        if (y != 0 || x != 0)
        {
            currentDelayTime += Time.deltaTime;
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
        if (currentDelayTime >= footstepDelay / 2)
        {
            AudioController.PlayFootstep();
            currentDelayTime = 0f;
        }

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
        if (currentDelayTime >= footstepDelay)
        {
            AudioController.PlayFootstep();
            currentDelayTime = 0f;
        }

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
