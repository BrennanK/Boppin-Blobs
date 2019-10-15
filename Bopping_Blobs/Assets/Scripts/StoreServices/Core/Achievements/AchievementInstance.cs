using UnityEngine;

namespace StoreServices.Core.Achievements {
    [System.Serializable]
    public class AchievementInstance : MonoBehaviour {
        public Achievement achievementReference;
        public float currentProgress;
        public bool isCurrentlyHidden = false;
        public long lastModification = 0;

        public bool Complete {
            get {
                return currentProgress >= achievementReference.goalValue;
            }
        }

        public float ProgressInPercentage {
            get {
                return (currentProgress / achievementReference.goalValue);
            }
        }

        public float CurrentProgress {
            get {
                return currentProgress;
            }
        }
    }
}
