using System.Collections.Generic;

/* [CLASS DOCUMENTATION]
 * 
 * This class acts as a parent class for general FSM purposes and uses.
 * 
 * [Must know]
 * 1. The SetState() method automatically calls the OnExit() and OnEntry() methods of
 *  previous + next state.
 * 
 */

public class FSM
{
    List<IState> states = new List<IState>();

    IState _currentState;
    IState _previousState;

    /// <summary>
    /// Call to invoke the OnUpdate() method of the currentState.
    /// </summary>
    public void Update()
    {
        _currentState.OnUpdate();
    }

    /// <summary>
    /// Call to set the currentState to the passed state.
    /// <para>Before assigning the next state, the previousState OnExit() gets called.</para>
    /// <para>After assigning the next state, the currentState OnEntry() gets called.</para>
    /// </summary>
    public void SetState(IState state)
    {
        _previousState = _currentState;
        if (_previousState != null && _previousState != state)
            _previousState.OnExit();

        _currentState = state;
        if (_currentState != null)
            _currentState.OnEntry();
    }

    /// <summary>
    /// Call to add the passed state in the states list IF the states list does not contain it.
    /// </summary>
    /// <param name="state"></param>
    public void AddStateToList(IState state)
    {
        if (!states.Contains(state))
        {
            states.Add(state);
        }
    }

    /// <summary>
    /// Call to get the currentState name.
    /// </summary>
    /// <returns></returns>
    public string CurrentStateName() => _currentState.GetStateName();
}
