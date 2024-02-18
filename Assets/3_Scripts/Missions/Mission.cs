using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public abstract class Mission
{
    public enum Objective
    {
        CompleteOneLevelInLessThanXSeconds,
        CompleteXLevels,
        DestroyXTiles,
        DestroyXTilesInLessThanYSeconds,
        DestroyXTilesUsingOneShot,
        DestroyXExplosiveTiles
        
        // IMPORTANT: Add new objectives here, at the end of the enum.
        // Do not change the enum order or it will mess saved data.
    }

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    static Mission Create(Objective objective, int x, int y, Reward reward, float progress)
    {
        switch (objective)
        {
            case Objective.CompleteOneLevelInLessThanXSeconds: return new MissionCompleteOneLevelInLessThanXSeconds(x, reward, progress);
            case Objective.CompleteXLevels:                    return new MissionCompleteXLevels(x, reward, progress);
            case Objective.DestroyXTiles:                      return new MissionDestroyXTiles(x, reward, progress);
            case Objective.DestroyXTilesInLessThanYSeconds:    return new MissionDestroyXTilesInLessThanYSeconds(x, y, reward, progress);
            case Objective.DestroyXTilesUsingOneShot:          return new MissionDestroyXTilesUsingOneShot(x, reward, progress);
            case Objective.DestroyXExplosiveTiles:             return new MissionDestroyXExplosiveTiles(x, reward, progress);
        }
        return null;
    }
    
    public static Mission Create(MissionObjectiveSettings missionObjectiveSettings, Difficulty difficulty, int level, Reward reward)
    {
        switch (missionObjectiveSettings.Objective)
        {
            case Objective.CompleteOneLevelInLessThanXSeconds: return new MissionCompleteOneLevelInLessThanXSeconds(missionObjectiveSettings, difficulty, level, reward);
            case Objective.CompleteXLevels:                    return new MissionCompleteXLevels(missionObjectiveSettings, difficulty, level, reward);
            case Objective.DestroyXTiles:                      return new MissionDestroyXTiles(missionObjectiveSettings, difficulty, level, reward);
            case Objective.DestroyXTilesInLessThanYSeconds:    return new MissionDestroyXTilesInLessThanYSeconds(missionObjectiveSettings, difficulty, level, reward);
            case Objective.DestroyXTilesUsingOneShot:          return new MissionDestroyXTilesUsingOneShot(missionObjectiveSettings, difficulty, level, reward);
            case Objective.DestroyXExplosiveTiles:             return new MissionDestroyXExplosiveTiles(missionObjectiveSettings, difficulty, level, reward);
        }
        return null;
    }

    public static string ToData(List<Mission> missions)
    {
        var data = new List<string>();
        foreach (var mission in missions)
        {
            data.Add(mission.ToData());
        }
        return string.Join(",", data);
    }

    public static List<Mission> ParseData(Context context, string missionsData)
    {
        var missions = new List<Mission>();
        
        var parts = missionsData.Split(",");
        foreach (var part in parts)
        {
            var mission = ParseMissionData(context, part);
            if (mission != null)
            {
                missions.Add(mission);
            }
        }

        return missions;
    }
    
    static Mission ParseMissionData(Context context, string missionData)
    {
        // Data is in format "objective|x|progress_rewardCurrency|rewardAmount|collected" or
        // "objective|x|progress|y_rewardCurrency|rewardAmount|collected"
        
        var parts = missionData.Split("_");
        if (parts.Length != 2)
        {
            Debug.LogError("Corrupted mission data");
            return null;
        }

        var reward = Reward.ParseData(context, parts[1]);
        if (reward == null)
        {
            return null;
        }

        var missionParts = parts[0].Split("|");
        if (missionParts.Length < 3)
        {
            Debug.LogError("Corrupted mission data");
            return null;
        }
        if (!int.TryParse(missionParts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int intObjective))
        {
            Debug.LogError("Corrupted mission objective");
            return null;
        }
        if (!int.TryParse(missionParts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int x))
        {
            Debug.LogError("Corrupted mission x");
            return null;
        }
        if (!float.TryParse(missionParts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float progress))
        {
            Debug.LogError("Corrupted mission progress value");
            return null;
        }

        int y = 0;
        if (missionParts.Length > 3 && !int.TryParse(missionParts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out y))
        {
            Debug.LogError("Corrupted mission y");
            return null;
        }

        var objective = (Objective)intObjective;
        return Create(objective, x, y, reward, progress);
    }

    protected float _progress;
    public Reward Reward { get; }
    protected Mission(Reward reward, float progress = -1f)
    {
        _progress = progress;
        Reward = reward;
    }

    public abstract string GetDescription(Context context);
    public abstract bool IsComplete();
    public abstract Objective MissionObjective { get; }
    public abstract void OnBallShot();
    public abstract void OnGameStart();
    public abstract void OnGameWin();
    public abstract void OnTileDisabled(TowerTile tile);
    protected abstract string ToData();
}

