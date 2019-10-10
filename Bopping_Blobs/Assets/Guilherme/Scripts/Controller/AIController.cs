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
        NOT_KING_WANDERING,

        KING_SEARCHING_PATH,
        KING_FOLLOWING_PATH,
        KING_CHASING_POWERUP,
        KING_WANDERING,
    }

    enum EAIBehavior {
        TRY_HARD,
        COLLECTOR,
        BASELINE
    }

    private EAIStates m_currentState = EAIStates.NOT_KING_WANDERING;
    private EAIBehavior m_currentBehavior = EAIBehavior.BASELINE;

    [Header("Behavior Tree Configuration")]
    public float baseReactionTime = 0.35f;
    public float reactionTimeVariation = 0.05f;
    public float minStartTime = 0.15f;
    public float maxStartTime = 0.35f;
    private const float km_attackingDistance = 2.0f;
    private BehaviorTree.BehaviorTree m_behaviorTree;

    // General AI Configuration
    private const float km_wanderRange = 25f;
    private const float km_distanceToStopWandering = 0.5f;

    // Not King Configuration
    private float m_distanceToFollowKing = 15f;
    private float m_distanceToPowerUp = 15f;
    private const float km_closestPlayerDistanceToFollow = 15f;
    private float m_timeSinceLastAttack = 0f;
    private float m_chaoticTimeToAttackAgain;

    // King AI Configuration
    private const float km_iminentDangerDistance = 10f;


    private NavMeshAgent m_navMeshAgent;
    private Rigidbody m_rigibody;

    // Variables for Tagging AI
    private TaggingIdentifier m_taggingIdentifier;
    private Transform m_playerCurrentlyBeingFollowed;
    private PowerUpTracker m_powerUpTracker;
    private bool m_isBeingKnockedBack = false;
    private bool m_canAttack = false;
    private List<PowerUpBox> m_powerUpBoxesInDistance;
    private Animator m_animator;

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_distanceToFollowKing);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_distanceToPowerUp);
    }

    private void Awake() {
        m_animator = GetComponentInChildren<Animator>();
        m_powerUpBoxesInDistance = new List<PowerUpBox>();
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_rigibody = GetComponent<Rigidbody>();
        m_taggingIdentifier = GetComponent<TaggingIdentifier>();
        m_powerUpTracker = GetComponent<PowerUpTracker>();
        m_rigibody.isKinematic = true;
    }

    private void Start() {
        if(GameController.instance) {
            // give me a het >:L
            GameObject myHat = GameController.instance.allHatsPrefabs[Random.Range(0, GameController.instance.allHatsPrefabs.Length)];
            Instantiate(myHat, m_animator.transform);
        }
    }

    #region Initialize AI Functions
    public void MakeAIBaseline() {
        m_behaviorTree = new BehaviorTree.BehaviorTree(
          new BehaviorTreeBuilder()
              .Selector("AI Behavior Main Selector")
                  .Condition("Is Being Knocked Back", IsBeingKnockedBack)

                  // Is King
                  .Sequence("Is KING Sequence")
                      .Condition("Check if is King", IsKing)
                      .Selector("KING Selector")

                          .Sequence("Is on Iminent Danger")
                              .Condition("Check if is on iminent danger", IsOnIminentDanger)
                               .Selector("Try to Use Power Up to Run")
                                   .Action("Try to use Back off", UseBackOffPowerUp)
                                   .Action("Try to use Super Speed", UseSuperSlamPowerUp)
                                   .Condition("Tried all power ups?", TriedAllPowerUps)
                               .End()
                              .Action("Run away from closest Player", RunAwayFromClosestPlayer)
                          .End()

                          .Sequence("Collect Power Up")
                              .Condition("Can get power ups (power up tracker is not full", CanGetPowerUp)
                              .Condition("Is there a Power Up within distance", IsThereAPowerUpWithinDistance)
                              .Action("Collect a Power Up", CollectPowerUp)
                          .End()

                          .Sequence("King Wander Around")
                              .Action("King Wander!", KingWander)
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

                              .Sequence("Attack with Super Slam if possible")
                                  .Condition("Is Within Super Slam Distance?", IsWithinSuperSlamDistance)
                                  .Condition("SUPER SLAM", UseSuperSlamPowerUp)
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
                          .Condition("Can we wander?", CanNotKingWander)
                          .Action("Wander...", NotKingWanderRandomly)
                      .End()
                  .End()

              .End()
              .Build()
          );

        m_currentBehavior = EAIBehavior.BASELINE;
        InitializeAI();
    }

    public void MakePowerUpCollectorAI() {
        m_distanceToPowerUp *= 3f;

        MakeAIBaseline();
        m_currentBehavior = EAIBehavior.COLLECTOR;
        InitializeAI();
    }

    public void MakeTryHardAI() {
        m_distanceToFollowKing *= 3.5f;

        MakeAIBaseline();
        m_currentBehavior = EAIBehavior.TRY_HARD;
        InitializeAI();
    }

    private void InitializeAI() {
        StartCoroutine(UpdateTreeRoutine(Random.Range(minStartTime, maxStartTime)));
    }

    #endregion

    private void Update() {
        m_animator.SetFloat("MoveSpeed", m_navMeshAgent.velocity.normalized.magnitude);
        transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, 0f);
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

    public void DeactivateController(bool _updateAnimation = false) {
        if(_updateAnimation) {
            m_animator.SetBool("Hit", true);
        }

        m_isBeingKnockedBack = true;
        m_navMeshAgent.enabled = false;

        if(m_navMeshAgent.isOnNavMesh) {
            m_navMeshAgent.ResetPath();
        }
    }

    public void ReactivateController() {
        m_animator.SetBool("Hit", false);
        m_isBeingKnockedBack = false;
        m_navMeshAgent.enabled = true;
    }
    #endregion

    #region BEHAVIOR TREE ACTIONS
    private IEnumerator UpdateTreeRoutine(float _delay) {
        yield return new WaitForSeconds(_delay);
        m_timeSinceLastAttack += _delay;
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
    private EReturnStatus IsKing() {
        if(m_taggingIdentifier.AmIKing()) {
            return EReturnStatus.SUCCESS;
        } else {
            return EReturnStatus.FAILURE;
        }
    }

    private EReturnStatus KingWander() {
        bool recalculateWandering = true;

        if(m_currentState == EAIStates.KING_WANDERING) {
            if(m_navMeshAgent.remainingDistance > km_distanceToStopWandering) {
                recalculateWandering = false;
            }
        }

        if(recalculateWandering) {
            if(SetARandomPointOnMavMesh(km_wanderRange)) {
                m_currentState = EAIStates.KING_WANDERING;
            } else {
                m_currentState = EAIStates.KING_SEARCHING_PATH;
            }
        }

        return EReturnStatus.SUCCESS;
    }

    private EReturnStatus IsOnIminentDanger() {
        Vector3 closestPlayer = GetClosestPlayerTransform().position;
        if(Vector3.Distance(transform.position, closestPlayer) < km_iminentDangerDistance) {
            return EReturnStatus.SUCCESS;
        }

        return EReturnStatus.FAILURE;
    }

    private EReturnStatus UseBackOffPowerUp() {
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

    private EReturnStatus UseSuperSlamPowerUp() {
        if(m_powerUpTracker.slot1.powerUp != null) {
            if(m_powerUpTracker.slot1.powerUp.powerUp == EPowerUps.SUPER_SLAM) {
                m_powerUpTracker.ActivatePowerUp1();
                return EReturnStatus.SUCCESS;
            }
        } else if(m_powerUpTracker.slot2.powerUp != null) {
            if(m_powerUpTracker.slot2.powerUp.powerUp == EPowerUps.SUPER_SLAM) {
                m_powerUpTracker.ActivatePowerUp2();
                return EReturnStatus.SUCCESS;
            }
        }

        return EReturnStatus.FAILURE;
    }

    private EReturnStatus TriedAllPowerUps() {
        return EReturnStatus.SUCCESS;
    }

    private EReturnStatus RunAwayFromClosestPlayer() {
        if(m_currentState == EAIStates.KING_FOLLOWING_PATH) {
            if(m_navMeshAgent.remainingDistance > km_distanceToStopWandering * 2f) {
                return EReturnStatus.SUCCESS;
            } else {
                m_currentState = EAIStates.KING_SEARCHING_PATH;
            }
        }

        bool someoneInFrontOfMe = Physics.Raycast(transform.position, transform.forward, km_iminentDangerDistance, m_taggingIdentifier.attackLayer);
        bool someoneBehindMe = Physics.Raycast(transform.position, -transform.forward, km_iminentDangerDistance, m_taggingIdentifier.attackLayer);
        bool someoneToMyRight = Physics.Raycast(transform.position, transform.right, km_iminentDangerDistance, m_taggingIdentifier.attackLayer);
        bool someoneToMyLeft = Physics.Raycast(transform.position, -transform.right, km_iminentDangerDistance, m_taggingIdentifier.attackLayer);

        List<Vector3> possibleDirections = new List<Vector3> {
            transform.forward,
            -transform.forward,
            transform.right,
            -transform.right
        };

        if(someoneInFrontOfMe) {
            possibleDirections.Remove(transform.forward);
        }

        if(someoneBehindMe) {
            possibleDirections.Remove(-transform.forward);
        }

        if(someoneToMyRight) {
            possibleDirections.Remove(transform.right);
        }

        if(someoneToMyLeft) {
            possibleDirections.Remove(-transform.right);
        }

        if(possibleDirections.Count > 0) {
            RunToThisDirection(possibleDirections[Random.Range(0, possibleDirections.Count)]);
            m_currentState = EAIStates.KING_FOLLOWING_PATH;
            return EReturnStatus.SUCCESS;
        }

        return KingWander();
    }

    private void RunToThisDirection(Vector3 _direction) {
        SetARandomPointOnThatDirection(_direction);
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

    private EReturnStatus CanChaoticAttack() {
        if((m_timeSinceLastAttack >= m_chaoticTimeToAttackAgain)) {
            return EReturnStatus.SUCCESS;
        } else {
            return EReturnStatus.FAILURE;
        }
    }

    private EReturnStatus CanNotKingWander() {
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

    private EReturnStatus NotKingWanderRandomly() {
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
            if(Vector3.Distance(transform.position, box.gameObject.transform.position) < m_distanceToPowerUp && box.IsActive) {
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

            if (!m_taggingIdentifier.AmIKing()) {
                m_currentState = EAIStates.NOT_KING_CHASING_POWERUP;
            } else {
                m_currentState = EAIStates.KING_CHASING_POWERUP;
            }

            return EReturnStatus.SUCCESS;
        }

        return EReturnStatus.FAILURE;
    }
    #endregion

    #region Attacking Related (can be used on is it or is not it)
    private EReturnStatus CheckIfClosestPlayerIsWithinAttackingDistance() {
        Transform closestPlayer = GetClosestPlayerTransform();
        if(Vector3.Distance(transform.position, closestPlayer.position) < km_attackingDistance) {
            return EReturnStatus.SUCCESS;
        }

        return EReturnStatus.FAILURE;
    }

    private EReturnStatus IsWithinSuperSlamDistance() {
        if(Vector3.Distance(transform.position, m_playerCurrentlyBeingFollowed.position) < (km_attackingDistance * 2f)) {
            return EReturnStatus.SUCCESS;
        }

        return EReturnStatus.FAILURE;
    }

    private EReturnStatus IsWithinAttackingDistance() {
        if (Vector3.Distance(transform.position, m_playerCurrentlyBeingFollowed.position) < km_attackingDistance) {
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
            transform.LookAt(m_playerCurrentlyBeingFollowed);
            m_canAttack = true;
            m_timeSinceLastAttack = 0f;
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

    private bool SetARandomPointOnThatDirection(Vector3 _direction) {
        for(int i = 0; i < 5; i++) {
            Vector3 randomPoint = transform.position + (_direction * km_wanderRange);
            NavMeshHit hit;

            if(NavMesh.SamplePosition(randomPoint, out hit, 5.0f, NavMesh.AllAreas)) {
                NavMeshPath path = new NavMeshPath();
                if(NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path)) {
                    Debug.DrawLine(transform.position, hit.position, Color.red, 1f);
                    m_navMeshAgent.SetPath(path);
                    return true;
                } else {
                    Debug.LogWarning($"AI there's no Path!");
                }
            } else {
                randomPoint = transform.position + (_direction * (km_wanderRange / 4.0f));
                if(NavMesh.SamplePosition(randomPoint, out hit, 5.0f, NavMesh.AllAreas)) {
                    NavMeshPath path = new NavMeshPath();
                    if(NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path)) {
                        m_navMeshAgent.SetPath(path);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool SetARandomPointOnMavMesh(float _range) {
        for(int i = 0; i < 10; i++) {
            Vector3 randomPoint = transform.position + (Random.insideUnitSphere * _range);
            randomPoint.y = 0;
            NavMeshHit hit;

            if(NavMesh.SamplePosition(randomPoint, out hit, 2.5f, NavMesh.AllAreas)) {
                NavMeshPath path = new NavMeshPath();
                if(NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path)) {
                    Debug.DrawLine(transform.position, hit.position, Color.green, 1f);
                    m_navMeshAgent.SetPath(path);
                    return true;
                } else {
                    Debug.LogWarning($"AI There's no path!!");
                }
            } else {
                Debug.LogWarning($"AI there's no sample position");
            }
        }

        return false;
    }

    #endregion
}
