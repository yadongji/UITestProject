using System;
using System.Collections.Generic;
using UnityEngine;

public enum GradeEnum
{
    A = 1,
    B = 2,
    C = 3,
}

/// <summary>
/// 学生数据模型（需加[Serializable]标签才能被JSON序列化）
/// </summary>
[Serializable]
public class Class
{
    [SerializeField] private GradeEnum _grade; // 年级
    [SerializeField] private string _classID; // 学号（唯一标识）
    [SerializeField] private string _name; // 班级名称
    [SerializeField] private int _count; // 学生数量

    // 属性封装（只读/读写控制）
    public string ClassID => _classID;

    public string Name
    {
        get => _name;
        set => _name = value;
    }

    public int Count
    {
        get => _count;
        set => _count = value;
    }

    public GradeEnum Grade
    {
        get => _grade;
        set => _grade = value;
    }
    
    // 构造函数（创建学生实例时使用）
    public Class(int grade, string classID,string name,int count)
    {
        _grade = (GradeEnum)grade;
        _classID = classID;
        _name = name;
        _count = count;
    }
}

/// <summary>
/// 学生列表包装类（JsonUtility不支持直接序列化List，需用类包装）
/// </summary>
[Serializable]
public class ClassListWrapper
{
    public List<Class> classes = new List<Class>();
}