public class MissionCompleteOneLevelInLessThanXSeconds : Mission
{
    float _timeStart;
    int _seconds;
    public MissionCompleteOneLevelInLessThanXSeconds(int seconds, Reward reward, float progress) : base(reward, progress)
    {
        _seconds = seconds;
    }
    public MissionCompleteOneLevelInLessThanXSeconds(MissionObjectiveSettings missionObjectiveSettings, Difficulty difficulty, int level, Reward reward) : base(reward)
    {
        _seconds = Mathf.RoundToInt(missionObjectiveSettings.XPerLevel(difficulty).Evaluate(level));
    }

    public override string GetDescription(Context context)
    {
        var missionObjectiveSettings = context.GetMissionObjectiveSettings(MissionObjective);
        var description = string.Format(missionObjectiveSettings.Description, _seconds);
        if (_progress > 0f)
        {
            // We ceil here to not frustrate players because of rounding errors.
            var progress = Mathf.Ceil(10f * _progress) * 0.1f;
            description += $" {string.Format(missionObjectiveSettings.DescriptionProgress, progress.ToString("N1"))}";
        }
        return description;
    }
    public override bool IsComplete() => _progress > 0f && _progress <= _seconds;
    public override Objective MissionObjective => Objective.CompleteOneLevelInLessThanXSeconds;
    
    public override void OnBallShot() {}

    public override void OnGameStart()
    {
        _timeStart = Time.timeSinceLevelLoad;
    }
    public override void OnGameWin()
    {
        var timeToWin = Time.timeSinceLevelLoad - _timeStart;
        if (_progress > 0f)
        {
            _progress = Mathf.Min(_progress, timeToWin);
        }
        else
        {
            _progress = timeToWin;
        }
    }
    public override void OnTileDisabled(TowerTile tile) {}
    
    protected override string ToData() => $"{(int)MissionObjective}|{_seconds}|{_progress}_{Reward.ToData()}";
}

public class MissionCompleteXLevels : Mission
{
    int _levels;
    public MissionCompleteXLevels(int levels, Reward reward, float progress) : base(reward, progress)
    {
        _levels = levels;
    }
    public MissionCompleteXLevels(MissionObjectiveSettings missionObjectiveSettings, Difficulty difficulty, int level, Reward reward) : base(reward)
    {
        _levels = Mathf.RoundToInt(missionObjectiveSettings.XPerLevel(difficulty).Evaluate(level));
    }
    
    public override string GetDescription(Context context)
    {
        var missionObjectiveSettings = context.GetMissionObjectiveSettings(MissionObjective);
        var description = string.Format(missionObjectiveSettings.Description, _levels);
        if (_progress > 0)
        {
            description += $" {string.Format(missionObjectiveSettings.DescriptionProgress, (int)_progress, _levels)}";
        }
        return description;
    }
    public override bool IsComplete() => _progress >= _levels;
    public override Objective MissionObjective => Objective.CompleteXLevels;
    
    public override void OnBallShot() {}
    public override void OnGameStart() {}
    public override void OnGameWin() => _progress = Mathf.Max(1, _progress + 1);
    public override void OnTileDisabled(TowerTile tile) {}
    
    protected override string ToData() => $"{(int)MissionObjective}|{_levels}|{_progress}_{Reward.ToData()}";
}

public class MissionDestroyXTiles : Mission
{
    int _tiles;
    public MissionDestroyXTiles(int tiles, Reward reward, float progress) : base(reward, progress)
    {
        _tiles = tiles;
    }
    public MissionDestroyXTiles(MissionObjectiveSettings missionObjectiveSettings, Difficulty difficulty, int level, Reward reward) : base(reward)
    {
        _tiles = Mathf.RoundToInt(missionObjectiveSettings.XPerLevel(difficulty).Evaluate(level));
    }
    
    public override string GetDescription(Context context)
    {
        var missionObjectiveSettings = context.GetMissionObjectiveSettings(MissionObjective);
        var description = string.Format(missionObjectiveSettings.Description, _tiles);
        if (_progress > 0)
        {
            description += $" {string.Format(missionObjectiveSettings.DescriptionProgress, (int)_progress, _tiles)}";
        }
        return description;
    }
    public override bool IsComplete() => _progress >= _tiles;
    public override Objective MissionObjective => Objective.DestroyXTiles;

    public override void OnBallShot() {}
    public override void OnGameStart() {}
    public override void OnGameWin() {}
    public override void OnTileDisabled(TowerTile tile) => _progress = Mathf.Max(1, _progress + 1);
    
    protected override string ToData() => $"{(int)MissionObjective}|{_tiles}|{_progress}_{Reward.ToData()}";
}

