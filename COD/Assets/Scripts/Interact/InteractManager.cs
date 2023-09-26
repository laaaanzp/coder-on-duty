using UnityEngine;

public class InteractManager : MonoBehaviour
{
    [SerializeField] private InteractOverlayControl overlayControl;
    [SerializeField] private Camera cameraSource;
    [SerializeField] private float interactRange = 1.5f;
    [SerializeField] private KeyCode keybind;
    [SerializeField] private LayerMask interactableLayerMask;


    void Update()
    {
        Ray ray = new Ray(cameraSource.transform.position, cameraSource.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactRange, interactableLayerMask))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out Interactable interactable))
            {
                if (!interactable.isInteractable)
                {
                    overlayControl.Hide();
                    return;
                }

                overlayControl.Show(interactable);
                
                if (Input.GetKeyDown(interactable.keybind) && ModalObserver.activeModalCount == 0)
                {
                    interactable.Interact();
                    DebugConsole.Log("Interacted with " + interactable.name);
                }
            }
            else
            {
                overlayControl.Hide();
            }
        }
        else
        {
            overlayControl.Hide();
        }
    }
}
