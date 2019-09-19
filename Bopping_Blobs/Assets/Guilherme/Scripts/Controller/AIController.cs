using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour, IBoppable {
    [Header("AI Configuration")]
    public float behaviorTreeRefreshRate = 0.01f;
    public float attackingDistance = 0.5f;
    private BehaviorTree.BehaviorTree m_behaviorTree;

    private NavMeshAgent m_navMeshAgent;
    private Rigidbody m_rigibody;
    private Vector3 m_playerAndAIDifference;

    // Variables for Tagging AI
    // TODO Cross-Reference, that's bad.
    private TaggingIdentifier m_taggingIdentifier;
    private TaggingIdentifier[] m_notItPlayers;
    private Transform m_playerToRunFrom;
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
                        .Action("Run from Everyone", RunAwayFromEveryone)
                    .End()
                    .Sequence("Is not IT sequence")
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
        return;
    }

    /// <summary>
    /// <para>Fill the m_notItPlayers vector and set the playerCurrentlyBeingFollowed to the closest player</para>
    /// </summary>
    
    // TODO
    // This functions does 2 things and has a behavior that misleads the programmer, fix it
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

    /// <summary>
    /// <para>Finds a point to run to.</para>
    /// <para>It is assumed the point maximizes the distance from all agents</para>
    /// </summary>
    /// <returns>SUCCESS if finds a path, FAILURE otherwise</returns>
    private EReturnStatus RunAwayFromEveryone() {
        /*
         * Running Away from Center of Mass is pretty bad
        if(RunAwayFromCenterOfMass()) {
            return EReturnStatus.SUCCESS;
        }
        */

        if(RunAwayFromClosestPlayer()) {
            return EReturnStatus.RUNNING;
        }

        /*
        if(RunAwayInASmartWay()) {
            return EReturnStatus.RUNNING;
        }
        */

        return EReturnStatus.FAILURE;
    }

    private bool RunAwayInASmartWay() {
        // 1. Get Possible Running Directions considering closest enemy
        if (m_notItPlayers == null) {
            GetPlayerToFollow();
        }

        // Getting closest player...
        Transform closestPlayer = m_notItPlayers[0].transform;
        for (int i = 1; i < m_notItPlayers.Length; i++) {
            if (Vector3.Distance(transform.position, m_notItPlayers[i].transform.position) < Vector3.Distance(transform.position, closestPlayer.position)) {
                closestPlayer = m_notItPlayers[i].transform;
            }
        }

        // Vector from AI to Threat
        m_playerAndAIDifference = closestPlayer.position - transform.position;
        List<Vector3> possibleRunawayDirections = new List<Vector3> {
            -m_playerAndAIDifference.normalized,
            transform.right,
            -transform.right,
        };

        Debug.Log($"Trying to run smart...");

        float distanceToCheckCollision = 3f;
        float distanceMultiplier = 2f;
        Vector3 chosenDirection = Vector3.zero;

        foreach (Vector3 direction in possibleRunawayDirections) {
            RaycastHit[] allHits = Physics.RaycastAll(transform.position, direction * distanceToCheckCollision);

            foreach(RaycastHit hit in allHits) {
                Debug.Log($"Hit: {hit.transform.name}");
            }

            if (allHits.Length == 1 && allHits[0].transform == this.transform) {
                // that's me!
                Debug.DrawRay(transform.position, direction * distanceToCheckCollision, Color.green);
                chosenDirection = direction;
                break;
            }
        }

        if(chosenDirection == Vector3.zero) {
            return false;
        }

        m_navMeshAgent.SetDestination(transform.position + (chosenDirection * distanceMultiplier));
        // (Optional) 2. Eliminate possible running directions considering second, third, ..., closest enemies
        // 3. Raycast on that direction, if there is something, choose another direction
        return true;
    }

    private bool RunAwayFromClosestPlayer() {
        if(m_notItPlayers == null) {
            GetPlayerToFollow();
        }

        // Getting closest player...
        Transform closestPlayer = m_notItPlayers[0].transform;
        for(int i = 1; i < m_notItPlayers.Length; i++) {
            if(Vector3.Distance(transform.position, m_notItPlayers[i].transform.position) < Vector3.Distance(transform.position, closestPlayer.position)) {
                closestPlayer = m_notItPlayers[i].transform;
            }
        }

        m_playerAndAIDifference = closestPlayer.position - transform.position;
        Vector3 positionToMove = transform.position - m_playerAndAIDifference;
        NavMeshPath path = new NavMeshPath();

        if (m_navMeshAgent.CalculatePath(positionToMove, path)) {
            m_navMeshAgent.SetPath(path);
        } else {
            SetARandomPointOnMavMesh();
        }

        return true;
    }

    private bool RunAwayFromCenterOfMass() {
        if(m_notItPlayers == null) {
            GetPlayerToFollow();
        }

        if(m_notItPlayers.Length == 0) {
            return false;
        }

        Vector3 runPosition = (m_notItPlayers[0].transform.position - transform.position);

        for(int i = 1; i < m_notItPlayers.Length; i++) {
            runPosition += (m_notItPlayers[i].transform.position - transform.position);
        }

        m_navMeshAgent.SetDestination(transform.position + runPosition);
        return true;
    }
    #endregion

    #region Is Not Functions
    private EReturnStatus HasPlayerAvailable() {
        if (m_playerCurrentlyBeingFollowed == null) {
            m_playerCurrentlyBeingFollowed = m_taggingIdentifier.taggingManager.GetItTransform();
            return EReturnStatus.FAILURE;
        }

        if (Vector3.Angle(transform.forward, m_playerCurrentlyBeingFollowed.position) > 135f) {
            transform.LookAt(m_playerCurrentlyBeingFollowed);
        }

        return EReturnStatus.SUCCESS;
    }

    private EReturnStatus RunTowardsPlayer() {
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
            transform.LookAt(m_playerCurrentlyBeingFollowed);
            m_canAttack = true;
            return EReturnStatus.SUCCESS;
        }

        return EReturnStatus.FAILURE;
    }
    #endregion

    #endregion

    #region Helper Functions
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
