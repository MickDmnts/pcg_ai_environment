using UnityEngine;

public class EnemyPatrolState : IState
{
    EnemyFSM stateMachine;

    int waypointIndex = 0;

    bool arrivedAtWaypoint = false;

    public EnemyPatrolState(EnemyFSM fsm)
    {
        this.stateMachine = fsm;

        stateMachine.AddStateToList(this);
    }

    // <summary>
    /// Call to get the fsm handling this state
    /// </summary>
    public FSM GetFSM()
    {
        return stateMachine;
    }

    /// <summary>
    /// Call to get this states name
    /// </summary>
    public string GetStateName()
    {
        return "Patrol";
    }

    /// <summary>
    /// Called when the state activates
    /// </summary>
    public void OnEntry()
    {
        stateMachine.agent.isStopped = false;
        stateMachine.agent.stoppingDistance = 0;

        stateMachine.enemyBehaviour.ChangeFinColor(Color.green);
    }

    public void OnUpdate()
    {
        //Change to IdleState if the agent is close to the currentWaypoint
        if (arrivedAtWaypoint)
        {
            stateMachine.SetState(stateMachine.IdleState);
        }

        if (stateMachine.waypoints != null)
        {
            PatrolToWaypoint();
        }
    }

    /// <summary>
    /// Call to iterate through the available waypoints and move the agent towards each one in a cycle.
    /// </summary>
    void PatrolToWaypoint()
    {
        if (stateMachine.waypoints.Count == 0)
        {
            Debug.Log("Waypoints list empty!");
            return;
        }

        stateMachine.agent.SetDestination(stateMachine.waypoints[waypointIndex].position);

        //Check the distance between the agent and the patrolling waypoint...
        if (Vector3.Distance(stateMachine.agent.transform.position, stateMachine.waypoints[waypointIndex].position) <= stateMachine.waypointDistanceOffset)
        {
            arrivedAtWaypoint = true;
            waypointIndex++;
        }

        //Check the waypointIndex so it down not get out of array bounds.
        if (waypointIndex >= stateMachine.waypoints.Count)
        {
            waypointIndex = 0;
        }
    }

    /// <summary>
    /// Called when the states exits
    /// </summary>
    public void OnExit()
    {
        arrivedAtWaypoint = false;
    }
}
