using UnityEngine;

public class Store : MonoBehaviour
{
    public static void BuySpeedUpgrade(int cost)
    {
        GameManager manager = GameManager.Instance;
        PlayerController player = manager.Player;

        if (manager.Money >= cost)
        {
            player.UpdateSpeed(1.25f);
            player.ChangePlayerColor();
            manager.RemovePlayerMoney(cost);
        }
        else
        {
            Debug.Log("Not enough money");
        }
    }

    public static void BuyPunchForceUpgrade(int cost)
    {
        GameManager manager = GameManager.Instance;
        PlayerController player = manager.Player;

        if (manager.Money >= cost)
        {
            player.UpdatePunchForce(1.25f);
            player.ChangePlayerColor();
            manager.RemovePlayerMoney(cost);
        }
        else
        {
            Debug.Log("Not enough money");
        }
    }

    public static void BuyStackUpgrade(int cost)
    {
        GameManager manager = GameManager.Instance;
        PlayerController player = manager.Player;

        if (manager.Money >= cost)
        {
            player.UpdateStackLimit(1);
            player.ChangePlayerColor();
            manager.RemovePlayerMoney(cost);
        }
        else
        {
            Debug.Log("Not enough money");
        }
    }

}
