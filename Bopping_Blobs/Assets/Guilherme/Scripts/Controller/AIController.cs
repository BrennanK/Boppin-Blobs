using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour, IBoppable {
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
    private TaggingIdentifier[] m_notItPlayers;
    private Transform m_playerCurrentlyBeingFollowed;
    private bool m_isBeingKnockedBack = false;
    private bool m_canAttack = false;

    private void Start() {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_rigibody = GetComponent<Rigidbody>();
        m_taggingIdentifier = GetComponent<TaggingIdentifier>();
        m_rigibody.isKinematic = true;

        m_behaviorTree = new BehaviorTree.BehaviorTree(
            new BehaviorTreeBuilder()
                .Selector("AI Behavior Main Selector")
                    .Condition("Is Being Knocked Back", IsBeingKnockedBack)
                    .Sequence("Is IT Sequence")
                        .Condition("Check if is IT", IsIt)
                        // TODO "break run from everyone" into different tasks
                        // Run from closest player
                        // Run to random point (this can be biased or not)
                        .Action("Run from Everyone", RunAwayFromEveryone)
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
                                .Action("Run towards available player", RunTowardsPlayerToBeFollowed)
                            .End()
                        .End()
                    .End()
                .End()
                .Build()
            );

        InvokeRepeating("UpdateTree", Random.Range(minStartTime, maxStartTime), (baseReactionTime + Random.Range(-reactionTimeVariation, reactionTimeVariation)) );
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

    #region Is It Functions
    private EReturnStatus IsIt() {
        if(m_taggingIdentifier.AmITag()) {
            return EReturnStatus.SUCCESS;
        } else {
            return EReturnStatus.FAILURE;
        }
    }

    private EReturnStatus RunAwayFromEveryone() {
        if(RunAwayFromClosestPlayer()) {
            return EReturnStatus.RUNNING;
        }

        return EReturnStatus.FAILURE;
    }

    private bool RunAwayFromClosestPlayer() {
        if(m_notItPlayers == null) {
            m_notItPlayers = m_taggingIdentifier.taggingManager.GetAllPlayersThatAreNotIt();
        }

        // Getting closest player...
        Transform closestPlayer = m_notItPlayers[0].transform;
        for(int i = 1; i < m_notItPlayers.Length; i++) {
            if(Vector3.Distance(transform.position, m_notItPlayers[i].transform.position) < Vector3.Distance(transform.position, closestPlayer.position)) {
                closestPlayer = m_notItPlayers[i].transform;
            }
        }

        Vector3 positionToMove = transform.position - (closestPlayer.position - transform.position);
        NavMeshPath path = new NavMeshPath();
        if (m_navMeshAgent.CalculatePath(positionToMove, path)) {
            m_navMeshAgent.SetPath(path);
        } else {
            SetARandomPointOnMavMesh();
        }

        return true;
    }
    #endregion

    #region Is Not Functions
    private EReturnStatus HasPlayerToFollow() {
        if (m_playerCurrentlyBeingFollowed == null) {
            m_playerCurrentlyBeingFollowed = m_taggingIdentifier.taggingManager.GetItTransform();
            return EReturnStatus.FAILURE;
        }

        return EReturnStatus.SUCCESS;
    }

    private EReturnStatus RunTowardsPlayerToBeFollowed() {
        m_navMeshAgent.SetDestination(m_playerCurrentlyBeingFollowed.position);
        return EReturnStatus.RUNNING;
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
    // TODO Get a random point given two angles (input: I want to escape considering these two angles)
    // TODO Get a random point biased towards a point (input: "I WANT TO GO TO THIS DIRECTION, btw here's my position", output: "here's a point =))
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
