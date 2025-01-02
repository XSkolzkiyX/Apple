using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlot
{
    public Image iconImage;
    public Image backgroundImage;
    public GameObject selectionOutline;
    public Slider durabilitySlider;
}

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory")]
    public List<ItemData> inventoryItems;
    [SerializeField] private Transform itemPlace;
    [SerializeField] private float throwForce;
    [SerializeField] private float interactionDistance;
    [SerializeField] private LayerMask interactionLayer;
    [Space(10)]

    [Header("UI")]
    [SerializeField] private List<InventorySlot> inventorySlots;
    [Space(10)]

    [Header("Controls")]
    public KeyCode interactionKey;
    public KeyCode alternateInteractionKey;
    public KeyCode pickUpKey;
    public KeyCode dropKey;
    [Space(10)]


    private int activeSlotIndex = 0;
    private ItemController interactionObject;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        SelectItem(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(pickUpKey)) PickUpItem();
        if (Input.GetKeyDown(dropKey)) DropItem();
        if(Input.inputString.Length > 0 && 
            byte.TryParse(Input.inputString[0].ToString(), out byte input) &&
            input - 1 < inventoryItems.Count && input - 1 >= 0)
        {
            SelectItem(input - 1);
        }
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward,
            out RaycastHit hit, interactionDistance, interactionLayer))
        {
            Debug.DrawLine(mainCamera.transform.position, hit.point, Color.red, 1f);
            if (hit.transform.TryGetComponent(out ItemController potentialItem) && !potentialItem.Equals(interactionObject))
            {
                if (interactionObject) interactionObject.outline.enabled = false;
                interactionObject = potentialItem;
                interactionObject.outline.enabled = true;
            }
        }
        else if (interactionObject)
        {
            interactionObject.outline.enabled = false;
            interactionObject = null;
        }
    }

    private void SelectItem(int newIndex)
    {
        inventorySlots[activeSlotIndex].selectionOutline.SetActive(false);
        if (itemPlace.childCount > 0) Destroy(itemPlace.GetChild(0).gameObject);
        activeSlotIndex = newIndex;
        inventorySlots[activeSlotIndex].selectionOutline.SetActive(true);
        if (inventoryItems[activeSlotIndex]) Instantiate(inventoryItems[activeSlotIndex].model, itemPlace.position, itemPlace.rotation, itemPlace);
    }

    private void PickUpItem()
    {
        if (!interactionObject) return;
        for(int i = 0; i < inventoryItems.Count; i++)
        {
            if (!inventoryItems[i])
            {
                inventoryItems[i] = interactionObject.itemData;
                inventorySlots[i].iconImage.sprite = interactionObject.itemData.icon;
                inventorySlots[i].durabilitySlider.gameObject.SetActive(interactionObject.itemData.durability > -1);
                if(i == activeSlotIndex) SelectItem(i);
                Destroy(interactionObject.gameObject);
                return;
            }
        }
        Debug.Log("No empty space");
    }

    private void DropItem()
    {
        if (!inventoryItems[activeSlotIndex]) return;
        GameObject item = Instantiate(inventoryItems[activeSlotIndex].prefab, itemPlace.position, itemPlace.rotation);
        item.GetComponent<Rigidbody>().velocity = itemPlace.forward * throwForce;
        inventorySlots[activeSlotIndex].iconImage.sprite = null;
        inventorySlots[activeSlotIndex].durabilitySlider.gameObject.SetActive(false);
        if (itemPlace.childCount > 0) Destroy(itemPlace.GetChild(0).gameObject);
        inventoryItems[activeSlotIndex] = null;
    }
}
