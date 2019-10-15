using StoreServices.Core.Achievements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoreServices {
    public class AchievementManager : MonoBehaviour {
        public Achievement[] allAchievements;
        private AchievementInstance[] m_achievementInstances;

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
