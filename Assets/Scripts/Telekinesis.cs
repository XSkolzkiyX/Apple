using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    class GrabObject
    {
        public Rigidbody rigidbody;
        public float drag;
        public float angularDrag;
    }

    [Header("Grab Stats")]
    [SerializeField] private float grabDistance = 2.5f;
    [SerializeField] private float grabSpeed = 12f;
    [SerializeField] private float grabDelta = .5f;

    private GrabObject grabbedObject = new GrabObject();
    private Transform mainCamera;

    private void Start()
    {
        mainCamera = Camera.main.transform;
    }

    private void Update()
    {
        if (Time.timeScale <= 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            foreach(RaycastHit hit in Physics.RaycastAll(mainCamera.position, mainCamera.forward, grabDistance))
            {
                if(!hit.transform.TryGetComponent(out FirstPersonController _) && hit.collider.TryGetComponent(out grabbedObject.rigidbody))
                {
                    grabbedObject.drag = grabbedObject.rigidbody.drag;
                    grabbedObject.angularDrag = grabbedObject.rigidbody.angularDrag;
                    grabbedObject.rigidbody.isKinematic = false;
                    grabbedObject.rigidbody.useGravity = false;
                    grabbedObject.rigidbody.drag = 10f;
                    grabbedObject.rigidbody.angularDrag = 1f;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0) && grabbedObject.rigidbody) DeselectGrabObject();
    }

    private void FixedUpdate()
    {
        if (grabbedObject.rigidbody)
        {
            Vector3 targetPosition = mainCamera.position + (mainCamera.forward * grabDistance);
            Vector3 direction = targetPosition - grabbedObject.rigidbody.transform.position;
            float distance = Vector3.Distance(targetPosition, grabbedObject.rigidbody.transform.position);
            if (distance > grabDistance) DeselectGrabObject();
            else if (distance > grabDelta) grabbedObject.rigidbody.velocity = direction.normalized * grabSpeed;
        }
    }

    private void DeselectGrabObject()
    {
        grabbedObject.rigidbody.useGravity = true;
        grabbedObject.rigidbody.drag = grabbedObject.drag;
        grabbedObject.rigidbody.angularDrag = grabbedObject.angularDrag;
        grabbedObject.rigidbody = null;
    }
}