using UnityEngine;

public enum ShopType
{
    Coin,
    Speed,
    DoubleJump,
    Health
}

public class ShopController : MonoBehaviour
{
    public ShopType shopType;
    public int price;

    public void Buy(PlayerController player)
    {
        if (player.coins < price) return;
        switch(shopType)
        {
            case ShopType.Coin:
                player.coins += player.apples;
                player.apples = 0;
                break;
            case ShopType.Speed:
                player.runningSpeed *= 2;
                player.walkingSpeed *= 2;
                player.acceleration *= 2;
                break;
            case ShopType.DoubleJump:
                player.maxCountOfJumps++;
                player.countOfJumps = player.maxCountOfJumps;
                break;
            case ShopType.Health:
                player.maxHealth += 20;
                player.health = player.maxHealth;
                break;
        }
        player.coins -= price;
    }
}