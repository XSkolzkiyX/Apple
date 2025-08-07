using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonController : MonoBehaviour
{
    public UnityEvent buttonEvent;
    [HideInInspector] public Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PressButton()
    {
        animator.SetTrigger("Press");
        buttonEvent.Invoke();
    }
}
