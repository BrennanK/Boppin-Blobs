using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour, IBoppable {
    [Header("AI Configuration")]
    public float aggroRange = 10f;

    private NavMeshAgent m_navMeshAgent;
    private Rigidbody m_rigibody;
    private Transform m_playerToRunFrom;
    private Vector3 m_playerAndAIDifference;

    // Variables for Tagging AI
    // TODO Cross-Reference, that's bad.
    private TaggingIdentifier m_taggingIdentifier;
    // private TaggingIdentifier[] m_notItPlayers;
    // private Transform m_playerCurrentlyBeingFollowed;
    private BehaviorTree.BehaviorTree m_behaviorTree;

    private void Start() {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_rigibody = GetComponent<Rigidbody>();
        m_taggingIdentifier = GetComponent<TaggingIdentifier>();
        m_playerToRunFrom = GameObject.FindGameObjectWithTag("Player").transform;
        m_rigibody.isKinematic = true;

        m_behaviorTree = new BehaviorTree.BehaviorTree(
            new BehaviorTreeBuilder()
                .Selector()
                    .Sequence()
                        .Condition("Check if is It", IsIt)
                    // insert behavior in case AI is it
                    .End()
                    // Search Power Ups ?
                    // Run from it
                    .Action("Run from It", RunFromIt)
                    // If everything else fails, just run randomly
                    .Action("Run Randomly", RunRandomly)
                .End()
                .Build()
            );

        InvokeRepeating("UpdateTree", 0f, .5f);
    }

    private void RunningFromItState() {
        m_playerAndAIDifference = m_playerToRunFrom.position - transform.position;
        Vector3 positionToMove = transform.position - m_playerAndAIDifference;

        if (m_playerToRunFrom != null && Vector3.Distance(transform.position, m_playerToRunFrom.position) < aggroRange) {
            NavMeshPath navmeshPath = new NavMeshPath();
            if (m_navMeshAgent.CalculatePath(positionToMove, navmeshPath)) {
                m_navMeshAgent.SetPath(navmeshPath);
            } else {
                transform.LookAt(m_playerToRunFrom.position);
                if(m_playerAndAIDifference.x > 0) {
                    m_navMeshAgent.destination = transform.position + (transform.forward + transform.right) * 5f;
                } else {
                    m_navMeshAgent.destination = transform.position + (transform.forward - transform.right) * 5f;
                }
            }
        }

        if (m_playerToRunFrom != null && Vector3.Distance(transform.position, m_playerToRunFrom.position) > aggroRange) {
            m_navMeshAgent.ResetPath();
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }

    #region IBoppable
    public bool HasAttacked() {
        return false;
    }

    public void TriggerAttackTransition() {
        return;
    }

    public void TriggerEndAttackTransition() {
        return;
    }

    public void TriggerIsTagTransition() {
        GetPlayerToFollow();
    }

    private void GetPlayerToFollow() {
        /*
        m_notItPlayers = FindObjectsOfType<TaggingIdentifier>();
        m_playerCurrentlyBeingFollowed = m_notItPlayers[0].transform;
        */

        /*
        for(int i = 1; i < m_notItPlayers.Length; i++) {
            if(Vector3.Distance(m_notItPlayers[i].transform.position, transform.position) < Vector3.Distance(m_playerCurrentlyBeingFollowed.position, transform.position) && m_notItPlayers[i].transform != transform) {
                m_playerCurrentlyBeingFollowed = m_notItPlayers[i].transform;
            }
        }
        */
    }

    public void TriggerIsNotTagTransition() {
        return;
    }

    public void UpdateWhoIsTag(Transform _whoIsTag) {
        m_playerToRunFrom = _whoIsTag;
    }

    public void DeactivateController() {
        m_navMeshAgent.enabled = false;
        if(m_navMeshAgent.isOnNavMesh) {
            m_navMeshAgent.ResetPath();
        }
    }

    public void ReactivateController() {
        m_navMeshAgent.enabled = true;
    }
    #endregion

    #region BEHAVIOR TREE ACTIONS
    private void UpdateTree() {
        m_behaviorTree.Update();
    }

    private EReturnStatus IsIt() {
        if(m_taggingIdentifier.AmITag()) {
            return EReturnStatus.SUCCESS;
        } else {
            return EReturnStatus.FAILURE;
        }
    }

    private EReturnStatus RunFromIt() {
        // TODO
        return EReturnStatus.FAILURE;
    }

    private EReturnStatus RunRandomly() {
        if(m_navMeshAgent.hasPath) {
            if(m_navMeshAgent.remainingDistance > 0.5f) {
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
}
