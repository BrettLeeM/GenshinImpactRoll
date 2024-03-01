using System.Collections;
using System.Collections.Generic;

//简化的状态机接口
public interface IStateBase
{
    public void Enter();
    public void Exit();
}

//简化的状态机
public class StateMachine<T> where T : IStateBase
{
    public T currentState;
    public void ChangeState(T newState)
    {
        if (newState.Equals(currentState)) return;

        if (currentState != null)
        {
            currentState.Exit();
        }
        newState.Enter();
        currentState = newState;
    }
}
