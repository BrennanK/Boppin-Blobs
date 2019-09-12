using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour, IBoppable {
    public enum EAIState {
        MonitoringIt,
        RunningFromIt,
        Tagging,
        Attacking,
        TemporarilyKnocked
    }

    [Header("AI Configuration")]
    public float aggroRange = 10f;

    private NavMeshAgent m_navMeshAgent;
    private Rigidbody m_rigibody;
    private Transform m_playerToRunFrom;
    private Vector3 m_playerAndAIDifference;
    private EAIState m_currentState;

    private void Start() {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_rigibody = GetComponent<Rigidbody>();
        m_currentState = EAIState.MonitoringIt;
        m_playerToRunFrom = GameObject.FindGameObjectWithTag("Player").transform;
        m_rigibody.isKinematic = true;
    }

    private void Update() {
        switch(m_currentState) {
            case EAIState.MonitoringIt:
                MonitoringItState();
                break;
            case EAIState.RunningFromIt:
                RunningFromItState();
                break;
        }

        Debug.DrawRay(transform.position, transform.forward * 2f, Color.cyan);
    }

    private void MonitoringItState() {
        transform.LookAt(m_playerToRunFrom.position);

        if(m_playerToRunFrom != null && Vector3.Distance(transform.position, m_playerToRunFrom.position) < aggroRange) {
            m_currentState = EAIState.RunningFromIt;
        }
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
            m_currentState = EAIState.MonitoringIt;
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

    public void UpdateWhoIsTag(Transform _whoIsTag) {
        m_playerToRunFrom = _whoIsTag;
    }

    public void DeactivateController() {
        m_navMeshAgent.ResetPath();
        m_navMeshAgent.enabled = false;
        m_currentState = EAIState.TemporarilyKnocked;
    }

    public void ReactivateController() {
        m_navMeshAgent.enabled = true;
        m_currentState = EAIState.RunningFromIt;
    }
    #endregion
}
