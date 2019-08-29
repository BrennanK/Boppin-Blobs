using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCContoller : MonoBehaviour
{
    public float patrolTime = 10;
    public float aggroRange = 10;
    //public Transform[] waypoints;

    int index;
    float speed, agentSpeed;
    [SerializeField]Transform player;

    NavMeshAgent agent;
    public Vector3 playerPosition;
    public float x;
    public float z;
    public float sinAngle;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agentSpeed = agent.speed;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //index = Random.Range(0, waypoints.Length);

        
        InvokeRepeating("Tick", 0, 0.5f);
        /*
        if (waypoints.Length > 0)
        {
            InvokeRepeating("Patrol", Random.Range(0, patrolTime), patrolTime);
        }
        */
    }

    void Update()
    {
        speed = Mathf.Lerp(speed, agent.velocity.magnitude, Time.deltaTime * 10);
        playerPosition = player.position - transform.position;
        x = playerPosition.x;
        z = playerPosition.z;
        sinAngle = Mathf.Sin(x / z);
    }

    void Patrol()
    {
        //index = index == waypoints.Length - 1 ? 0 : index + 1;
    }

    void Tick()
    {
        //agent.destination = waypoints[index].position;
        agent.speed = agentSpeed / 2;
        if (player != null && Vector3.Distance(transform.position, player.transform.position) < 2* aggroRange)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < aggroRange)
            {
                agent.speed = agentSpeed;
            }
            agent.destination = new Vector3(transform.position.x - x, transform.position.y, transform.position.z - z);
        }
        /*
        else if (player != null && Vector3.Distance(transform.position, player.transform.position) < aggroRange)
        {
            agent.speed = agentSpeed;
            //agent.destination = player.position;
            agent.destination = new Vector3(transform.position.x-x,transform.position.y,transform.position.z-z);
        }       
        */
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
