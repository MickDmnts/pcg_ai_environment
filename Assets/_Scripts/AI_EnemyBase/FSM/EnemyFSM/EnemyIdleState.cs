using UnityEngine;

public class EnemyIdleState : IState
{
    EnemyFSM stateMachine;

    float waitTime;
    float waitDoneTime;

    public EnemyIdleState(EnemyFSM fsm)
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
        return "Idle";
    }

    /// <summary>
    /// Called when the state activates
    /// </summary>
    public void OnEntry()
    {
        stateMachine.agent.isStopped = true;
        stateMachine.agent.stoppingDistance = 0;

        waitTime = stateMachine.idleTime;
        waitDoneTime = Time.time + waitTime;

        stateMachine.enemyBehaviour.ChangeFinColor(Color.cyan);
    }

    public void OnUpdate()
    {
        //Stay idle for X seconds and then transit to PatrolState
        if (Time.time > waitDoneTime)
        {
            stateMachine.SetState(stateMachine.PatrolState);
            return;
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
