using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private Currency currency;
    [SerializeField] private Image imageIcon;
    [SerializeField] private TMP_Text textValue;

    public void Setup()
    {
        Refresh();
    }
    
    void OnCurrencyAmountChanged(Currency parCurrency, int parPrevious, int parUpdated)
    {
        Refresh();
    }
    
    void OnEnable()
    {
        Player.Instance.Wallet.OnCurrencyAmountChanged += OnCurrencyAmountChanged;
        Player.Instance.Wallet.OnLoad += OnLoad;
        imageIcon.sprite = currency.Sprite;
        textValue.color = currency.Color;
        Refresh();
    }

    void OnDisable()
    {
        Player.Instance.Wallet.OnCurrencyAmountChanged -= OnCurrencyAmountChanged;
        Player.Instance.Wallet.OnLoad -= OnLoad;
    }

    void OnLoad()
    {
        Refresh();
    }

    void Refresh()
    {
        textValue.text = Player.Instance.Wallet.GetAmount(currency).ToString();
    }
}
