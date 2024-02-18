using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower of Colors/Currency")]
public class Currency : ScriptableObject
{
    [SerializeField] private int saveId;
    [SerializeField] private string currencyName;
    [SerializeField] private Color color;
    [SerializeField] private Sprite sprite;
    
    public Color Color => color;
    
    public string CurrencyName => currencyName;
    
    public int SaveId => saveId;
    
    public Sprite Sprite => sprite;
}
