using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour, IBoppable {
    [Header("AI Configuration")]
    public float aggroRange = 10f;
    public float behaviorTreeRefreshRate = 0.01f;
    public float attackingDistance = 0.5f;
    private BehaviorTree.BehaviorTree m_behaviorTree;

    private NavMeshAgent m_navMeshAgent;
    private Rigidbody m_rigibody;
    private Transform m_playerToRunFrom;
    private Vector3 m_playerAndAIDifference;

    // Variables for Tagging AI
    // TODO Cross-Reference, that's bad.
    private TaggingIdentifier m_taggingIdentifier;
    private TaggingIdentifier[] m_notItPlayers;
    private Transform m_playerCurrentlyBeingFollowed;
    private bool m_isBeingKnockedBack = false;
    private bool m_canAttack = false;


    private void Start() {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_rigibody = GetComponent<Rigidbody>();
        m_taggingIdentifier = GetComponent<TaggingIdentifier>();
        m_playerToRunFrom = GameObject.FindGameObjectWithTag("Player").transform;
        m_rigibody.isKinematic = true;

        m_behaviorTree = new BehaviorTree.BehaviorTree(
            new BehaviorTreeBuilder()
                .Selector("AI Behavior Main Selector")
                    .Condition("Is Being Knocked Back", IsBeingKnockedBack)
                    .Sequence("Is it Sequence")
                        .Condition("Check if is it", IsIt)
                        .Action("Run from Everyone", RunAwayFromEveryone)
                    .End()
                    .Sequence("Is not it sequence")
                        .Condition("Has Player Available", HasPlayerAvailable)
                        .Selector("Attack or Follow")
                            .Sequence("Attack if possible")
                                .Condition("Check if within attacking distance", IsWithinAttackingDistance)
                                .Condition("Check if Player can attack", CanAttack)
                                .Action("Attack It", AttackNearestPlayer)
                            .End()
                            .Sequence("Run towards It")
                                    .Action("Run towards available player", RunTowardsPlayer)
                            .End()
                        .End()
                    .End()
                .End()
                .Build()
            );

        InvokeRepeating("UpdateTree", 0f, behaviorTreeRefreshRate);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }

    #region IBoppable
    public bool HasAttacked() {
        return m_canAttack;
    }

    public void TriggerAttackTransition() {
        m_canAttack = false;
    }

    public void TriggerEndAttackTransition() {
        return;
    }

    public void TriggerIsTagTransition() {
        // GetPlayerToFollow();
    }

    private void GetPlayerToFollow() {
        m_notItPlayers = m_taggingIdentifier.taggingManager.GetAllPlayersThatAreNotIt();
        m_playerCurrentlyBeingFollowed = m_notItPlayers[0].transform;

        for(int i = 1; i < m_notItPlayers.Length; i++) {
            if(Vector3.Distance(m_notItPlayers[i].transform.position, transform.position) < Vector3.Distance(m_playerCurrentlyBeingFollowed.position, transform.position) && m_notItPlayers[i].transform != transform) {
                m_playerCurrentlyBeingFollowed = m_notItPlayers[i].transform;
            }
        }
    }

    public void TriggerIsNotTagTransition() {
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

    #region Is It Functions
    private EReturnStatus IsIt() {
        if(m_taggingIdentifier.AmITag()) {
            return EReturnStatus.SUCCESS;
        } else {
            return EReturnStatus.FAILURE;
        }
    }

    private EReturnStatus IsBeingKnockedBack() {
        if (m_isBeingKnockedBack) {
            return EReturnStatus.SUCCESS;
        } else {
            return EReturnStatus.FAILURE;
        }
    }

    /// <summary>
    /// <para>Finds a point to run to.</para>
    /// <para>It is assumed the point maximizes the distance from all agents</para>
    /// </summary>
    /// <returns>SUCCESS if finds a path, FAILURE otherwise</returns>
    private EReturnStatus RunAwayFromEveryone() {
        if(RunAwayFromCenterOfMass()) {
            return EReturnStatus.SUCCESS;
        }

        return EReturnStatus.FAILURE;
    }

    private bool RunAwayFromCenterOfMass() {
        Debug.Log($"Trying to run...");
        return false;
    }

    private EReturnStatus IsWithinAttackingDistance() {
        if (Vector3.Distance(transform.position, m_playerCurrentlyBeingFollowed.position) < attackingDistance) {
            return EReturnStatus.SUCCESS;
        }

        return EReturnStatus.FAILURE;
    }

    private EReturnStatus CanAttack() {
        if(m_taggingIdentifier.TaggingState == TaggingIdentifier.ETaggingBehavior.TaggingAtacking) {
            return EReturnStatus.FAILURE;
        }

        return EReturnStatus.SUCCESS;
    }

    private EReturnStatus AttackNearestPlayer() {
        if (!m_canAttack) {
            transform.LookAt(m_playerCurrentlyBeingFollowed);
            m_canAttack = true;
            return EReturnStatus.SUCCESS;
        }

        return EReturnStatus.FAILURE;
    }

    private EReturnStatus HasPlayerAvailable() {
        if(m_playerCurrentlyBeingFollowed == null) {
            m_playerCurrentlyBeingFollowed = m_taggingIdentifier.taggingManager.GetItTransform();
            return EReturnStatus.FAILURE;
        }

        
        if(Vector3.Angle(transform.forward, m_playerCurrentlyBeingFollowed.position) > 135f) {
            transform.LookAt(m_playerCurrentlyBeingFollowed);
        }
        

        return EReturnStatus.SUCCESS;
    }

    private EReturnStatus RunTowardsPlayer() {
        if(m_playerCurrentlyBeingFollowed == null) {
            return EReturnStatus.FAILURE;
        }

        m_navMeshAgent.SetDestination(m_playerCurrentlyBeingFollowed.position);
        return EReturnStatus.RUNNING;
    }
    #endregion

    #region Is Not It Functions
    private EReturnStatus RunFromIt() {
        if(m_playerToRunFrom == null) {
            // TODO get player to run from
            return EReturnStatus.FAILURE;
        }

        m_playerAndAIDifference = m_playerToRunFrom.position - transform.position;
        Vector3 positionToMove = transform.position - m_playerAndAIDifference;

        if(Vector3.Distance(transform.position, m_playerToRunFrom.position) < aggroRange) {
            NavMeshPath path = new NavMeshPath();
            if(m_navMeshAgent.CalculatePath(positionToMove, path)) {
                m_navMeshAgent.SetPath(path);
            } else {
                transform.LookAt(m_playerToRunFrom.position);
                if(m_playerAndAIDifference.x > 0) {
                    m_navMeshAgent.destination = transform.position + ((transform.forward + transform.right) * 5f);
                } else {
                    m_navMeshAgent.destination = transform.position + ((transform.forward - transform.right) * 5f);
                }
            }

            return EReturnStatus.RUNNING;
        }

        if(Vector3.Distance(transform.position, m_playerToRunFrom.position) > aggroRange) {
            m_navMeshAgent.ResetPath();
        }

        return EReturnStatus.FAILURE;
    }

    private EReturnStatus LookAtIt() {
        if(m_playerToRunFrom == null) {
            m_playerToRunFrom = m_taggingIdentifier.taggingManager.GetItTransform();

            if(m_playerToRunFrom == null) {
                return EReturnStatus.FAILURE;
            }
        }

        transform.LookAt(m_playerToRunFrom.position);
        return EReturnStatus.SUCCESS;
    }

    private EReturnStatus RunRandomly() {
        if(m_navMeshAgent.hasPath) {
            if(m_navMeshAgent.remainingDistance > 0.75f) {
                return EReturnStatus.RUNNING;
            } else {
                m_navMeshAgent.ResetPath();
                return EReturnStatus.SUCCESS;
            }
        }

        // TODO get a random point in front of the player, unless it is blocked, which then should get a point backwards...
        Vector3 pointToMove = transform.position + (transform.forward + new Vector3(Random.value, 0f, Random.value));
        NavMeshPath pathToMove = new NavMeshPath();

        if (m_navMeshAgent.CalculatePath(pointToMove, pathToMove)) {
            m_navMeshAgent.SetPath(pathToMove);
            return EReturnStatus.RUNNING;
        }

        return EReturnStatus.FAILURE;
    }
    #endregion
    #endregion
}
