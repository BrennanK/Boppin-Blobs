using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SampleInventory : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI CoinText;
    [SerializeField] int CurrentCoins;

    // Start is called before the first frame update
    void Start()
    {
        CoinText.text = "$" + CurrentCoins.ToString();
    }

    // Add money or Lose money
    public void ModifiyCoins(int money)
    {
        CurrentCoins += money;
        CoinText.text = "$" + CurrentCoins.ToString();
    }
}
