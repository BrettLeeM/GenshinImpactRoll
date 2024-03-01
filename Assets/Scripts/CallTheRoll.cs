using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//������ع���
public struct StudentInfo
{
    public StudentInfo(string inName, int inWeight)
    {
        name = inName;
        weight = inWeight;
        //ȷ��Ȩ���Ǵ��ڵ���0��
        if (weight < 0)
        {
            weight = 0;
        }
        characterPath = "";
        elementPath = "";
    }
    //����
    public string name { get; set; }

    //������Ȩ�أ�Խ��Խ���ױ��㵽��
    public float weight { get; set; }

    //�������StreamingAsset·��
    public string characterPath { get; set; }

    //Ԫ��ͼ�����StreamingAsset·��
    public string elementPath { get; set; }
}

public class CallTheRoll
{
    //��¼����ѧ����Ϣ��List
    public static List<StudentInfo> studentInfos = new List<StudentInfo>();

    public static bool RollStudent(out StudentInfo studentInfo)
    {
        studentInfo = new StudentInfo();
        if (studentInfos.Count == 0) return false;

        //��ǩ�ĳ���
        List<int> lottery = new List<int>();

        for (int i = 0; i < studentInfos.Count; i++)
        {
            //����Ȩ��������ǩ���������
            for (int j = 0; j < studentInfos[i].weight; j++)
            {
                lottery.Add(i);
            }
        }
        if (lottery.Count == 0) return false;
        //���һ���±�
        int randomIndex = Random.Range(0, lottery.Count);
        studentInfo = studentInfos[lottery[randomIndex]];
        return true;
    }

    /// <summary>
    /// ������ɸ�ѧ�������ظ�
    /// </summary>
    /// <param name="studentsNum">���ѧ������������С��ѧ������</param>
    /// <returns></returns>
    public static bool RollStudents(int studentsNum,out StudentInfo[] outStudentInfos)
    {
        outStudentInfos = new StudentInfo[studentsNum];
        //�����ж�
        if (studentInfos.Count == 0 || studentsNum <= 0 || studentsNum > studentInfos.Count) return false;
        //��ǩ�ĳ���
        List<int> lottery = new List<int>();

        for (int i = 0; i < studentInfos.Count; i++)
        {
            //����Ȩ��������ǩ���������
            for (int j = 0; j < studentInfos[i].weight; j++)
            {
                lottery.Add(i);
            }
        }
        if (lottery.Count == 0) return false;
        for (int i = 0; i < studentsNum; i++)
        {
            //���һ���±�
            int randomIndex = Random.Range(0, lottery.Count);
            int indexTemp = lottery[randomIndex];
            outStudentInfos[i] = studentInfos[indexTemp];

            //�ӳ�ǩ���н��鵽������ɾ��
            for (int j = 0; j < lottery.Count; j++)
            {
                if (lottery[j] == indexTemp)
                {
                    lottery.RemoveAt(j);
                    j--;
                }

            }
        }

        return true;
    }
}
