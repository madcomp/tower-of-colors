using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionsManager : MonoBehaviour
{
    [SerializeField] private GameObject[] gameObjectsActivableByMissionsConfig;
    [SerializeField] private Currency[] currencies;
    [SerializeField] private RewardCurrencySettings[] rewardsCurrencySettings;
    [SerializeField] private MissionObjectiveSettings[] missionObjectivesSettings;
    [SerializeField] private PopupMissions popupMissions;
    [SerializeField] private CurrencyUI[] currencyUis;

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
        _popupVisible = true;
        _animator.SetBool("MissionsPopupVisible", _popupVisible);
    }

    public void OnBallShot()
    {
        foreach (var mission in _missions)
        {
            mission.OnBallShot();
        }
        SaveData.CurrentMissionsData = Mission.ToData(_missions);
    }

    public void OnGameStart()
    {
        foreach (var mission in _missions)
        {
            mission.OnGameStart();
        }
        SaveData.CurrentMissionsData = Mission.ToData(_missions);
    }
    
    public void OnGameWin()
    {
        foreach (var mission in _missions)
        {
            mission.OnGameWin();
        }
        SaveData.CurrentMissionsData = Mission.ToData(_missions);
    }
    
    public void OnTileDisabled(TowerTile tile)
    {
        foreach (var mission in _missions)
        {
            mission.OnTileDisabled(tile);
        }
        SaveData.CurrentMissionsData = Mission.ToData(_missions);
    }
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.speed = 1.0f / Time.timeScale;
        _context = new Context(currencies, missionObjectivesSettings, rewardsCurrencySettings);
        LoadMissions();
        Player.Instance.Wallet.Load(_context);
        foreach (var currencyUI in currencyUis)
        {
            currencyUI.Setup();
        }

        foreach (var gameObjectActivableByMissionsConfig in gameObjectsActivableByMissionsConfig)
        {
            gameObjectActivableByMissionsConfig.SetActive(RemoteConfig.MISSIONS_ENABLED);
        }
    }

    void CreateMissions()
    {
        foreach (var mission in _missions)
        {
            mission.Reward.OnCollect = null;
        }
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
            var reward = new RewardCurrency(currencyAmount, collected: false);
            
            var indexObjective = UnityEngine.Random.Range(0, availableMissionObjectivesSettings.Count);
            var chosenMissionObjectivesSettings = availableMissionObjectivesSettings.GetAndRemoveAt(indexObjective);
            var mission = Mission.Create(chosenMissionObjectivesSettings, difficulty, currentLevel, reward);
            _missions.Add(mission);

            mission.Reward.OnCollect += OnCollectMissionReward;
        }

        popupMissions.Setup(_context, _missions);
        
        SaveData.CurrentMissionsData = Mission.ToData(_missions);
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
            popupMissions.Setup(_context, _missions);
            foreach (var mission in _missions)
            {
                mission.Reward.OnCollect += OnCollectMissionReward;
            }
        }
    }

    void OnCollectMissionReward()
    {
        if (AllMissionsCollected())
        {
            CreateMissions();
        }
        else
        {
            SaveData.CurrentMissionsData = Mission.ToData(_missions);
        }
    }

    bool AllMissionsCollected()
    {
        foreach (var mission in _missions)
        {
            if (!mission.Reward.IsCollected())
            {
                return false;
            }
        }
        return true;
    }
}
