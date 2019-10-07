using BehaviorTree;
using PowerUp;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour, IBoppable {
    enum EAIStates {
        NOT_KING_RECALCULATING_PATH,
        NOT_KING_CHASING_KING,
        NOT_KING_CHASING_POWERUP,
        NOT_KING_FOLLOWING_PLAYER,
        NOT_KING_WANDERING,
        KING_SEARCHING_PATH,
        KING_FOLLOWING_PATH,
        KING_FOLLOWING_RANDOM_PATH,
        KING_FOLLOWING_PREFERRED_PATH,
    }

    private EAIStates m_currentState = EAIStates.NOT_KING_WANDERING;

    [Header("Behavior Tree Configuration")]
    public float baseReactionTime = 0.35f;
    public float reactionTimeVariation = 0.05f;
    public float minStartTime = 0.15f;
    public float maxStartTime = 0.35f;
    public float attackingDistance = 1.5f;
    private BehaviorTree.BehaviorTree m_behaviorTree;

    // General AI Configuration
    private const float km_wanderRange = 25f;
    private const float km_distanceToStopWandering = 0.5f;

    // Not King Configuration
    public float m_distanceToFollowKing = 15f;
    public float DistanceToFollowKing {
        set {
            m_distanceToFollowKing = value;
        }
    }
    public const float km_distanceToPowerUp = 10f;
    public const float km_closestPlayerDistanceToFollow = 15f;

    // King AI Configuration
    private const float km_iminentDangerDistance = 5f;


    private NavMeshAgent m_navMeshAgent;
    private Rigidbody m_rigibody;

    // Variables for Tagging AI
    private TaggingIdentifier m_taggingIdentifier;
    private Transform m_playerCurrentlyBeingFollowed;
    private PowerUpTracker m_powerUpTracker;
    private bool m_isBeingKnockedBack = false;
    private bool m_canAttack = false;
    private List<PowerUpBox> m_powerUpBoxesInDistance;

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_distanceToFollowKing);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, km_distanceToPowerUp);
    }

    private void Awake() {
        m_powerUpBoxesInDistance = new List<PowerUpBox>();
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_rigibody = GetComponent<Rigidbody>();
        m_taggingIdentifier = GetComponent<TaggingIdentifier>();
        m_powerUpTracker = GetComponent<PowerUpTracker>();
        m_rigibody.isKinematic = true;
    }

    private void Start() {
        m_behaviorTree = new BehaviorTree.BehaviorTree(
           new BehaviorTreeBuilder()
               .Selector("AI Behavior Main Selector")
                   .Condition("Is Being Knocked Back", IsBeingKnockedBack)

                   // Is King
                   .Sequence("Is KING Sequence")
                       .Condition("Check if is IT", IsIt)

                       .Selector("is King Selector - Select one of these actions to use to run away")
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
                               .Sequence("Collect Power Up")
                                   .Condition("Can get power ups", CanGetPowerUp)
                                   .Condition("Is there a Power Up within distance", IsThereAPowerUpWithinDistance)
                                   .Action("Collect a Power Up", CollectPowerUp)
                               .End()
                           .End()
                       .End()

                   .End()

                   // IS NOT KING
                   .Selector("Is NOT King Selector")

                       .Sequence("Chase King")
                           .Condition("Is King Withing Follow Distance", IsKingWithinFollowDistance)
                           .Selector("AI Attack or Follow")
                               .Sequence("Attack if possible")
                                   .Condition("Check if is within Attacking Distance", IsWithinAttackingDistance)
                                   .Condition("Check if AI can attack", CanAttack)
                                   .Action("Attack King", Attack)
                               .End()
                               .Sequence("Chase King")
                                   .Action("Chase King", ChaseKing)
                               .End()
                           .End()
                       .End()

                       .Sequence("Collect Power Up")
                           .Condition("Can get power ups (power up tracker is not full", CanGetPowerUp)
                           .Condition("Is there a Power Up within distance", IsThereAPowerUpWithinDistance)
                           .Action("Collect a Power Up", CollectPowerUp)
                       .End()

                       .Sequence("I dunno man, I like my own space")
                           .Condition("Check if someone is REALLY close", CheckIfClosestPlayerIsWithinAttackingDistance)
                           .Condition("Checking if I can attack", CanAttack)
                           .Action("Attack!!", Attack)
                       .End()

                       .Sequence("Walk Randomly")
                           .Condition("Can we wander?", CanPlayerWander)
                           .Action("Wander...", WanderRandomly)
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

    public void ChangeSpeed(float _baseSpeed, float _tempSpeedBost, float _externalSpeedBoost) {
        m_navMeshAgent.speed = _baseSpeed + _tempSpeedBost + _externalSpeedBoost;
        m_navMeshAgent.acceleration = m_navMeshAgent.speed * 2f;
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
        Vector3 closestPlayer = GetClosestPlayerTransform().position;
        if(Vector3.Distance(transform.position, closestPlayer) < km_iminentDangerDistance) {
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
        if(m_currentState == EAIStates.NOT_KING_CHASING_KING) {
            // AI is King but is Chasing King? AI just got King!
            m_currentState = EAIStates.KING_SEARCHING_PATH;
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
        Vector3 closestPlayerPosition = GetClosestPlayerTransform().position;
        Vector3 directionToMove = transform.position - closestPlayerPosition;
        directionToMove.Scale(new Vector3(3, 3, 3));

        Debug.DrawRay(transform.position, directionToMove, Color.red, 0.125f);
        SetPathToAgentFromPosition(directionToMove);
        m_currentState = EAIStates.KING_FOLLOWING_PATH;

        return EReturnStatus.RUNNING;
    }
    #endregion

    #region Is Not King Functions
    private EReturnStatus IsKingWithinFollowDistance() {
        Transform kingTransform = m_taggingIdentifier.taggingManager.KingTransform;
        if(Vector3.Distance(transform.position, kingTransform.position) < m_distanceToFollowKing) {
            m_playerCurrentlyBeingFollowed = kingTransform;
            return EReturnStatus.SUCCESS;
        }

        return EReturnStatus.FAILURE;
    }

    private EReturnStatus ChaseKing() {
        if(m_navMeshAgent.isOnNavMesh) {
            m_navMeshAgent.SetDestination(m_playerCurrentlyBeingFollowed.position + m_playerCurrentlyBeingFollowed.forward);
            m_currentState = EAIStates.NOT_KING_CHASING_KING;
            return EReturnStatus.SUCCESS;
        }

        return EReturnStatus.FAILURE;
    }

    private EReturnStatus CanPlayerWander() {
        if(m_currentState == EAIStates.NOT_KING_WANDERING) {
            // we are wandering already, so we check if we are close to the point we were previosuly wandering too...
            if(m_navMeshAgent.remainingDistance < km_distanceToStopWandering) {
                // we are very close, so we can wander again!
                return EReturnStatus.SUCCESS;
            } else {
                // just keep wandering...
                return EReturnStatus.FAILURE;
            }
        }

        // if we are not wandering and we got here it is because king or power ups is not close :( so we can wander!
        return EReturnStatus.SUCCESS;
    }

    private EReturnStatus WanderRandomly() {
        if(SetARandomPointOnMavMesh(km_wanderRange)) {
            m_currentState = EAIStates.NOT_KING_WANDERING;
            return EReturnStatus.SUCCESS;
        }

        m_currentState = EAIStates.NOT_KING_RECALCULATING_PATH;
        return EReturnStatus.SUCCESS;
    }
    #endregion

    #region Power Up Related Functions
    private EReturnStatus CanGetPowerUp() {
        if(m_powerUpTracker.AreSlotsFull) {
            return EReturnStatus.FAILURE;
        }

        return EReturnStatus.SUCCESS;
    }

    private EReturnStatus IsThereAPowerUpWithinDistance() {
        PowerUpBox[] allPowerupBoxes = GameController.instance.PowerupBoxes;
        m_powerUpBoxesInDistance.Clear();

        foreach(PowerUpBox box in allPowerupBoxes) {
            if(Vector3.Distance(transform.position, box.gameObject.transform.position) < km_distanceToPowerUp && box.IsActive) {
                m_powerUpBoxesInDistance.Add(box);
            }
        }

        if(m_powerUpBoxesInDistance.Count > 0) {
            return EReturnStatus.SUCCESS;
        } else {
            return EReturnStatus.FAILURE;
        }
    }

    private EReturnStatus CollectPowerUp() {
        if(m_navMeshAgent.isOnNavMesh) {
            m_navMeshAgent.SetDestination(m_powerUpBoxesInDistance[Random.Range(0, m_powerUpBoxesInDistance.Count)].gameObject.transform.position);

            if (!m_taggingIdentifier.AmITag()) {
                m_currentState = EAIStates.NOT_KING_CHASING_POWERUP;
            }

            return EReturnStatus.SUCCESS;
        }

        return EReturnStatus.FAILURE;
    }
    #endregion

    #region Attacking Related (can be used on is it or is not it)
    private EReturnStatus CheckIfClosestPlayerIsWithinAttackingDistance() {
        Transform closestPlayer = GetClosestPlayerTransform();
        if(Vector3.Distance(transform.position, closestPlayer.position) < attackingDistance) {
            return EReturnStatus.SUCCESS;
        }

        return EReturnStatus.FAILURE;
    }

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

    private EReturnStatus Attack() {
        if (!m_canAttack) {
            m_canAttack = true;
            return EReturnStatus.SUCCESS;
        }

        return EReturnStatus.FAILURE;
    }
    #endregion

    #endregion

    #region Helper Functions
    private Transform GetClosestPlayerTransform() {
        List<TaggingIdentifier> notKingPlayers = m_taggingIdentifier.taggingManager.GetAllPlayersThatAreNotIt().ToList();
        notKingPlayers.Remove(m_taggingIdentifier);

        Transform closestPlayer = notKingPlayers[0].transform;
        for (int i = 1; i < notKingPlayers.Count; i++) {
            if (Vector3.Distance(transform.position, notKingPlayers[i].transform.position) < Vector3.Distance(transform.position, closestPlayer.position)) {
                closestPlayer = notKingPlayers[i].transform;
            }
        }

        return closestPlayer;
    }

    private void SetPathToAgentFromPosition(Vector3 _position) {
        _position = new Vector3(_position.x, transform.position.y, _position.z);
        Vector3 positionToGo = GetRandomPositionAroundAPoint(_position);
        m_navMeshAgent.SetDestination(positionToGo);
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

    private bool SetARandomPointOnMavMesh(float _range) {
        for(int i = 0; i < 10; i++) {
            Vector3 randomPoint = transform.position + (Random.insideUnitSphere * _range);
            randomPoint.y = 0;
            NavMeshHit hit;

            if(NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) {
                m_navMeshAgent.SetDestination(hit.position);
                return true;
            }
        }

        return false;
    }
    #endregion
}
