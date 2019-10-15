using UnityEngine;

namespace StoreServices.Core.Achievements {
    /// <summary>
    /// <para>States that an achievement can take</para>
    /// </summary>
    public enum EAchievementState {
        /// <summary>
        /// <para>A hidden achievement means that details about the achievement are hidden from the player.</para>
        /// </summary>

        HIDDEN,
        /// <summary>
        /// A revealed achievement means that the player knows about the achievement, but hasn't earned it yet.
        /// </summary>
        REVEALED,

        /// <summary>
        /// <para>An unlocked achievement means that the player has successfully earned the achievement.<para>
        /// </summary>
        UNLOCKED
    }

    /// <summary>
    /// <para>Type of achievement</para>
    /// </summary>
    public enum EAchievementType {
        /// <summary>
        /// <para>Standard achievements can only be locked or unlocked</para>
        /// </summary>
        STANDARD,

        /// <summary>
        /// <para>An incremental achievement involves a player making gradual progress towards earning the achievement over a longer period of time</para>
        /// </summary>
        INCREMENTAL
    }

    [CreateAssetMenu(fileName = "Achievement", menuName = "Achievements/AchievementAsset")]
    public class Achievement : ScriptableObject {
        public string achievementDescription;
        public float goalValue;
        public bool isAchievementHidden;
        public EAchievementType achievementType;

        public string achievementID;
    }
}
