using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private ItemManager itemManager;

    [SerializeField] private float interactionDistance = 3f;

    private Camera mainCamera;
    private GameObject interactionObject;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) Interact();

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward,
            out RaycastHit hit, interactionDistance))
        {
            Debug.DrawLine(mainCamera.transform.position, hit.point, Color.red, 1f);
            if (!hit.transform.gameObject.Equals(interactionObject))
            {
                if (interactionObject && interactionObject.TryGetComponent(out Outline outline)) outline.enabled = false;
                interactionObject = hit.transform.gameObject;
                if(interactionObject.TryGetComponent(out outline)) outline.enabled = true;
            }
        }
        else if (interactionObject)
        {
            if (interactionObject.TryGetComponent(out Outline outline)) outline.enabled = false;
            interactionObject = null;
        }
    }

    private void Interact()
    {
        if (!interactionObject) return;

        if(interactionObject.TryGetComponent(out CollectableItem item))
        {
            switch(item.type)
            {
                case CollectableItemType.HealPotion:
                    itemManager.healPotion++;
                    break;
                case CollectableItemType.RegenerationPotion:
                    itemManager.regenerationPotion++;
                    break;
                case CollectableItemType.SpeedPotion:
                    itemManager.speedPotion++;
                    break;
                case CollectableItemType.StrengthPotion:
                    itemManager.strengthPotion++;
                    break;
                default:
                    Debug.Log("Unable to pick item...");
                    return;
            }
            Destroy(interactionObject);
            interactionObject = null;
            itemManager.FillItemTexts();
        }
    }
}
