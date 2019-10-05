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
        // TODO maybe somehow have the King AI recognize other blobs as obstacles ?!
        // BAD IDEA 1: Have a pool of navmesh obstacles, so the King AI pull then, activate then, put on all the blobs positions, calculate path, and then deactivate all obstacles.
        m_behaviorTree = new BehaviorTree.BehaviorTree(
           new BehaviorTreeBuilder()
               .Selector("AI Behavior Main Selector")
                   .Condition("Is Being Knocked Back", IsBeingKnockedBack)
                   .Sequence("Is IT Sequence")
                       .Condition("Check if is IT", IsIt)
                       .Selector("is King Selector - Select one of these actions to use to run away")
                            // Short Term Reactions
                            // Get away from closest player
                            // use power up to run
                           .Sequence("Is On Iminent Danger Sequence")
                               .Condition("Check if is on iminent danger", IsOnIminentDanger)
                               .Selector("Try to Use a Power Up To Run")
                                   .Action("Try to use Back Off", UseBackOffPowerUp)
                                   .Action("Try to use Super Speed", UseSuperSpeedPowerUp)
                                   .Condition("Tried all power ups?", TriedAllPowerUps)
                               .End()
                               .Action("Run Away from Closest Player", RunAwayFromClosestPlayer)
                           .End()
                           .Selector("If not on iminent danger, choose of these")
                               .Condition("Check if can search for preferred path", IsKingFollowingPath)
                           .End()
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

    private EReturnStatus IsOnIminentDanger() {
        Debug.Log("Searching for iminent danger =)");
        Vector3 closestPlayer = GetClosestPlayerPosition();
        if(Vector3.Distance(transform.position, closestPlayer) < 15f) {
            return EReturnStatus.SUCCESS;
        }

        return EReturnStatus.FAILURE;
    }

    private EReturnStatus UseBackOffPowerUp() {
        Debug.Log($"AI Trying to use Back of Power Up");
        if(m_powerUpTracker.slot1.powerUp != null) {
            if(m_powerUpTracker.slot1.powerUp.powerUp == EPowerUps.BACK_OFF) {
                m_powerUpTracker.ActivatePowerUp1();
                return EReturnStatus.SUCCESS;
            }
        } else if(m_powerUpTracker.slot2.powerUp != null) {
            if(m_powerUpTracker.slot2.powerUp.powerUp == EPowerUps.BACK_OFF) {
                m_powerUpTracker.ActivatePowerUp2();
                return EReturnStatus.SUCCESS;
            }
        }

        return EReturnStatus.FAILURE;
    }

    private EReturnStatus UseSuperSpeedPowerUp() {
        Debug.Log($"AI Trying to use Speed Up Power Up");

        if (m_powerUpTracker.slot1.powerUp != null) {
            if(m_powerUpTracker.slot1.powerUp.powerUp == EPowerUps.SUPER_SPEED && m_powerUpTracker.slot1.canActivate) {
                m_powerUpTracker.ActivatePowerUp1();
                return EReturnStatus.SUCCESS;
            }
        } else if(m_powerUpTracker.slot2.powerUp != null) {
            if(m_powerUpTracker.slot2.powerUp.powerUp == EPowerUps.SUPER_SPEED && m_powerUpTracker.slot2.canActivate) {
                m_powerUpTracker.ActivatePowerUp2();
                return EReturnStatus.SUCCESS;
            }
        }

        return EReturnStatus.FAILURE;
    }

    private EReturnStatus TriedAllPowerUps() {
        return EReturnStatus.SUCCESS;
    }

    private EReturnStatus IsKingFollowingPath() {
        if(m_currentState == EAIStates.CHASING_KING) {
            // AI is King but is Chasing King? AI just got King!
            return EReturnStatus.FAILURE;
        }

        if(m_currentState == EAIStates.KING_SEARCHING_PATH || m_currentState == EAIStates.KING_FOLLOWING_RANDOM_PATH) {
            return EReturnStatus.FAILURE;
        }

        if(m_currentState == EAIStates.KING_FOLLOWING_PREFERRED_PATH || 
            m_currentState == EAIStates.KING_FOLLOWING_PATH) {
            // Checking if we are at stopping distance
            if(m_navMeshAgent.remainingDistance < 2.0f) {
                return EReturnStatus.FAILURE;
            }
        }

        return EReturnStatus.SUCCESS;
    }

    private EReturnStatus RunAwayFromClosestPlayer() {
        Vector3 closestPlayerPosition = GetClosestPlayerPosition();
        Vector3 directionToMove = transform.position - closestPlayerPosition;
        directionToMove.Scale(new Vector3(3, 3, 3));

        Debug.DrawRay(transform.position, directionToMove, Color.red, 0.125f);
        SetPathToAgentFromPosition(directionToMove);
        m_currentState = EAIStates.KING_FOLLOWING_PATH;

        return EReturnStatus.RUNNING;
    }

    private void SetPathToAgentFromPosition(Vector3 _position) {
        _position = new Vector3(_position.x, transform.position.y, _position.z);
        Vector3 positionToGo = GetRandomPositionAroundAPoint(_position);

        NavMeshPath path = new NavMeshPath();
        if(m_navMeshAgent.CalculatePath(positionToGo, path)) {
            m_navMeshAgent.SetDestination(positionToGo);
            // m_navMeshAgent.SetPath(path);
        } else {
            Debug.LogWarning($"AI going to a Random Point on Nav Mesh!!");
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
