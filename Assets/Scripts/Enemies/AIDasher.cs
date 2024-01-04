using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIDasher : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public float startWaitTime = 4;
    public float timeToRotate = 2;
    public float dashDistance = 15;
    public float dashCooldown = 5;
    public float dashSpeed = 20;
    public float speedWalk = 6;
    public float speedRun = 9;
    public float viewRadius = 15;
    public float viewAngle = 360;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public Transform[] waypoints;
    int m_CurrentWaypointIndex;
    Vector3 playerLastPosition = Vector3.zero;
    Vector3 m_PlayerPosition;
    float m_WaitTime;
    float m_TimeToRotate;
    float m_DashCooldownTimer;
    bool m_PlayerInRange;
    bool m_IsChasing; // New variable to track chasing state
    bool m_CaughtPlayer;
    bool m_PlayerNear;

    void Start()
    {
        m_PlayerPosition = Vector3.zero;
        m_IsChasing = false;
        m_CaughtPlayer = false;
        m_PlayerInRange = false;
        m_WaitTime = startWaitTime;
        m_TimeToRotate = timeToRotate;
        m_DashCooldownTimer = 0;
        m_CurrentWaypointIndex = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;

        if (waypoints.Length > 0)
        {
            navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
        }
        else
        {
            Debug.LogWarning("No waypoints assigned to the AI controller.");
        }
    }

    void FixedUpdate()
    {
        EnvironmentView();

        if (m_IsChasing)
        {
            Chasing();
        }
        else
        {
            Patroling();
        }
    }

    private void Chasing()
    {
        m_PlayerNear = false;
        playerLastPosition = Vector3.zero;

        if (!m_CaughtPlayer)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, m_PlayerPosition);
            float minDistanceToPlayer = 3f;

            Debug.Log("Distance to player: " + distanceToPlayer);
            Debug.Log("Dash cooldown timer: " + m_DashCooldownTimer);

            if (distanceToPlayer > minDistanceToPlayer && m_DashCooldownTimer <= 0)
            {
                Debug.Log("Dashing!");
                Dash();
                m_DashCooldownTimer = dashCooldown;
            }
            else
            {
                // Move towards the player at run speed
                Move(speedRun);
                navMeshAgent.SetDestination(m_PlayerPosition);
            }
        }

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (!m_CaughtPlayer && Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 6f)
            {
                // Stop chasing and go back to patrol
                m_IsChasing = false;
                Move(speedWalk);
                m_DashCooldownTimer = 0;
                m_PlayerInRange = false;
                m_TimeToRotate = timeToRotate;
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
            }
        }
    }

    private void Dash()
    {
        Vector3 dashDirection = (m_PlayerPosition - transform.position).normalized;
        
        
        // Dash directly to the player's position
        Vector3 dashDestination = m_PlayerPosition;

        // Use MoveTowards in FixedUpdate for more consistent movement
        transform.position = Vector3.Lerp(transform.position, dashDestination, dashSpeed * Time.fixedDeltaTime);
    }

    private void Patroling()
    {
        if (m_PlayerInRange)
        {
            m_IsChasing = true;
            m_WaitTime = 0; // Reset wait time when transitioning to chase
            return; // Don't execute the rest of the patrolling logic
        }

        if (m_TimeToRotate <= 0)
        {
            Move(speedWalk);
            
        }
        else
        {
            Stop();
            m_TimeToRotate -= Time.fixedDeltaTime;
        }

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (m_WaitTime <= 0)
            {
                NextPoint();
                Move(speedWalk);
                m_WaitTime -= startWaitTime;
            }
            else
            {
                Stop();
                m_WaitTime -= Time.fixedDeltaTime;
            }
        }
    }

    void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }

    void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
    }

    public void NextPoint()
    {
        if (waypoints.Length > 0)
        {
            m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
            navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
        }
        else
        {
            Debug.LogWarning("No waypoints assigned to the AI controller.");
        }
    }

    void EnvironmentView()
    {
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);
        for (int i = 0; i < playerInRange.Length; i++)
        {
            Transform player = playerInRange[i].transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                float dstToPlayer = Vector3.Distance(transform.position, player.position);
                if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMask))
                {
                    m_PlayerInRange = true;

                    // Transition to chasing state immediately
                    m_IsChasing = true;
                    m_WaitTime = 0;

                    m_PlayerPosition = player.transform.position;
                }
                else
                {
                    m_PlayerInRange = false;
                }
            }
            if (Vector3.Distance(transform.position, player.position) > viewRadius)
            {
                m_PlayerInRange = false;
            }
        }

        // Update dash cooldown timer
        m_DashCooldownTimer = Mathf.Max(0, m_DashCooldownTimer - Time.fixedDeltaTime);
    }
}
