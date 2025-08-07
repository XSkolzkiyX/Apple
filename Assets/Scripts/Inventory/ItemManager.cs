using System.Collections;
using TMPro;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [Header("Items")]
    public int healPotion;
    public int regenerationPotion;
    public int speedPotion;
    public int strengthPotion;
    [Space(10)]

    [Header("UI")]
    [SerializeField] private GameObject itemMenu;
    [SerializeField] private TextMeshProUGUI healText;
    [SerializeField] private TextMeshProUGUI regenerationText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI strengthText;

    private bool itemMenuActive = false;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab)) ToggleItemMenu();

        if (!itemMenuActive) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) UsePotion("Heal");
        if (Input.GetKeyDown(KeyCode.Alpha2)) UsePotion("Regeneration");
        if (Input.GetKeyDown(KeyCode.Alpha3)) UsePotion("Speed");
        if (Input.GetKeyDown(KeyCode.Alpha4)) UsePotion("Strength");
    }

    public void UsePotion(string potionName)
    {
        switch(potionName)
        {
            case "Heal" when healPotion > 0:
                healPotion--;
                Debug.Log("+10 Health");
                break;
            case "Regeneration" when regenerationPotion > 0:
                regenerationPotion--;
                StartCoroutine(Regenete());
                break;
            case "Speed" when speedPotion > 0:
                speedPotion--;
                Debug.Log("+1 Speed");
                break;
            case "Strength" when strengthPotion > 0:
                strengthPotion--;
                Debug.Log("+10 Strength");
                break;
        }
        FillItemTexts();
        ToggleItemMenu();
    }

    public void FillItemTexts()
    {
        healText.text = $"Heal Potion : {healPotion}";
        regenerationText.text = $"Regen Potion : {regenerationPotion}";
        speedText.text = $"Speed Potion : {speedPotion}";
        strengthText.text = $"Strength Potion : {strengthPotion}";
    }

    private void ToggleItemMenu()
    {
        itemMenuActive = !itemMenuActive;
        itemMenu.SetActive(itemMenuActive);
        Cursor.visible = itemMenuActive;
        Cursor.lockState = itemMenuActive ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = itemMenuActive ? 0 : 1;
    }

    private IEnumerator Regenete()
    {
        Debug.Log("Regeneration started");
        yield return new WaitForSeconds(3f);
        Debug.Log("Regeneration finished");
    }
}
