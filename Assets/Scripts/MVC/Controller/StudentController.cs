using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
/// <summary>
/// 学生管理控制器（协调Model和View，解耦两层）
/// </summary>
public class StudentController : SingletonBase<StudentController>
{
    private StudentModel _studentModel; // 持有Model引用

    protected override void Awake()
    {
        base.Awake();
        // 初始化Model
        _studentModel = new StudentModel();
        // 注册事件（接收View的操作指令）
        EventCenter.Instance.RegisterEvent(EventDefine.AddStudent, OnAddStudent);
        EventCenter.Instance.RegisterEvent(EventDefine.DeleteStudent, OnDeleteStudent);
        EventCenter.Instance.RegisterEvent(EventDefine.UpdateStudentList, OnDeleteStudent);
    }

    /// <summary>
    /// 初始化（启动游戏时调用）
    /// </summary>
    public void Init()
    {
        // 加载UI面板
        UIManager.Instance.OpenView<StudentManagerView>("StudentManagerPanel");
        // 加载本地数据
        LoadLocalData();
    }

    /// <summary>
    /// 监听添加学生事件
    /// </summary>
    private void OnAddStudent(object data)
    {
        if (data is Student student)
        {
            bool success = _studentModel.AddStudent(student);
            if (success)
            {
                DebugHelper.Instance.Log($"添加学生成功：{student.StudentId} - {student.Name}");
            }
        }
    }

    /// <summary>
    /// 监听删除学生事件
    /// </summary>
    private void OnDeleteStudent(object data)
    {
        if (data is string studentId)
        {
            bool success = _studentModel.DeleteStudent(studentId);
            if (success)
            {
                DebugHelper.Instance.Log($"删除学生成功：{studentId}");
            }
        }
    }
    
    /// <summary>
    /// 监听更新学生事件
    /// </summary>
    private void OnUpdateStudent(object data)
    {
        if (data is string studentId)
        {
            bool success = _studentModel.DeleteStudent(studentId);
            if (success)
            {
                DebugHelper.Instance.Log($"删除学生成功：{studentId}");
            }
        }
    }

    /// <summary>
    /// 加载本地数据
    /// </summary>
    public void LoadLocalData()
    {
        _studentModel.LoadData(isLocal: true);
    }

    /// <summary>
    /// 加载网络数据（需在MonoBehaviour中启动协程）
    /// </summary>
    public void LoadNetData(string url)
    {
        StartCoroutine(JsonTool.DownloadJson<StudentListWrapper>(url, (wrapper) =>
        {
            if (wrapper != null && wrapper.Students != null)
            {
                // 这里需要Model提供设置数据的方法（扩展StudentModel）
                DebugHelper.Instance.Log("网络数据加载成功，更新Model");
                EventCenter.Instance.TriggerEvent(EventDefine.LoadDataSuccess);
            }
        }));
    }

    /// <summary>
    /// 保存数据（本地/网络）
    /// </summary>
    public bool SaveData(bool isLocal = true)
    {
        bool success = _studentModel.SaveData(isLocal);
        if (success)
        {
            DebugHelper.Instance.Log("数据保存成功！");
        }
        else
        {
            DebugHelper.Instance.LogError("数据保存失败！");
        }
        return success;
    }

    /// <summary>
    /// 上传数据到网络
    /// </summary>
    public void UploadData(string url)
    {
        List<Student> students = _studentModel.GetAllStudents();
        StudentListWrapper wrapper = new StudentListWrapper { Students = students };
        StartCoroutine(JsonTool.UploadJson(url, wrapper, (success) =>
        {
            if (success)
            {
                DebugHelper.Instance.Log("数据上传成功！");
            }
            else
            {
                DebugHelper.Instance.LogError("数据上传失败！");
            }
        }));
    }

    /// <summary>
    /// 获取所有学生（提供给View调用）
    /// </summary>
    public List<Student> GetAllStudents()
    {
        return _studentModel.GetAllStudents();
    }

    /// <summary>
    /// 销毁时清理资源
    /// </summary>
    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEvent(EventDefine.AddStudent, OnAddStudent);
        EventCenter.Instance.RemoveEvent(EventDefine.DeleteStudent, OnDeleteStudent);
        EventCenter.Instance.ClearAllEvents();
    }
}