using UnityEngine;

public class EnemyBackToOriginState : IState
{
    EnemyFSM stateMachine;

    //Construct the state
    public EnemyBackToOriginState(EnemyFSM fsm)
    {
        this.stateMachine = fsm;

        stateMachine.AddStateToList(this);
    }

    /// <summary>
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
        return "BackToOrigin";
    }

    /// <summary>
    /// Called when the state activates
    /// </summary>
    public void OnEntry()
    {
        stateMachine.agent.isStopped = false;
        stateMachine.agent.stoppingDistance = 0;

        stateMachine.enemyBehaviour.ChangeFinColor(Color.black);
    }

    public void OnUpdate()
    {
        //Check if the agent arrived close to his starting position
        if (Vector3.Distance(stateMachine.originalPos, stateMachine.agent.transform.position) <= 1f)
        {
            //If true change to IdleState
            stateMachine.enemyBehaviour.FOVManager.ActivateAllDetectors();
            stateMachine.SetState(stateMachine.IdleState);
        }
        else
        {
            //Else continue towards the position
            stateMachine.agent.SetDestination(stateMachine.originalPos);
        }
    }

    /// <summary>
    /// Called when the states exits
    /// </summary>
    public void OnExit()
    {
        //nop...
    }
}
