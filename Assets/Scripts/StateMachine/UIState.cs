using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIState : MonoBehaviour, IStateBase
{
    //����״̬ʱ����Ҫ���ص�GameObject
    public List<GameObject> gameObjectsToShow = new List<GameObject>();

    //�뿪״̬ʱ����Ҫ���ص�GameObject
    public List<GameObject> gameObjectsToHide = new List<GameObject>();

    //����һ���ղ����շ���ֵ��ί������
    public delegate void StateDelegate();

    //����״̬ʱ���ί��
    public StateDelegate OnEnter;

    //�뿪״̬ʱ���ί��
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
