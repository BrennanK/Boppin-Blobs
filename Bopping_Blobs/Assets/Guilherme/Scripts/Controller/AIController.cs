using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {
    public enum EAIState {
        MonitoringPlayer,
        RunningFromPlayer
    }


    [Header("AI Configuration")]
    public float aggroRange = 10f;

    private NavMeshAgent m_navMeshAgent;
    private Transform m_playerReference;
    private Vector3 m_playerAndAIDifference;
    private EAIState m_currentState;

    private void Start() {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_playerReference = GameObject.FindGameObjectWithTag("Player").transform;
        m_currentState = EAIState.MonitoringPlayer;
    }

    private void Update() {
        switch(m_currentState) {
            case EAIState.MonitoringPlayer:
                MonitoringPlayerState();
                break;
            case EAIState.RunningFromPlayer:
                RunningFromPlayerState();
                break;
        }

        Debug.DrawRay(transform.position, transform.forward * 2f, Color.cyan);
        Debug.DrawRay(transform.position, transform.right * 2f, Color.cyan);
        Debug.DrawRay(transform.position, -transform.right * 2f, Color.cyan);
        Debug.DrawRay(transform.position, -transform.forward * 2f, Color.cyan);
    }

    private void MonitoringPlayerState() {
        transform.LookAt(m_playerReference.position);

        if(m_playerReference != null && Vector3.Distance(transform.position, m_playerReference.position) < aggroRange) {
            m_currentState = EAIState.RunningFromPlayer;
        }
    }

    private void RunningFromPlayerState() {
        m_playerAndAIDifference = m_playerReference.position - transform.position;
        Vector3 positionToMove = transform.position - m_playerAndAIDifference;

        if (m_playerReference != null && Vector3.Distance(transform.position, m_playerReference.position) < aggroRange) {
            NavMeshPath navmeshPath = new NavMeshPath();

            if (m_navMeshAgent.CalculatePath(positionToMove, navmeshPath)) {
                m_navMeshAgent.destination = positionToMove;
            } else {
                transform.LookAt(m_playerReference.position);
                if(m_playerAndAIDifference.x > 0) {
                    m_navMeshAgent.destination = transform.position + (transform.forward + transform.right) * 5f;
                } else {
                    m_navMeshAgent.destination = transform.position + (transform.forward - transform.right) * 5f;
                }
            }
        }

        if (m_playerReference != null && Vector3.Distance(transform.position, m_playerReference.position) > aggroRange) {
            m_navMeshAgent.ResetPath();
            m_currentState = EAIState.MonitoringPlayer;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
