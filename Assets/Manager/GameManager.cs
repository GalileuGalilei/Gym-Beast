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

    [SerializeField] private PlayerController player;
    [SerializeField] private UIManager uIManager;
    
    void Start()
    {
        instance = this;
    }

    public void AddPlayerMoney(int money)
    {

    }
}
