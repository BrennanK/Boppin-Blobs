using UnityEngine;

namespace StoreServices.Core.Achievements {
    [System.Serializable]
    public class AchievementInstance {
        [System.NonSerialized]
        private readonly Achievement m_achievementReference;
        public string AchievementName {
            get {
                return m_achievementReference.name;
            }
        }

        public float GoalValue {
            get {
                return m_achievementReference.goalValue;
            }
        }

        private float m_currentProgress;
        private bool m_isCurrentlyHidden = false;
        private long m_lastModification = 0;


        public bool Complete {
            get {
                return m_currentProgress >= m_achievementReference.goalValue;
            }
        }

        public float ProgressInPercentage {
            get {
                return (m_currentProgress / m_achievementReference.goalValue);
            }
        }

        public float CurrentProgress {
            get {
                return m_currentProgress;
            }
        }

        public AchievementInstance(Achievement _achievementReference) {
            m_achievementReference = _achievementReference;
            m_currentProgress = 0;
            m_isCurrentlyHidden = false;
            m_lastModification = 0;
        }

        public AchievementInstance(Achievement _achievementReference, float _currentProgress, bool _isCurrentlyHidden, long _lastModification) {
            m_achievementReference = _achievementReference;
            m_currentProgress = _currentProgress;
            m_isCurrentlyHidden = _isCurrentlyHidden;
            m_lastModification = _lastModification;
        }

        public override string ToString() {
            return $"Achievement {m_achievementReference.name} - Progress: {m_currentProgress}/{m_achievementReference.goalValue} - Last Modification: {m_lastModification}";
        }
    }
}
