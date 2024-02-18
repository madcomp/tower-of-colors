using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MissionObjectiveSettings))]
[CanEditMultipleObjects]
public class MissionObjectiveSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var propertyObjective = serializedObject.FindProperty("objective");
        EditorGUILayout.PropertyField(propertyObjective);
        Mission.Objective objective = (Mission.Objective)propertyObjective.intValue;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));
        
        EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
        
        if (objective == Mission.Objective.CompleteOneLevelInLessThanXSeconds)
        {
            EditorGUILayout.LabelField("Seconds per current level:");
        }
        else if (objective == Mission.Objective.CompleteXLevels)
        {
            EditorGUILayout.LabelField("Levels per current level:");
        }
        else
        {
            EditorGUILayout.LabelField("Tiles per current level:");
        }
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("xPerLevelOnEasy"), new GUIContent("Easy"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("xPerLevelOnMedium"), new GUIContent("Medium"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("xPerLevelOnHard"), new GUIContent("Hard"));

        if (objective == Mission.Objective.DestroyXTilesInLessThanYSeconds)
        {
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("Seconds per current level:");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("yPerLevelOnEasy"), new GUIContent("Easy"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("yPerLevelOnMedium"), new GUIContent("Medium"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("yPerLevelOnHard"), new GUIContent("Hard"));
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
