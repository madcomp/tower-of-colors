using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower of Colors/Currency")]
public class Currency : ScriptableObject
{
    [SerializeField] private int saveId;
    [SerializeField] private string currencyName;
    [SerializeField] private Sprite sprite;
    
    public string CurrencyName => currencyName;
    
    public int SaveId => saveId;
    
    public Sprite Sprite => sprite;
}
