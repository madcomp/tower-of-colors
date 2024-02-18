using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CurrencyAmount
{
    [SerializeField] private Currency currency;
    [SerializeField] public int amount;

    public CurrencyAmount(Currency parCurrency, int parAmount)
    {
        currency = parCurrency;
        amount = parAmount;
    }

    public Currency Currency => currency;
}
