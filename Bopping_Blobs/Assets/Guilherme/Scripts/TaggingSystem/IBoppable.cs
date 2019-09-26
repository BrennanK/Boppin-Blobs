using UnityEngine;

public interface IBoppable {
    // Tagging
    bool HasAttacked();
    void TriggerAttackTransition();
    void TriggerEndAttackTransition();
    void UpdateWhoIsTag(Transform _whoIsTag);
    void ChangeSpeed(float _newSpeed);
    float GetSpeed();

    // Knockback
    void DeactivateController();
    void ReactivateController();
}