using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionsManager : MonoBehaviour
{
    [SerializeField] private Currency[] currencies;
    [SerializeField] private RewardCurrencySettings[] rewardsCurrencySettings;
    [SerializeField] private MissionObjectiveSettings[] missionObjectivesSettings;
    [SerializeField] private PopupMissions popupMissions;

    Animator _animator;
    private Context _context;
    List<Mission> _missions = new List<Mission>();
    bool _popupVisible = false;

    public void HidePopup()
    {
        _popupVisible = false;
        _animator.SetBool("MissionsPopupVisible", _popupVisible);
    }
    
    public void ShowPopup()
    {
        popupMissions.Setup(_context, _missions);
        _popupVisible = true;
        _animator.SetBool("MissionsPopupVisible", _popupVisible);
    }
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.speed = 1.0f / Time.timeScale;
        _context = new Context(currencies, missionObjectivesSettings, rewardsCurrencySettings);
        LoadMissions();
    }

    void CreateMissions()
    {
        _missions.Clear();

        var currentLevel = SaveData.CurrentLevel;
        
        var availableMissionObjectivesSettings = new List<MissionObjectiveSettings>(missionObjectivesSettings);
        var difficulties = new Mission.Difficulty[]
        {
            Mission.Difficulty.Easy, Mission.Difficulty.Medium, Mission.Difficulty.Hard
        };
        foreach (var difficulty in difficulties)
        {
            var chosenRewardCurrencySettings = _context.GetRewardCurrencySettings(difficulty);
            var possibleCurrencyAmounts = chosenRewardCurrencySettings.PossibleCurrencyAmounts;
            var indexCurrencyAmount = UnityEngine.Random.Range(0, possibleCurrencyAmounts.Length);
            var currencyAmount = possibleCurrencyAmounts[indexCurrencyAmount];
            var reward = new RewardCurrency(currencyAmount);
            
            var indexObjective = UnityEngine.Random.Range(0, availableMissionObjectivesSettings.Count);
            var chosenMissionObjectivesSettings = availableMissionObjectivesSettings.GetAndRemoveAt(indexObjective);
            var mission = Mission.Create(chosenMissionObjectivesSettings, difficulty, currentLevel, reward);
            _missions.Add(mission);
        }
    }

    void LoadMissions()
    {
        var missionsData = SaveData.CurrentMissionsData;
        if (string.IsNullOrEmpty(missionsData))
        {
            CreateMissions();
        }
        else
        {
            _missions.AddRange(Mission.ParseData(_context, missionsData));
        }
    }
}
