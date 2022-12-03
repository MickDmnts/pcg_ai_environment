using UnityEngine;

public class EnemyChaseState : IState
{
    EnemyFSM stateMachine;

    //Construct the state
    public EnemyChaseState(EnemyFSM fsm)
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
        return "Chase";
    }

    /// <summary>
    /// Called when the state activates
    /// </summary>
    public void OnEntry()
    {
        stateMachine.agent.isStopped = false;
        stateMachine.agent.stoppingDistance = 5;
        stateMachine.agent.speed = 15f;

        stateMachine.enemyBehaviour.ChangeFinColor(Color.red);

        stateMachine.enemyBehaviour.FOVManager.DeactivateAllDetectors();
    }

    public void OnUpdate()
    {
        //Move the agent towards the target
        stateMachine.agent.SetDestination(stateMachine.target.position);
    }

    /// <summary>
    /// Called when the states exits
    /// </summary>
    public void OnExit()
    {
        //nop...
    }
}
