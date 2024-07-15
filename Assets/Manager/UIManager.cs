using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject Store;
    //[SerializeField]  money;

    private int playerMoney = 0;

    public void AddPlayerMoney(int money)
    {
        playerMoney += money;
    }
}
