using BehaviorTree;
using PowerUp;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour, IBoppable {
    enum EAIStates {
        CHASING_KING,
        KING_SEARCHING_PATH,
        KING_FOLLOWING_PATH,
        KING_FOLLOWING_RANDOM_PATH
    }

    private EAIStates m_currentState = EAIStates.CHASING_KING;

    [Header("AI Configuration")]
    public float baseReactionTime = 0.35f;
    public float reactionTimeVariation = 0.05f;
    public float minStartTime = 0.15f;
    public float maxStartTime = 0.35f;
    public float attackingDistance = 0.5f;
    private BehaviorTree.BehaviorTree m_behaviorTree;

    private NavMeshAgent m_navMeshAgent;
    private Rigidbody m_rigibody;

    // Variables for Tagging AI
    private TaggingIdentifier m_taggingIdentifier;
    private Transform m_playerCurrentlyBeingFollowed;
    private PowerUpTracker m_powerUpTracker;
    private bool m_isBeingKnockedBack = false;
    private bool m_canAttack = false;

    private void Awake() {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_rigibody = GetComponent<Rigidbody>();
        m_taggingIdentifier = GetComponent<TaggingIdentifier>();
        m_powerUpTracker = GetComponent<PowerUpTracker>();
        m_rigibody.isKinematic = true;
    }

    private void Start() {
        StartCoroutine(UpdateTreeRoutine(Random.Range(minStartTime, maxStartTime)));
    }

    #region IBoppable Functions
    public bool HasAttacked() {
        return m_canAttack;
    }

    public void TriggerAttackTransition() {
        m_canAttack = false;
    }

    public void TriggerEndAttackTransition() {
        return;
    }

    public void UpdateWhoIsTag(Transform _whoIsTag) {
        m_playerCurrentlyBeingFollowed = _whoIsTag;
    }

    public void ChangeSpeed(float _newSpeed) {
        m_navMeshAgent.speed = _newSpeed;
        m_navMeshAgent.acceleration = _newSpeed * 2f;
    }

    public float GetSpeed() {
        return m_navMeshAgent.speed;
    }

    public void DeactivateController() {
        m_isBeingKnockedBack = true;
        m_navMeshAgent.enabled = false;

        if(m_navMeshAgent.isOnNavMesh) {
            m_navMeshAgent.ResetPath();
        }
    }

    public void ReactivateController() {
        m_isBeingKnockedBack = false;
        m_navMeshAgent.enabled = true;
    }
    #endregion

    #region BEHAVIOR TREE ACTIONS
    private IEnumerator UpdateTreeRoutine(float _delay) {
        yield return new WaitForSeconds(_delay);
        UpdateTree();
        StartCoroutine(UpdateTreeRoutine(baseReactionTime + Random.Range(-reactionTimeVariation, reactionTimeVariation)));
    }

    private void UpdateTree() {
        m_behaviorTree.Update();
    }

    #endregion

    #region Helper Functions
    private Vector3 GetClosestPlayerPosition() {
        TaggingIdentifier[] notKingPlayers = m_taggingIdentifier.taggingManager.GetAllPlayersThatAreNotIt();

        Transform closestPlayer = notKingPlayers[0].transform;
        for (int i = 1; i < notKingPlayers.Length; i++) {
            if (Vector3.Distance(transform.position, notKingPlayers[i].transform.position) < Vector3.Distance(transform.position, closestPlayer.position)) {
                closestPlayer = notKingPlayers[i].transform;
            }
        }

        return closestPlayer.position;
    }

    // TODO Get a random point given two angles (input: I want to escape considering these two angles)
    private Vector3 GetRandomPositionAroundAPoint(Vector3 _point) {
        // TODO maybe this range can be a public variable to tune the AI
        float range = 1f;
        // TODO max tries can be a constant value on class
        int maxTries = 20;

        for(int i = 0; i < maxTries; i++) {

            NavMeshHit navMeshHit;

            if (NavMesh.SamplePosition(_point, out navMeshHit, range, NavMesh.AllAreas)) {
                return navMeshHit.position;
            }
        }

        return Vector3.zero;
    }

    private void SetARandomPointOnMavMesh() {
        float range = 5f;
        for(int i = 0; i < 10; i++) {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * range;
            NavMeshHit hit;

            if(NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) {
                m_navMeshAgent.SetDestination(hit.position);
                return;
            }
        }
    }
    #endregion
}
