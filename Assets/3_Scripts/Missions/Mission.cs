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
        ShootXExplosiveTiles
        
        // IMPORTANT: Add new objectives here, at the end of the enum.
        // Do not change the enum order or it will mess saved data.
    }

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    public static Mission Create(Objective objective, int x, int y, Reward reward)
    {
        switch (objective)
        {
            case Objective.CompleteOneLevelInLessThanXSeconds: return new MissionCompleteOneLevelInLessThanXSeconds(x, reward);
            case Objective.CompleteXLevels:                    return new MissionCompleteXLevels(x, reward);
            case Objective.DestroyXTiles:                      return new MissionDestroyXTiles(x, reward);
            case Objective.DestroyXTilesInLessThanYSeconds:    return new MissionDestroyXTilesInLessThanYSeconds(x, y, reward);
            case Objective.DestroyXTilesUsingOneShot:          return new MissionDestroyXTilesUsingOneShot(x, reward);
            case Objective.ShootXExplosiveTiles:               return new MissionShootXExplosiveTiles(x, reward);
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
            case Objective.ShootXExplosiveTiles:               return new MissionShootXExplosiveTiles(missionObjectiveSettings, difficulty, level, reward);
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
        // Data is in format "objective|x|y_rewardCurrency|rewardAmount"
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
        if (missionParts.Length < 2)
        {
            Debug.LogError("Corrupted mission data");
            return null;
        }
        if (!int.TryParse(missionParts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int intObjective))
        {
            Debug.LogError("Corrupted mission objective");
            return null;
        }
        if (!int.TryParse(missionParts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int rewardValue))
        {
            Debug.LogError("Corrupted mission objective");
            return null;
        }

        if (!int.TryParse(missionParts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int x))
        {
            Debug.LogError("Corrupted mission x");
            return null;
        }

        int y = 0;
        if (missionParts.Length > 2 && !int.TryParse(missionParts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out y))
        {
            Debug.LogError("Corrupted mission y");
            return null;
        }

        var objective = (Objective)intObjective;
        return Create(objective, x, y, reward);
    }

    public Reward Reward { get; }
    protected Mission(Reward reward)
    {
        Reward = reward;
    }

    public abstract string GetDescription(Context context);
    public abstract Objective MissionObjective { get; }
    public abstract string ToData();
}

public class MissionCompleteOneLevelInLessThanXSeconds : Mission
{
    private int _seconds;
    public MissionCompleteOneLevelInLessThanXSeconds(int seconds, Reward reward) : base(reward)
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
        return string.Format(missionObjectiveSettings.Description, _seconds);
    }
    public override Objective MissionObjective => Objective.CompleteOneLevelInLessThanXSeconds;
    public override string ToData() => $"{(int)MissionObjective}|{_seconds}_{Reward.ToData()}";
}

public class MissionCompleteXLevels : Mission
{
    int _levels;
    public MissionCompleteXLevels(int levels, Reward reward) : base(reward)
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
        return string.Format(missionObjectiveSettings.Description, _levels);
    }
    public override Objective MissionObjective => Objective.CompleteXLevels;
    public override string ToData() => $"{(int)MissionObjective}|{_levels}_{Reward.ToData()}";
}

public class MissionDestroyXTiles : Mission
{
    int _tiles;
    public MissionDestroyXTiles(int tiles, Reward reward) : base(reward)
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
        return string.Format(missionObjectiveSettings.Description, _tiles);
    }
    public override Objective MissionObjective => Objective.DestroyXTiles;
    public override string ToData() => $"{(int)MissionObjective}|{_tiles}_{Reward.ToData()}";
}

public class MissionDestroyXTilesInLessThanYSeconds : Mission
{
    int _tiles;
    int _seconds;
    public MissionDestroyXTilesInLessThanYSeconds(int tiles, int seconds, Reward reward) : base(reward)
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
        return string.Format(missionObjectiveSettings.Description, _tiles, _seconds);
    }
    public override Objective MissionObjective => Objective.DestroyXTilesInLessThanYSeconds;
    public override string ToData() => $"{(int)MissionObjective}|{_tiles}|{_seconds}_{Reward.ToData()}";
}

public class MissionDestroyXTilesUsingOneShot : Mission
{
    int _tiles;
    public MissionDestroyXTilesUsingOneShot(int tiles, Reward reward) : base(reward)
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
        return string.Format(missionObjectiveSettings.Description, _tiles);
    }
    public override Objective MissionObjective => Objective.DestroyXTilesUsingOneShot;
    public override string ToData() => $"{(int)MissionObjective}|{_tiles}_{Reward.ToData()}";
}

public class MissionShootXExplosiveTiles : Mission
{
    int _tiles;
    public MissionShootXExplosiveTiles(int tiles, Reward reward) : base(reward)
    {
        _tiles = tiles;
    }
    public MissionShootXExplosiveTiles(MissionObjectiveSettings missionObjectiveSettings, Difficulty difficulty, int level, Reward reward) : base(reward)
    {
        _tiles = Mathf.RoundToInt(missionObjectiveSettings.XPerLevel(difficulty).Evaluate(level));
    }
    public override string GetDescription(Context context)
    {
        var missionObjectiveSettings = context.GetMissionObjectiveSettings(MissionObjective);
        return string.Format(missionObjectiveSettings.Description, _tiles);
    }
    public override Objective MissionObjective => Objective.ShootXExplosiveTiles;
    public override string ToData() => $"{(int)MissionObjective}|{_tiles}_{Reward.ToData()}";
}
