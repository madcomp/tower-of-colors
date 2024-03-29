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
        if (parts.Length < 3)
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
        if (!int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int collected))
        {
            Debug.LogError("Corrupted reward collect state");
            return null;
        }
        var currency = context.GetCurrency(saveId);
        return new RewardCurrency(currency, amount, collected != 0);
    }

    public System.Action OnCollect;

    protected bool _collected;

    public abstract void Collect();
    public bool IsCollected() => _collected;
    public abstract void SetupUI(Image image, TMP_Text text);
    public abstract string ToData();

    protected Reward(bool collected)
    {
        _collected = collected;
    }
}
