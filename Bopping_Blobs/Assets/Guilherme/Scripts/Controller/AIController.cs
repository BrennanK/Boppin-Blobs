using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {
    [Header("AI Configuration")]
    public float aggroRange = 10f;

    private NavMeshAgent m_navMeshAgent;
    private Transform m_playerReference;
    private Vector3 m_playerAndAIDifference;

    private void Start() {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_playerReference = GameObject.FindGameObjectWithTag("Player").transform;

        InvokeRepeating("TickAI", 0f, 0.25f);
    }

    private void Update() {
        m_playerAndAIDifference = m_playerReference.position - transform.position;

        if (m_playerReference != null && Vector3.Distance(transform.position, m_playerReference.position) < 2 * aggroRange) {
            m_navMeshAgent.destination = new Vector3(transform.position.x - m_playerAndAIDifference.x + Random.value, transform.position.y, transform.position.z - m_playerAndAIDifference.z + Random.value);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
