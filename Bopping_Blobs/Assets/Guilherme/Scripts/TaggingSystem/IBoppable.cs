using UnityEngine;

public interface IBoppable {
    // Tagging
    bool HasAttacked();
    void TriggerAttackTransition();
    void TriggerEndAttackTransition();
    void UpdateWhoIsTag(Transform _whoIsTag);
    void ChangeSpeed(float _baseSpeed, float _tempSpeedBost, float _externalSpeedBoost);
    float GetSpeed();

    // Knockback
    void DeactivateController(bool _updateAnimation = false);
    void ReactivateController();
}