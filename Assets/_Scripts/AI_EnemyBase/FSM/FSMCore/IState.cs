/* [INTERFACE DOCUMENTATION]
 * 
 * The IState interface is used in conjuction with the FSM base class to handle state transitions and managing.
 * 
 */

public interface IState
{
    FSM GetFSM();

    void OnEntry();

    void OnUpdate();

    void OnExit();

    string GetStateName();
}
