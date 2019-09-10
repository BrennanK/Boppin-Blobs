using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {
    public enum EAIState {
        MonitoringPlayer,
        RunningFromPlayer,
        TemporarilyKnocked
    }

    [Header("AI Configuration")]
    public float aggroRange = 10f;

    private NavMeshAgent m_navMeshAgent;
    private Rigidbody m_rigibody;
    private Transform m_playerReference;
    private Vector3 m_playerAndAIDifference;
    private EAIState m_currentState;

    private void Start() {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_rigibody = GetComponent<Rigidbody>();
        m_playerReference = GameObject.FindGameObjectWithTag("Player").transform;
        m_currentState = EAIState.MonitoringPlayer;

        m_rigibody.isKinematic = true;
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
                m_navMeshAgent.SetPath(navmeshPath);
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

    public void AIWasTagged(Color _colorToRender, Vector3 _knockbackDirection) {
        GetComponent<Renderer>().material.color = _colorToRender;
        m_navMeshAgent.ResetPath();
        m_navMeshAgent.enabled = false;
        m_rigibody.isKinematic = false;
        m_rigibody.velocity = _knockbackDirection * 25f;
        m_currentState = EAIState.TemporarilyKnocked;
        StartCoroutine(KnockbackWait());
    }

    private IEnumerator KnockbackWait() {
        yield return new WaitForSeconds(1.0f);
        GetComponent<Renderer>().material.color = Color.cyan;
        m_rigibody.isKinematic = true;
        m_navMeshAgent.enabled = true;
        m_currentState = EAIState.RunningFromPlayer;
    }
}
