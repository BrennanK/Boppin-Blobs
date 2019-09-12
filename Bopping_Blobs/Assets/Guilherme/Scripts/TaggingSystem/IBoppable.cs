using UnityEngine;

public interface IBoppable {
    // Tagging
    bool HasAttacked();
    void TriggerAttackTransition();
    void TriggerEndAttackTransition();
    void UpdateWhoIsTag(Transform _whoIsTag);

    // Knockback
    void DeactivateController();
    void ReactivateController();
}