using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StoreServices.AchievementManager))]
public class AchievementsVisualizer : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        StoreServices.AchievementManager achievementManager = FindObjectOfType<StoreServices.AchievementManager>();
        StoreServices.Core.Achievements.AchievementInstance[] instances = achievementManager.AchievementInstances;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Achievement Name", GUILayout.Width(200));
        GUILayout.Label("Progress", GUILayout.Width(200));
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.Label("Achievements Instances", GUILayout.Width(200));
        foreach (StoreServices.Core.Achievements.AchievementInstance instance in instances) {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{instance.AchievementName}", GUILayout.Width(200));
            GUILayout.Label($"{instance.CurrentProgress}/{instance.GoalValue}", GUILayout.Width(200));
            GUILayout.EndHorizontal();
        }
    }
}
