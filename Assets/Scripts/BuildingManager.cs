using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public bool inBuildingMode = false;
    
    public GameObject buildingPrefab;
    public GameObject ghostBuildingPrefab;

    [SerializeField] private float placingRange = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private LayerMask placingLayer;

    private Transform mainCamera;
    private GameObject ghostBuilding;

    private void Start()
    {
        mainCamera = Camera.main.transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (inBuildingMode)
                ExitBuildingMode();
            else inBuildingMode = true;
        }

        if (!inBuildingMode) return;

        if (Input.GetKeyDown(KeyCode.Mouse0)) PlaceBuilding();
        if (Input.GetKey(KeyCode.Q)) RotateBuilding(-1);
        if (Input.GetKey(KeyCode.E)) RotateBuilding();
    }

    private void FixedUpdate()
    {
        if (!inBuildingMode) return;

        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out RaycastHit hit, placingRange, placingLayer))
        {
            if(ghostBuilding)
            {
                ghostBuilding.transform.position = hit.point;
            }
            else ghostBuilding = Instantiate(ghostBuildingPrefab, hit.point, transform.rotation);
        }
        else
        {
            Destroy(ghostBuilding);
        }
    }

    private void RotateBuilding(int direction = 1)
    {
        ghostBuilding.transform.Rotate(0, direction * rotationSpeed, 0);
    }

    private void PlaceBuilding()
    {
        Instantiate(buildingPrefab, ghostBuilding.transform.position, ghostBuilding.transform.rotation);
        Destroy(ghostBuilding);
        inBuildingMode = false;
    }

    private void ExitBuildingMode()
    {
        if (ghostBuilding) Destroy(ghostBuilding);
        inBuildingMode = false;
    }
}
