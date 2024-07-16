using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject Store;
    [SerializeField]  TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI storeMoneyText;

    public void UpdatePlayerMoneyText(int money)
    {
        moneyText.text = "Cash: " + money.ToString();
        storeMoneyText.text = "Cash: " + money.ToString();
    }
}
