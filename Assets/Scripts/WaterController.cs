using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class RigidbodyProperties
{
    public Rigidbody body;
    public float mass;
    public float drag;
    public float angularDrag;
}

public class WaterController : MonoBehaviour
{
    public float waterDrag;
    public float waterPushingForce;
    public List<RigidbodyProperties> a = new List<RigidbodyProperties>();
    public SerializedDictionary<Rigidbody, float> rigidbodies = new SerializedDictionary<Rigidbody, float>();

    private void FixedUpdate()
    {
        if (rigidbodies.Count <= 0) return;
        foreach(Rigidbody rb in rigidbodies.Keys)
        {
            rb.AddForce(Vector3.up * waterPushingForce);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out Rigidbody rb))
        {
            rigidbodies.Add(rb, rb.drag);
            rb.drag = waterDrag;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.TryGetComponent(out Rigidbody rb))
        {
            rb.drag = rigidbodies[rb];
            rigidbodies.Remove(rb);
        }
    }
}
