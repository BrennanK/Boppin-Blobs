using UnityEngine;

public interface IBoppable {
    // Tagging
    bool HasAttacked();
    void TriggerAttackTransition();
    void TriggerEndAttackTransition();
    void TriggerIsTagTransition();
    void TriggerIsNotTagTransition();
    void UpdateWhoIsTag(Transform _whoIsTag);
    void ChangeSpeed(float _newSpeed);

    // Knockback
    void DeactivateController();
    void ReactivateController();
}