using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DevPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI devText;

    private FirstPersonController player;

    private void Start()
    {
        player = FindFirstObjectByType<FirstPersonController>();
    }

    private void FixedUpdate()
    {
        FillDevText(player.health, player.rb.velocity, player.curWeapon ? player.curWeapon.name : "None", player.interactionObject ? player.interactionObject.name : "None");
    }

    private void FillDevText(float health, Vector3 velocity, string weapon, string interactionObject)
    {
        devText.text = $"DevPanel:\n{health}\n{velocity}\n{weapon}\n{interactionObject}";
    }
}
