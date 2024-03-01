using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIState : MonoBehaviour, IStateBase
{
    //进入状态时候需要隐藏的GameObject
    public List<GameObject> gameObjectsToShow = new List<GameObject>();

    //离开状态时候需要隐藏的GameObject
    public List<GameObject> gameObjectsToHide = new List<GameObject>();

    //声明一个空参数空返回值的委托类型
    public delegate void StateDelegate();

    //进入状态时候的委托
    public StateDelegate OnEnter;

    //离开状态时候的委托
    public StateDelegate OnExit;

    public void Enter()
    {
        for (int i = 0; i < gameObjectsToShow.Count; i++)
        {
            gameObjectsToShow[i].SetActive(true);
        }

        for (int i = 0; i < gameObjectsToHide.Count; i++)
        {
            gameObjectsToHide[i].SetActive(false);
        }
        if (OnEnter != null)
        {
            OnEnter();
        }

    }

    public void Exit()
    {
        for (int i = 0; i < gameObjectsToShow.Count; i++)
        {
            gameObjectsToShow[i].SetActive(false);
        }

        for (int i = 0; i < gameObjectsToHide.Count; i++)
        {
            gameObjectsToHide[i].SetActive(true);
        }

        if (OnExit != null)
        {
            OnExit();
        }
    }
}
