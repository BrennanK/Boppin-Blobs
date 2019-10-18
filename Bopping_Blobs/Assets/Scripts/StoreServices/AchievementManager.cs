using StoreServices.Core.Achievements;
using System.Linq;
using UnityEngine;

namespace StoreServices {
    public class AchievementManager : MonoBehaviour {
        public static AchievementManager instance;
        public Achievement[] allAchievements;

        private AchievementInstance[] m_achievementInstances;
        public AchievementInstance[] AchievementInstances {
            get {
                return m_achievementInstances;
            }
        }

        private void Awake() {
            if(instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
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

        public void UpdateAllAchievements(PlayerProfile _profileIncrement, int _finalPlayerPosition) {
            Debug.Log($"Final Player Position: {_finalPlayerPosition}");

            // KING Achievements
            // Tracking Incremental Time as King Based Values
            TrackStandardAchievementUsingInternalID(InternalIDs.achievement_who_needs_royalty, _profileIncrement.timeAsKing == 0);
            TrackIncrementalAchievementUsingInternalID(InternalIDs.achievement_call_me_king, _profileIncrement.timeAsKing / 60.0f);
            TrackIncrementalAchievementUsingInternalID(InternalIDs.achievement_still_king, _profileIncrement.timeAsKing / 60.0f);
            TrackIncrementalAchievementUsingInternalID(InternalIDs.achievement_the_true_king, _profileIncrement.timeAsKing / 60.0f);
            TrackIncrementalAchievementUsingInternalID(InternalIDs.achievement_king_of_kings, _profileIncrement.timeAsKing / 60.0f);

            // Time Investiment Achievements
            TrackStandardAchievementUsingInternalID(InternalIDs.achievement_first_timer, _profileIncrement.gamesPlayed >= 1);
            TrackIncrementalAchievementUsingInternalID(InternalIDs.achievement_10_down, 1);
            TrackIncrementalAchievementUsingInternalID(InternalIDs.achievement_half_a_century, 1);
            TrackIncrementalAchievementUsingInternalID(InternalIDs.achievement_four_score_and_another_too, 1);
            TrackIncrementalAchievementUsingInternalID(InternalIDs.achievement_no_lifer, 1);

            // Placing Achievements
            TrackStandardAchievementUsingInternalID(InternalIDs.achievement_middle_of_the_pack, _finalPlayerPosition == 4);
            TrackStandardAchievementUsingInternalID(InternalIDs.achievement_room_to_grow, _finalPlayerPosition == 7);
            TrackStandardAchievementUsingInternalID(InternalIDs.achievement_winner, _finalPlayerPosition == 1);
            TrackIncrementalAchievementUsingInternalID(InternalIDs.achievement_going_pro, (_finalPlayerPosition == 1 ? 1 : 0));
            TrackIncrementalAchievementUsingInternalID(InternalIDs.achievement_pretty_good, (_finalPlayerPosition == 1 ? 1 : 0));
            TrackIncrementalAchievementUsingInternalID(InternalIDs.achievement_try_hard, (_finalPlayerPosition == 1 ? 1 : 0));


            // Others
            TrackStandardAchievementUsingInternalID(InternalIDs.achievement_friendly, _profileIncrement.timesAttackedBlobs == 0);
            TrackIncrementalAchievementUsingInternalID(InternalIDs.achievement_hammer_down, _profileIncrement.timesAttackedBlobs);
            TrackIncrementalAchievementUsingInternalID(InternalIDs.achievement_regicide, _profileIncrement.timesKing);

            PersistAchievements();
        }

        private void TrackIncrementalAchievementUsingInternalID(string _achievementID, float _incrementValue) {
            AchievementInstance achievementBeingUpdated = m_achievementInstances.Where((achievement) => {
                return achievement.AchievementInternalID == _achievementID;
            }).FirstOrDefault();

            if (achievementBeingUpdated.AlreadyCompleted) {
                return;
            }

            achievementBeingUpdated.CurrentProgress += _incrementValue;

            if (achievementBeingUpdated.Complete) {
                // TODO Give Money to Player
                Debug.Log($"{achievementBeingUpdated.AchievementName} was completed!");
            }
        }

        private void TrackStandardAchievementUsingInternalID(string _achievementID, bool _isComplete) {
            AchievementInstance achievementBeingUpdated = m_achievementInstances.Where((achievement) => {
                return achievement.AchievementInternalID == _achievementID;
            }).FirstOrDefault();

            if(achievementBeingUpdated.AlreadyCompleted | !_isComplete) {
                return;
            }

            achievementBeingUpdated.CurrentProgress += 1;

            if (achievementBeingUpdated.Complete) {
                // TODO Give Money to Player
                Debug.Log($"{achievementBeingUpdated.AchievementName} was completed!");
            }
        }

        private void PersistAchievements() {
            Debug.Log($"Persist Achievements");

            for (int i = 0; i < m_achievementInstances.Length; i++) {
                Debug.Log(JsonUtility.ToJson(m_achievementInstances[i]));
            }
        }
    }
}
