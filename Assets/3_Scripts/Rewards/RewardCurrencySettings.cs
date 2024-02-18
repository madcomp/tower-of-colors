using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower of Colors/Reward Currency Settings")]
public class RewardCurrencySettings : ScriptableObject
{
    [SerializeField] private Mission.Difficulty difficulty;
    [SerializeField] private CurrencyAmount[] possibleCurrencyAmounts;
    
    public Mission.Difficulty Difficulty => difficulty;
    
    public CurrencyAmount[] PossibleCurrencyAmounts => possibleCurrencyAmounts;
}
