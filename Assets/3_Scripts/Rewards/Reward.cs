using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Reward
{
    public static Reward ParseData(Context context, string data)
    {
        var parts = data.Split("|");
        if (parts.Length < 2)
        {
            Debug.LogError("Corrupted reward data");
        }
        if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int saveId))
        {
            Debug.LogError("Corrupted reward id");
            return null;
        }
        if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int amount))
        {
            Debug.LogError("Corrupted reward amount");
            return null;
        }
        var currency = context.GetCurrency(saveId);
        return new RewardCurrency(currency, amount);
    }
    
    public abstract void Consume(Player player);
    public abstract void SetupUI(Image image, TMP_Text text);
    public abstract string ToData();
}
