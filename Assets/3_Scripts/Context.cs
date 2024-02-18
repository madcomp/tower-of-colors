using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Context
{
    private Dictionary<int, Currency> _currencyBySaveId;
    private Dictionary<Mission.Objective, MissionObjectiveSettings> _missionObjectiveSettingsByMissionObjective;
    private Dictionary<Mission.Difficulty, RewardCurrencySettings> _rewardCurrencySettingsByMissionDifficulty;

    public Context(
        Currency[] currencies,
        MissionObjectiveSettings[] missionObjectivesSettings,
        RewardCurrencySettings[] rewardsCurrencySettings)
    {
        _currencyBySaveId = new Dictionary<int, Currency>();
        foreach (var currency in currencies)
        {
            _currencyBySaveId[currency.SaveId] = currency;
        }
        
        _missionObjectiveSettingsByMissionObjective = new Dictionary<Mission.Objective, MissionObjectiveSettings>();
        foreach (var missionObjectiveSettings in missionObjectivesSettings)
        {
            _missionObjectiveSettingsByMissionObjective[missionObjectiveSettings.Objective] = missionObjectiveSettings;
        }
        
        _rewardCurrencySettingsByMissionDifficulty = new Dictionary<Mission.Difficulty, RewardCurrencySettings>();
        foreach (var rewardCurrencySettings in rewardsCurrencySettings)
        {
            _rewardCurrencySettingsByMissionDifficulty[rewardCurrencySettings.Difficulty] = rewardCurrencySettings;
        }
    }

    public Currency GetCurrency(int currencySaveId)
    {
        return _currencyBySaveId.GetValueOrDefault(currencySaveId, null);
    }

    public MissionObjectiveSettings GetMissionObjectiveSettings(Mission.Objective objective)
    {
        return _missionObjectiveSettingsByMissionObjective.GetValueOrDefault(objective, null);
    }
    
    public RewardCurrencySettings GetRewardCurrencySettings(Mission.Difficulty difficulty)
    {
        return _rewardCurrencySettingsByMissionDifficulty.GetValueOrDefault(difficulty, null);
    }
}
