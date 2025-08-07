using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resource : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private Slider healthBarSlider;

    private float maxHealth;

    private void Start()
    {
        maxHealth = health;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBarSlider.value = health / maxHealth;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