public class MissionDestroyXTilesInLessThanYSeconds : Mission
{
    int _tiles;
    int _seconds;
    List<float> _times = new List<float>();
    public MissionDestroyXTilesInLessThanYSeconds(int tiles, int seconds, Reward reward, float progress) : base(reward, progress)
    {
        _tiles = tiles;
        _seconds = seconds;
    }
    public MissionDestroyXTilesInLessThanYSeconds(MissionObjectiveSettings missionObjectiveSettings, Difficulty difficulty, int level, Reward reward) : base(reward)
    {
        _tiles = Mathf.RoundToInt(missionObjectiveSettings.XPerLevel(difficulty).Evaluate(level));
        _seconds = Mathf.RoundToInt(missionObjectiveSettings.YPerLevel(difficulty).Evaluate(level));
    }
    
    public override string GetDescription(Context context)
    {
        var missionObjectiveSettings = context.GetMissionObjectiveSettings(MissionObjective);
        var description = string.Format(missionObjectiveSettings.Description, _tiles, _seconds);
        if (_progress > 0)
        {
            description += $" {string.Format(missionObjectiveSettings.DescriptionProgress, (int)_progress)}";
        }
        return description;
    }
    public override bool IsComplete() => _progress >= _tiles;
    public override Objective MissionObjective => Objective.DestroyXTilesInLessThanYSeconds;
    
    public override void OnBallShot() {}
    public override void OnGameStart() => _times.Clear();
    public override void OnGameWin() {}
    public override void OnTileDisabled(TowerTile tile)
    {
        var time = Time.timeSinceLevelLoad;
        _times.Add(time);
        _times.RemoveAll(t => (time - t) > _seconds);
        _progress = Mathf.Max(_progress, _times.Count);
    }
    
    protected override string ToData() => $"{(int)MissionObjective}|{_tiles}|{_progress}|{_seconds}_{Reward.ToData()}";
}

public class MissionDestroyXTilesUsingOneShot : Mission
{
    int _currentProgress = 0;
    int _tiles;
    public MissionDestroyXTilesUsingOneShot(int tiles, Reward reward, float progress) : base(reward, progress)
    {
        _tiles = tiles;
    }
    public MissionDestroyXTilesUsingOneShot(MissionObjectiveSettings missionObjectiveSettings, Difficulty difficulty, int level, Reward reward) : base(reward)
    {
        _tiles = Mathf.RoundToInt(missionObjectiveSettings.XPerLevel(difficulty).Evaluate(level));
    }
    public override string GetDescription(Context context)
    {
        var missionObjectiveSettings = context.GetMissionObjectiveSettings(MissionObjective);
        var description = string.Format(missionObjectiveSettings.Description, _tiles);
        if (_progress > 0)
        {
            description += $" {string.Format(missionObjectiveSettings.DescriptionProgress, (int)_progress)}";
        }
        return description;
    }
    public override bool IsComplete() => _progress >= _tiles;
    public override Objective MissionObjective => Objective.DestroyXTilesUsingOneShot;

    public override void OnBallShot() => _currentProgress = 0;
    public override void OnGameStart() {}
    public override void OnGameWin() {}
    public override void OnTileDisabled(TowerTile tile)
    {
        _currentProgress++;
        _progress = Mathf.Max(_progress, _currentProgress);
    }
    
    protected override string ToData() => $"{(int)MissionObjective}|{_tiles}|{_progress}_{Reward.ToData()}";
}

public class MissionDestroyXExplosiveTiles : Mission
{
    int _tiles;
    public MissionDestroyXExplosiveTiles(int tiles, Reward reward, float progress) : base(reward, progress)
    {
        _tiles = tiles;
    }
    public MissionDestroyXExplosiveTiles(MissionObjectiveSettings missionObjectiveSettings, Difficulty difficulty, int level, Reward reward) : base(reward)
    {
        _tiles = Mathf.RoundToInt(missionObjectiveSettings.XPerLevel(difficulty).Evaluate(level));
    }
    public override string GetDescription(Context context)
    {
        var missionObjectiveSettings = context.GetMissionObjectiveSettings(MissionObjective);
        var description = string.Format(missionObjectiveSettings.Description, _tiles);
        if (_progress > 0)
        {
            description += $" {string.Format(missionObjectiveSettings.DescriptionProgress, (int)_progress, _tiles)}";
        }
        return description;
    }
    public override bool IsComplete() => _progress >= _tiles;
    public override Objective MissionObjective => Objective.DestroyXExplosiveTiles;
    
    public override void OnBallShot() {}
    public override void OnGameStart() {}
    public override void OnGameWin() {}
    public override void OnTileDisabled(TowerTile tile)
    {
        if (tile.IsExplosive())
        {
            _progress = Mathf.Max(1, _progress + 1);
        }
    }
    
    protected override string ToData() => $"{(int)MissionObjective}|{_tiles}|{_progress}_{Reward.ToData()}";
}
