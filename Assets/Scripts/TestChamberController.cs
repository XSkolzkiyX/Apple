using TMPro;
using UnityEngine;

[System.Serializable]
public class ChamberControl
{
    public KeyCode primaryKey;
    public KeyCode secondaryKey;
}

public class TestChamberController : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] private GameObject defaultEnemy;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform enemies;
    public int amountOfEnemiesToSpawn = 10;
    [Space(20)]

    [Header("Controls")]
    [SerializeField] private ChamberControl spawnEnemyKeys;
    [SerializeField] private ChamberControl killAllKeys;
    [SerializeField] private ChamberControl cleanUpKeys;
    [SerializeField] private ChamberControl refillAmmoKeys;
    [SerializeField] private ChamberControl healPlayerKeys;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI amountOfEnemiesToSpawnText;

    private FirstPersonController player;

    private void Start()
    {
        player = FindFirstObjectByType<FirstPersonController>();
        amountOfEnemiesToSpawnText.text = $"Enemies to spawn: {amountOfEnemiesToSpawn}";
    }

    private void Update()
    {
        if(Input.GetKey(spawnEnemyKeys.primaryKey))
        {
            if (Input.GetKeyDown(spawnEnemyKeys.secondaryKey))
            {
                SpawnEnemies();
            }
        }

        if (Input.GetKey(killAllKeys.primaryKey))
        {
            if (Input.GetKeyDown(killAllKeys.secondaryKey))
            {
                KillAllEnemies();
            }
        }

        if (Input.GetKey(cleanUpKeys.primaryKey))
        {
            if (Input.GetKeyDown(cleanUpKeys.secondaryKey))
            {
                CleanUp();
            }
        }

        if (Input.GetKey(refillAmmoKeys.primaryKey))
        {
            if (Input.GetKeyDown(refillAmmoKeys.secondaryKey))
            {
                RefillAmmo();
            }
        }

        if (Input.GetKey(healPlayerKeys.primaryKey))
        {
            if (Input.GetKeyDown(healPlayerKeys.secondaryKey))
            {
                HealPlayer();
            }
        }
    }

    public void SpawnEnemies(int indexOfSpawnPoint = 0)
    {
        indexOfSpawnPoint--;
        for (int i = 0; i < amountOfEnemiesToSpawn; i++)
        {
            Instantiate(defaultEnemy, spawnPoints[indexOfSpawnPoint < 0 || indexOfSpawnPoint > spawnPoints.Length ? Random.Range(0, spawnPoints.Length) : indexOfSpawnPoint].position, Quaternion.identity, enemies);
        }
    }

    public void KillAllEnemies()
    {
        foreach(Transform enemy in enemies)
        {
            if(enemy.TryGetComponent(out EnemyController enemyController))
            {
                enemyController.Die();
            }
        }
    }

    public void CleanUp()
    {
        foreach (Transform _object in enemies)
        {
            Destroy(_object.gameObject);
        }
    }

    public void RefillAmmo()
    {
        foreach(WeaponController weapon in FindObjectsByType<WeaponController>(FindObjectsSortMode.None))
        {
            weapon.ammoInMag = weapon.weaponData.ammoInMag;
            weapon.ammo = weapon.weaponData.ammo;
        }
        //player.curWeapon.ammo = player.curWeapon.weaponData.ammo;
        //player.curWeapon.ammoInMag = player.curWeapon.weaponData.ammoInMag;
        //player.playerUI.ammoText.text = $"{player.curWeapon.ammoInMag} / {player.curWeapon.ammo}";
    }

    public void HealPlayer()
    {
        player.health = player.playerStats.health;
    }

    public void ChangeAmountOfEnemiesToSpawn(int dir)
    {
        amountOfEnemiesToSpawn += dir;
        amountOfEnemiesToSpawnText.text = $"Enemies to spawn: {amountOfEnemiesToSpawn}";
    }
}
