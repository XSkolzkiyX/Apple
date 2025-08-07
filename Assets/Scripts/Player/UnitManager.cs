using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviour
{
    public NavMeshAgent unit;

    private Camera mainCamera;
    private Vector3 selectedDestination;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            PointDestination();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GoTo();
        }
    }

    private void PointDestination()
    {
        if(Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            selectedDestination = hit.point;
        }
    }

    private void GoTo()
    {
        if (selectedDestination == null) return;
        unit.SetDestination(selectedDestination);
    }
}
