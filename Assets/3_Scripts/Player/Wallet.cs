using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Wallet
{
    private Dictionary<Currency, int> _amountByCurrency;

    public System.Action<Currency, int, int> OnCurrencyAmountChanged;
    public System.Action OnLoad;

    public Wallet()
    {
        _amountByCurrency = new Dictionary<Currency, int>();
    }
    
    public void Add(CurrencyAmount currencyAmount)
    {
        var currency = currencyAmount.Currency;
        var updatedValue = currencyAmount.amount;
        
        if (_amountByCurrency.TryGetValue(currency, out var previous))
        {
            updatedValue += previous;
        }
        _amountByCurrency[currency] = updatedValue;

        SaveData.WalletData = ToData();
        
        OnCurrencyAmountChanged?.Invoke(currency, previous, updatedValue);
        
        SaveData.WalletData = Player.Instance.Wallet.ToData();
    }

    public int GetAmount(Currency currency)
    {
        return _amountByCurrency.GetValueOrDefault(currency, 0);
    }

    public void Load(Context context)
    {
        var walletData = SaveData.WalletData;
        var parts = walletData.Split(",");
        foreach (var part in parts)
        {
            var entryParts = part.Split("|");
            if (entryParts.Length != 2)
            {
                continue;
            }
            if (!int.TryParse(entryParts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int currencyId))
            {
                continue;
            }
            if (!int.TryParse(entryParts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int amount))
            {
                continue;
            }

            var currency = context.GetCurrency(currencyId);
            _amountByCurrency[currency] = amount;
        }
        OnLoad?.Invoke();
    }

    public string ToData()
    {
        var data = new List<string>();
        foreach (var entry in _amountByCurrency)
        {
            data.Add($"{entry.Key.SaveId}|{entry.Value}");
        }
        return string.Join(",", data);
    }
}
