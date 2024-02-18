using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower of Colors/Mission Objective Settings")]
public class MissionObjectiveSettings : ScriptableObject
{
    [SerializeField] private Mission.Objective objective;
    [SerializeField] private string description;
    [SerializeField] private AnimationCurve xPerLevelOnEasy;
    [SerializeField] private AnimationCurve xPerLevelOnMedium;
    [SerializeField] private AnimationCurve xPerLevelOnHard;
    [SerializeField] private AnimationCurve yPerLevelOnEasy;
    [SerializeField] private AnimationCurve yPerLevelOnMedium;
    [SerializeField] private AnimationCurve yPerLevelOnHard;
    
    public Mission.Objective Objective => objective;
    
    public string Description => description;

    public AnimationCurve XPerLevel(Mission.Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Mission.Difficulty.Easy: return xPerLevelOnEasy;
            case Mission.Difficulty.Medium: return xPerLevelOnMedium;
            case Mission.Difficulty.Hard: return xPerLevelOnHard;
        }
        return xPerLevelOnMedium;
    }
    public AnimationCurve XPerLevelOnEasy => xPerLevelOnEasy;
    public AnimationCurve XPerLevelOnMedium => xPerLevelOnMedium;
    public AnimationCurve XPerLevelOnHard => xPerLevelOnHard;
    
    public AnimationCurve YPerLevel(Mission.Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Mission.Difficulty.Easy: return yPerLevelOnEasy;
            case Mission.Difficulty.Medium: return yPerLevelOnMedium;
            case Mission.Difficulty.Hard: return yPerLevelOnHard;
        }
        return yPerLevelOnMedium;
    }
    public AnimationCurve YPerLevelOnEasy => yPerLevelOnEasy;
    public AnimationCurve YPerLevelOnMedium => yPerLevelOnMedium;
    public AnimationCurve YPerLevelOnHard => yPerLevelOnHard;
}
