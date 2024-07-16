using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Singleton game manager
/// </summary>
public class GameManager : MonoBehaviour
{
    static GameManager instance = null;
    public static GameManager Instance { get {return instance; }}

    public int Money { get => money; set => money = value; }
    public PlayerController Player { get => player; set => player = value; }

    [SerializeField] private PlayerController player;
    [SerializeField] private UIManager uIManager;

    private int money = 0;
    
    void Start()
    {
        instance = this;
        uIManager.UpdatePlayerMoneyText(this.Money);
    }

    public void AddPlayerMoney(int money)
    {
        this.Money += money;
        uIManager.UpdatePlayerMoneyText(this.Money);
    }

    public void RemovePlayerMoney(int money)
    {
        this.Money -= money;
        uIManager.UpdatePlayerMoneyText(this.Money);
    }

    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Unpause()
    {
        Time.timeScale = 1;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BotBody"))
        {
            AddPlayerMoney(10);
            Destroy(other.transform.GetComponentInParent<BotController>().gameObject);
        }
    }
}
