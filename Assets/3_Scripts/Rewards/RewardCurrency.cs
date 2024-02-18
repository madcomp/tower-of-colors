using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardCurrency : Reward
{
    private CurrencyAmount _currencyAmount;
    
    public RewardCurrency(Currency currency, int amount, bool collected) : base(collected)
    {
        _currencyAmount = new CurrencyAmount(currency, amount);
    }
    
    public RewardCurrency(CurrencyAmount currencyAmount, bool collected) : base(collected)
    {
        _currencyAmount = currencyAmount;
    }

    public override void Collect()
    {
        if (_collected)
        {
            return;
        }
        _collected = true;
        Player.Instance.Wallet.Add(_currencyAmount);
        OnCollect?.Invoke();
    }

    public override void SetupUI(Image image, TMP_Text text)
    {
        image.sprite = _currencyAmount.Currency.Sprite;
        text.text = _currencyAmount.amount.ToString();
    }

    public override string ToData()
    {
        return $"{_currencyAmount.Currency.SaveId}|{_currencyAmount.amount}|{(_collected ? 1 : 0)}";
    }
}
