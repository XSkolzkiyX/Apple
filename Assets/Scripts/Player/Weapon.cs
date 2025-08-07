using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody weaponRigidbody;
    [HideInInspector] public Collider weaponCollider;
    [HideInInspector] public Outline weaponOutline;
    [HideInInspector] public FirstPersonController player;
    [HideInInspector] public Transform mainCamera;

    public virtual void Start()
    {
        animator = GetComponent<Animator>();
        weaponRigidbody = GetComponent<Rigidbody>();
        weaponCollider = GetComponent<Collider>();
        weaponOutline = GetComponent<Outline>();
        mainCamera = Camera.main.transform;
    }
}
