using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardCurrency : Reward
{
    private CurrencyAmount _currencyAmount;
    
    public RewardCurrency(Currency currency, int amount)
    {
        _currencyAmount = new CurrencyAmount(currency, amount);
    }
    
    public RewardCurrency(CurrencyAmount currencyAmount)
    {
        _currencyAmount = currencyAmount;
    }

    public override void Consume(Player player)
    {
        player.Wallet.Add(_currencyAmount);
    }

    public override void SetupUI(Image image, TMP_Text text)
    {
        image.sprite = _currencyAmount.Currency.Sprite;
        text.text = _currencyAmount.amount.ToString();
    }

    public override string ToData()
    {
        return $"{_currencyAmount.Currency.SaveId}|{_currencyAmount}";
    }
}
