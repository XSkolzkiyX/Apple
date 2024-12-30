using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HackAbility : MonoBehaviour
{
    [Header("Stats")]
    public float hackDistance;
    public int score;
    public float scoreRegenerationDelay;
    [SerializeField] private LayerMask hackLayer;
    [SerializeField] private Color hackColor;
    private int maxScore;
    [Space(10)]

    [Header("UI")]
    [SerializeField] private GameObject hackScreen;
    [SerializeField] private GameObject hackPanel;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Slider scoreSlider;
    [SerializeField] private Vector2 hackPanelOffset;
    [Space(10)]

    [SerializeField] private InputSettings controls;

    [HideInInspector] public bool lockAbility = false;

    private Camera mainCamera;
    private HackableObject interactionObject;
    private Outline interactionOutline;

    private void Start()
    {
        mainCamera = Camera.main;
        maxScore = score;
        scoreSlider.maxValue = maxScore;
        scoreText.text = score.ToString();
        StartCoroutine(ScoreRegeneration());
    }

    private void Update()
    {
        if (GameManager.Instance.IsPaused) return;

        if (Input.GetKeyDown(controls.hackKey))
        {
            if (hackScreen.activeSelf) DeactivateHackAbility();
            else ActivateHackAbility();
        }

        if (Input.GetMouseButtonDown(0) && interactionObject) HackObject();

        if (!hackScreen.activeSelf) return;
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition),
            out RaycastHit hit, hackDistance, hackLayer) && hit.transform.TryGetComponent(out HackableObject hackableObject))
        {
            if (interactionObject && interactionObject.TryGetComponent(out interactionOutline))
            {
                interactionOutline.enabled = false;
                interactionOutline.OutlineColor = Color.yellow;
            }
            interactionObject = hackableObject;
            if (interactionObject.TryGetComponent(out interactionOutline))
            {
                interactionOutline.enabled = true;
                interactionOutline.OutlineColor = hackColor;
            }
            priceText.text = interactionObject.price.ToString();
            hackPanel.SetActive(true);
        }
        else if (interactionObject)
        {
            if (interactionObject.TryGetComponent(out interactionOutline))
            {
                interactionOutline.enabled = false;
                interactionOutline.OutlineColor = Color.yellow;
            }
            interactionObject = null;
            hackPanel.SetActive(false);
        }

        if (interactionObject)
        {
            hackPanel.transform.position = mainCamera.WorldToScreenPoint(interactionObject.transform.position) + (Vector3)hackPanelOffset;
        }
    }

    public void HackObject()
    {
        if (interactionObject.price > score) return;
        if (interactionObject.TryGetComponent(out ButtonController button)) button.PressButton();
        else if (interactionObject.TryGetComponent(out DoorController door) && !door.locked) door.ChangeDoorState();
        else if (interactionObject.TryGetComponent(out BarrelController barrel)) barrel.StartCoroutine(barrel.Explode());
        else return;
        score -= interactionObject.price;
        scoreSlider.value = score;
        scoreText.text = score.ToString();
        hackPanel.SetActive(false);
    }

    public void ActivateHackAbility()
    {
        hackScreen.SetActive(true);
        //Time.timeScale = 0f;
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
    }

    public void DeactivateHackAbility()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        //Time.timeScale = 1f;
        hackScreen.SetActive(false);
        if (interactionObject && interactionObject.TryGetComponent(out interactionOutline))
        {
            interactionOutline.enabled = false;
            interactionOutline.OutlineColor = Color.yellow;
        }
        interactionObject = null;
        hackPanel.SetActive(false);
    }

    private IEnumerator ScoreRegeneration()
    {
        yield return new WaitForSeconds(scoreRegenerationDelay);
        score = Mathf.Clamp(score + 1, 0, maxScore);
        scoreSlider.value = score;
        scoreText.text = score.ToString();
        StartCoroutine(ScoreRegeneration());
    }
}
