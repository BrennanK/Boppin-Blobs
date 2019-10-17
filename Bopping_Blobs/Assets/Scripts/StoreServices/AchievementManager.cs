using StoreServices.Core.Achievements;
using UnityEngine;

namespace StoreServices {
    public class AchievementManager : MonoBehaviour {
        public Achievement[] allAchievements;

        private AchievementInstance[] m_achievementInstances;
        public AchievementInstance[] AchievementInstances {
            get {
                return m_achievementInstances;
            }
        }

        private void Start() {
            Debug.Log($"Starting Achievement Manager!");
            // Create all achievement instances and/or check for persistence!!
            m_achievementInstances = new AchievementInstance[allAchievements.Length];

            for(int i = 0; i < allAchievements.Length; i++) {
                m_achievementInstances[i] = new AchievementInstance(allAchievements[i]);
                // Debug.Log(JsonUtility.ToJson(m_achievementInstances[i]));
            }
        }

        // TODO this function should receive an increment to the save game?!
        public void UpdateAllAchievements() {

        }

        /// <summary>
        /// <para>Receives the ID and Progress Value for a specific achievement, so it can check for its progress.</para>
        /// </summary>
        /// <param name="_achievementID">Achievement ID</param>
        /// <param name="_progressValue">Progress Value</param>
        private void TrackAchievement(string _achievementID, string _progressValue) {
            // get achievement
            // update progress value and check for completion
        }
    }
}
