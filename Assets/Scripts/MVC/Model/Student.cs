using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 学生数据模型（需加[Serializable]标签才能被JSON序列化）
/// </summary>
[Serializable]
public class Student
{
    [SerializeField] private string _id; // 学号（唯一标识）
    [SerializeField] private string _name; // 姓名
    [SerializeField] private int _age; // 年龄
    [SerializeField] private bool _gender; // 性别 true代表男，false代表女
    [SerializeField] private float _score; // 成绩

    // 属性封装（只读/读写控制）
    public string StudentId => _id;

    public string Name
    {
        get => _name;
        set => _name = value;
    }

    public int Age
    {
        get => _age;
        set => _age = value;
    }

    public bool Gender
    {
        get => _gender;
        set => _gender = value;
    }

    public float Score
    {
        get => _score;
        set => _score = value;
    }

    // 构造函数（创建学生实例时使用）
    public Student(string id, string name, int age, string gender, float score)
    {
        _id = id;
        _name = name;
        _age = age;
        _gender = gender=="男";
        _score = score;
    }
}

/// <summary>
/// 学生列表包装类（JsonUtility不支持直接序列化List，需用类包装）
/// </summary>
[Serializable]
public class StudentListWrapper
{
    public List<Student> Students = new List<Student>();
}