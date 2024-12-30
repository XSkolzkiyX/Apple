using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public InputSettings inputSettings;
    public LayerMask interactionLayer;
    public float interactionDistance = 3f;
    public float throwingForce = 15f;

    private Camera mainCamera;
    private GameObject interactionObject;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        DetectInteractable();
        HandleInteractionInput();
    }

    private void DetectInteractable()
    {
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward,
            out RaycastHit hit, interactionDistance, interactionLayer))
        {
            Debug.DrawLine(mainCamera.transform.position, hit.point, Color.red, 0.1f);

            if (!hit.transform.gameObject.Equals(interactionObject))
            {
                ClearCurrentInteractionObject();
                interactionObject = hit.transform.gameObject;
                EnableOutline(interactionObject, true);
            }
        }
        else
        {
            ClearCurrentInteractionObject();
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(inputSettings.interactionKey))
        {
            Interact();
        }
    }

    private void Interact()
    {
        if (!interactionObject) return;

        if (interactionObject.TryGetComponent(out WeaponController weapon))
        {
            FindObjectOfType<WeaponManager>()?.EquipWeapon(weapon);
        }
        else if (interactionObject.TryGetComponent(out DoorController door))
        {
            door.ChangeDoorState();
        }
        else if (interactionObject.TryGetComponent(out ButtonController button))
        {
            button.PressButton();
        }
        else if (interactionObject.TryGetComponent(out BarrelController barrel))
        {
            StartCoroutine(barrel.Explode(3));
        }
    }

    private void ClearCurrentInteractionObject()
    {
        if (!interactionObject) return;
        EnableOutline(interactionObject, false);
        interactionObject = null;
    }

    private void EnableOutline(GameObject obj, bool enable)
    {
        if (obj.TryGetComponent(out Outline outline))
        {
            outline.enabled = enable;
        }
    }
}
