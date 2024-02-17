using UnityEditor;

public static class ToolsClearSaveData
{
    [MenuItem("Tools/Clear Save Data")]
    public static void ClearAllSave()
    {
        SaveData.Clear();
    }
}
