public interface IBoppable {
    // Tagging
    bool HasAttacked();
    void TriggerAttackTransition();
    void TriggerEndAttackTransition();

    // Knockback
    void DeactivateController();
    void ReactivateController();
}