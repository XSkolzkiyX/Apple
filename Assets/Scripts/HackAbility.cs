using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackAbility : MonoBehaviour
{
    [Header("UI")]
    public GameObject hackScreen;
    public GameObject hackPanel;

    private FirstPersonController player;
    private Camera mainCamera;

    private void Start()
    {
        player = FindFirstObjectByType<FirstPersonController>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if(Input.GetKeyDown(player.controls.hackKey))
        {
            if(hackScreen.activeSelf) DeactivateHackAbility();
            else ActivateHackAbility();
        }
        if (hackScreen.activeSelf)
        {
            if(Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition)))
            {
                //
            }
        }
    }

    public void ActivateHackAbility()
    {
        hackScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void DeactivateHackAbility()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        hackScreen.SetActive(false);
    }
}
