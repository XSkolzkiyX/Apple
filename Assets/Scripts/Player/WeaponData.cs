using System;
using UnityEngine;

[Serializable]
public class HorizontalSpray
{
    [Range(0, 10)] public float leftDirection;
    [Range(0, 10)] public float rightDirection;
}

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Data/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Main")]
    public float damage;
    [Range(1, 200)] public int ammoInMag;
    [Range(1, 1000)] public int ammo;
    [Range(1, 32)] public int bulletsPerShot;
    [Range(0, 2000)] public float fireRate;
    [Space(10)]

    [Header("Spread")]
    [Range(0, 45)] public float aimShootingSpread;
    [Range(0, 45)] public float shootingSpread;
    [Range(0, 45)] public float maxShootingSpread;
    [Range(0, 10)] public float shootingSpreadIncreaseValue;
    [Range(0, 10)] public float shootingSpreadDecreaseValue;
    [Space(10)]

    [Header("Spray")]
    [Range(0, 10)] public float verticalSpray;
    public HorizontalSpray horizontalSpray;
    [Space(10)]

    [Header("Reload")]
    [Range(0, 10)] public float reloadingDuration;
    [Space(10)]

    [Header("Bullet")]
    [Range(1, 100)] public float bulletSpeed;
    public GameObject bulletPrefab;
    [Space(10)]

    [Header("Model")]
    public GameObject weaponModel;
}
