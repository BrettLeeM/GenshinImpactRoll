using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//点名相关功能
public struct StudentInfo
{
    public StudentInfo(string inName, int inWeight)
    {
        name = inName;
        weight = inWeight;
        //确保权重是大于等于0的
        if (weight < 0)
        {
            weight = 0;
        }
        characterPath = "";
        elementPath = "";
    }
    //姓名
    public string name { get; set; }

    //被点名权重（越高越容易被点到）
    public float weight { get; set; }

    //立绘相对StreamingAsset路径
    public string characterPath { get; set; }

    //元素图标相对StreamingAsset路径
    public string elementPath { get; set; }
}

public class CallTheRoll
{
    //记录所有学生信息的List
    public static List<StudentInfo> studentInfos = new List<StudentInfo>();

    public static bool RollStudent(out StudentInfo studentInfo)
    {
        studentInfo = new StudentInfo();
        if (studentInfos.Count == 0) return false;

        //抽签的池子
        List<int> lottery = new List<int>();

        for (int i = 0; i < studentInfos.Count; i++)
        {
            //根据权重数量将签放入池子中
            for (int j = 0; j < studentInfos[i].weight; j++)
            {
                lottery.Add(i);
            }
        }
        if (lottery.Count == 0) return false;
        //随机一个下标
        int randomIndex = Random.Range(0, lottery.Count);
        studentInfo = studentInfos[lottery[randomIndex]];
        return true;
    }

    /// <summary>
    /// 随机若干个学生，不重复
    /// </summary>
    /// <param name="studentsNum">随机学生数量，不能小于学生数量</param>
    /// <returns></returns>
    public static bool RollStudents(int studentsNum,out StudentInfo[] outStudentInfos)
    {
        outStudentInfos = new StudentInfo[studentsNum];
        //错误判断
        if (studentInfos.Count == 0 || studentsNum <= 0 || studentsNum > studentInfos.Count) return false;
        //抽签的池子
        List<int> lottery = new List<int>();

        for (int i = 0; i < studentInfos.Count; i++)
        {
            //根据权重数量将签放入池子中
            for (int j = 0; j < studentInfos[i].weight; j++)
            {
                lottery.Add(i);
            }
        }
        if (lottery.Count == 0) return false;
        for (int i = 0; i < studentsNum; i++)
        {
            //随机一个下标
            int randomIndex = Random.Range(0, lottery.Count);
            int indexTemp = lottery[randomIndex];
            outStudentInfos[i] = studentInfos[indexTemp];

            //从抽签池中将抽到的名字删掉
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
