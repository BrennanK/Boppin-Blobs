using UnityEngine;

namespace StoreServices.Core.Achievements {
    [System.Serializable]
    public class AchievementInstance {
        [System.NonSerialized]
        private readonly Achievement m_achievementReference;
        public string AchievementInternalID {
            get {
                return m_achievementReference.internalAchievementID;
            }
        }

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

        [SerializeField]
        private float m_currentProgress;

        [SerializeField]
        private bool m_isCurrentlyHidden;

        [SerializeField]
        private long m_lastModification = 0;

        [SerializeField]
        private bool m_isCompletedAlready = false;
        public bool AlreadyCompleted {
            get {
                return m_isCompletedAlready;
            }
        }


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
            set {
                m_lastModification = System.DateTime.Now.Ticks;
                m_isCurrentlyHidden = false;
                m_currentProgress = value;
            }
        }

        public AchievementInstance(Achievement _achievementReference) {
            m_achievementReference = _achievementReference;
            m_currentProgress = 0;
            m_isCurrentlyHidden = _achievementReference.isAchievementHidden;
            m_lastModification = 0;
            m_isCompletedAlready = false;
        }

        public AchievementInstance(Achievement _achievementReference, float _currentProgress, bool _isCurrentlyHidden, long _lastModification, bool _isCompletedAlready) {
            m_achievementReference = _achievementReference;
            m_currentProgress = _currentProgress;
            m_isCurrentlyHidden = _isCurrentlyHidden;
            m_lastModification = _lastModification;
            m_isCompletedAlready = _isCompletedAlready;
        }

        public override string ToString() {
            return $"Achievement {m_achievementReference.name} - Progress: {m_currentProgress}/{m_achievementReference.goalValue} - Last Modification: {m_lastModification}";
        }
    }
}
