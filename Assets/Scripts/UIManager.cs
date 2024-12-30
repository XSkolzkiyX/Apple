using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public static Vector2 referenceResolution = new Vector2(1920, 1080);

    [Header("Crosshair Settings")]
    public RectTransform crossHair;
    public RectTransform alternateCrossHair;

    [Header("Ammo Display Settings")]
    public TextMeshProUGUI ammoText;

    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateAmmoText(int ammoInMag, int ammo)
    {
        ammoText.text = $"{ammoInMag} / {ammo}";
    }

    public void SetCrossHairActive(bool active, int ammoInMag = -1, int ammo = -1)
    {
        crossHair.gameObject.SetActive(active);
        alternateCrossHair.gameObject.SetActive(!active);

        if (ammoInMag >= 0 || ammo >= 0)
        {
            ammoText.color = Color.white;
            UpdateAmmoText(ammoInMag, ammo);
            alternateCrossHair.gameObject.SetActive(active);
        }
        else
        {
            ammoText.color = Color.gray;
            ammoText.text = "...";
            alternateCrossHair.gameObject.SetActive(!active);
        }
    }

    public void UpdateCrossHairSize(float shootingSpread)
    {
        float crossHairSize = ((referenceResolution.y / 100.0f) * shootingSpread) * 2;
        crossHair.sizeDelta = new Vector2(crossHairSize, crossHairSize);
    }
}
