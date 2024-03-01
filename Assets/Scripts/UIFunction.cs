using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFunction : MonoBehaviour
{
    [HideInInspector]
    public static UIFunction Ins;
    public StateMachine<UIState> UIStateMachine = new StateMachine<UIState>();
    public UIState mainUI;
    public UIState videoUI;
    public UIState animationUI;
    public UIState tenTimesUI;
    private void Awake()
    {
        Ins = this;
    }
}
