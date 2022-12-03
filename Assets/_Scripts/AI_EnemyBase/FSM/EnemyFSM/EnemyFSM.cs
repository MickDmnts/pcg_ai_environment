using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

/* [CLASS DOCUMENTATION]
 * 
 * This class file derives from FSM and caches all the inspector given values of the EnemyBehaviour.
 * Acts as container and manager for the enemy states.
 * 
 */

public class EnemyFSM : FSM
{
    public float idleTime;
    public List<Transform> waypoints;
    public float waypointDistanceOffset;

    public Transform target;
    public Transform enemy;
    public Vector3 originalPos;

    public NavMeshAgent agent;
    public Rigidbody agentRB;
    public EnemyBehaviour enemyBehaviour;

    //Enemy states
    public EnemyIdleState IdleState;
    public EnemyPatrolState PatrolState;
    public EnemyChaseState ChaseState;
    public EnemyBackToOriginState BackToOrigin;

    public EnemyFSM(float idleTime, List<Transform> waypoints, float waypointDistanceOffset, Transform target,
        Transform enemy)
    {
        this.idleTime = idleTime;
        this.waypoints = waypoints;
        this.waypointDistanceOffset = waypointDistanceOffset;
        this.target = target;
        this.enemy = enemy;

        agent = enemy.GetComponent<NavMeshAgent>();
        enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();

        this.originalPos = agent.transform.position;

        //Initialise the FSM states
        IdleState = new EnemyIdleState(this);
        PatrolState = new EnemyPatrolState(this);
        ChaseState = new EnemyChaseState(this);
        BackToOrigin = new EnemyBackToOriginState(this);

        SetState(IdleState);
    }
}
