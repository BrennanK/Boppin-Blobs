using BehaviorTree;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class AIController : MonoBehaviour, IBoppable {
    enum EAIStates {
        CHASING_KING,
        KING_SEARCHING_PATH,
        KING_FOLLOWING_PATH,
        KING_FOLLOWING_RANDOM_PATH,
        KING_FOLLOWING_PREFERRED_PATH,
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
    private bool m_isBeingKnockedBack = false;
    private bool m_canAttack = false;

    private void Awake() {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_rigibody = GetComponent<Rigidbody>();
        m_taggingIdentifier = GetComponent<TaggingIdentifier>();
        m_rigibody.isKinematic = true;
    }

    private void Start() {
        m_behaviorTree = new BehaviorTree.BehaviorTree(
           new BehaviorTreeBuilder()
               .Selector("AI Behavior Main Selector")
                   .Condition("Is Being Knocked Back", IsBeingKnockedBack)
                   .Sequence("Is IT Sequence")
                       .Condition("Check if is IT", IsIt)
                       .Selector("is King Selector - Select one of these actions to use to run away")
                           // Run to random point (this can be biased or not)
                           .Condition("Check if can search for preferred path", IsKingFollowingPath)
                           .Action("Run to farthest preferred location", RunToFarthestPreferredLocation)
                           .Action("Run away from closest player", RunAwayFromClosestPlayer)
                       .End()
                   .End()
                   .Sequence("Is not IT sequence")
                       .Condition("Has Player Available", HasPlayerToFollow)
                       .Selector("Attack or Follow")
                           .Sequence("Attack if possible")
                               .Condition("Check if within attacking distance", IsWithinAttackingDistance)
                               .Condition("Check if Player can attack", CanAttack)
                               .Action("Attack It", AttackNearestPlayer)
                           .End()
                           .Sequence("Run towards It")
                               .Action("Run towards available player", ChaseKing)
                           .End()
                       .End()
                   .End()
               .End()
               .Build()
           );

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

    private EReturnStatus IsBeingKnockedBack() {
        if (m_isBeingKnockedBack) {
            return EReturnStatus.SUCCESS;
        } else {
            return EReturnStatus.FAILURE;
        }
    }

    #region King Functions
    private EReturnStatus IsIt() {
        if(m_taggingIdentifier.AmITag()) {
            return EReturnStatus.SUCCESS;
        } else {
            return EReturnStatus.FAILURE;
        }
    }

    private EReturnStatus IsKingFollowingPath() {
        if(m_currentState == EAIStates.CHASING_KING) {
            // AI is King but is Chasing King? AI just got King!
            return EReturnStatus.FAILURE;
        }

        if(m_currentState == EAIStates.KING_SEARCHING_PATH) {
            return EReturnStatus.FAILURE;
        }

        if(m_currentState == EAIStates.KING_FOLLOWING_PREFERRED_PATH || 
            m_currentState == EAIStates.KING_FOLLOWING_PATH ||
            m_currentState == EAIStates.KING_FOLLOWING_RANDOM_PATH) {
            // Checking if we are at stopping distance
            if(m_navMeshAgent.remainingDistance < 0.5f) {
                return EReturnStatus.FAILURE;
            }
        }

        return EReturnStatus.SUCCESS;
    }

    private EReturnStatus RunToFarthestPreferredLocation() {
        Vector3 closestPlayerPosition = GetClosestPlayerPosition();
        Transform[] possibleSpotsToRunTo = m_taggingIdentifier.taggingManager.aiPreferredSpots;

        if(possibleSpotsToRunTo.Length == 0) {
            return EReturnStatus.FAILURE;
        }

        Vector3 preferredLocation = possibleSpotsToRunTo[0].position;
        for(int i = 1; i < possibleSpotsToRunTo.Length; i++) {
            if(Vector3.Distance(closestPlayerPosition, possibleSpotsToRunTo[i].position) > Vector3.Distance(closestPlayerPosition, preferredLocation)) {
                preferredLocation = possibleSpotsToRunTo[i].position;
            }
        }

        m_currentState = EAIStates.KING_FOLLOWING_PREFERRED_PATH;
        SetPathToAgentFromPosition(preferredLocation);
        return EReturnStatus.RUNNING;
    }

    private EReturnStatus RunAwayFromClosestPlayer() {
        Vector3 closestPlayerPosition = GetClosestPlayerPosition();

        Vector3 positionToMove = transform.position - (closestPlayerPosition - transform.position);
        SetPathToAgentFromPosition(positionToMove);
        m_currentState = EAIStates.KING_FOLLOWING_PATH;

        return EReturnStatus.RUNNING;
    }

    private void SetPathToAgentFromPosition(Vector3 _position) {
        _position = new Vector3(_position.x, transform.position.y, _position.z);

        NavMeshPath path = new NavMeshPath();
        if(m_navMeshAgent.CalculatePath(_position, path)) {
            m_navMeshAgent.SetPath(path);
        } else {
            m_currentState = EAIStates.KING_FOLLOWING_RANDOM_PATH;
            SetARandomPointOnMavMesh();
        }
    }
    #endregion

    #region Is Not King Functions
    private EReturnStatus HasPlayerToFollow() {
        if (m_playerCurrentlyBeingFollowed == null) {
            m_playerCurrentlyBeingFollowed = m_taggingIdentifier.taggingManager.GetItTransform();
            return EReturnStatus.FAILURE;
        }

        return EReturnStatus.SUCCESS;
    }

    private EReturnStatus ChaseKing() {
        if(m_navMeshAgent.isOnNavMesh) {
            m_navMeshAgent.SetDestination(m_playerCurrentlyBeingFollowed.position + m_playerCurrentlyBeingFollowed.forward);
            return EReturnStatus.SUCCESS;
        }

        return EReturnStatus.FAILURE;
    }
    #endregion

    #region Attacking Related (can be used on is it or is not it)
    private EReturnStatus IsWithinAttackingDistance() {
        if (Vector3.Distance(transform.position, m_playerCurrentlyBeingFollowed.position) < attackingDistance) {
            return EReturnStatus.SUCCESS;
        }

        return EReturnStatus.FAILURE;
    }

    private EReturnStatus CanAttack() {
        if (m_taggingIdentifier.TaggingState == TaggingIdentifier.ETaggingBehavior.TaggingAtacking) {
            return EReturnStatus.FAILURE;
        }

        return EReturnStatus.SUCCESS;
    }

    private EReturnStatus AttackNearestPlayer() {
        if (!m_canAttack) {
            m_canAttack = true;
            return EReturnStatus.SUCCESS;
        }

        return EReturnStatus.FAILURE;
    }
    #endregion

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
        float range = 5f;
        // TODO max tries can be a constant value on class
        int maxTries = 10;

        for(int i = 0; i < maxTries; i++) {
            Vector3 randomPosition = _point + (Random.insideUnitSphere * range);
            NavMeshHit navMeshHit;

            if (NavMesh.SamplePosition(randomPosition, out navMeshHit, range, NavMesh.AllAreas)) {
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
