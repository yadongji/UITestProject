using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// 学生数据管理类（处理增删改查业务逻辑）
/// </summary>
public class StudentModel
{
    private List<Student> _studentList = new List<Student>();
    private StudentListWrapper _wrapper = new StudentListWrapper();

    /// <summary>
    /// 加载数据（本地/网络）
    /// </summary>
    public void LoadData(bool isLocal = true)
    {
        if (isLocal)
        {
            // 从本地加载
            _wrapper = JsonTool.LoadFromLocal<StudentListWrapper>();
            _studentList = _wrapper.Students ?? new List<Student>();
            EventCenter.Instance.TriggerEvent(EventDefine.LoadDataSuccess,_studentList);
        }
        else
        {
            // 模拟从网络加载（需在MonoBehaviour中启动协程，这里用Controller调用）
            DebugHelper.Instance.Log("请通过Controller调用网络加载方法");
        }
    }

    /// <summary>
    /// 保存数据（本地/网络）
    /// </summary>
    public bool SaveData(bool isLocal = true)
    {
        _wrapper.Students = _studentList;
        if (isLocal)
        {
            return JsonTool.SaveToLocal(_wrapper);
        }
        else
        {
            // 模拟网络上传（需协程）
            return false;
        }
    }

    /// <summary>
    /// 添加学生（学号唯一）
    /// </summary>
    public bool AddStudent(Student student)
    {
        if (string.IsNullOrEmpty(student.StudentId))
        {
            DebugHelper.Instance.LogError("学号不能为空！");
            return false;
        }

        if (_studentList.Exists(s => s.StudentId == student.StudentId))
        {
            DebugHelper.Instance.LogError($"学号{student.StudentId}已存在！");
            return false;
        }

        _studentList.Add(student);
        TriggerListUpdate(); // 触发列表更新事件
        return true;
    }

    /// <summary>
    /// 删除学生（通过学号）
    /// </summary>
    public bool DeleteStudent(string studentId)
    {
        Student student = _studentList.FirstOrDefault(s => s.StudentId == studentId);
        if (student == null)
        {
            DebugHelper.Instance.LogError($"学号{studentId}不存在！");
            return false;
        }

        _studentList.Remove(student);
        TriggerListUpdate(); // 触发列表更新事件
        return true;
    }

    /// <summary>
    /// 查询学生（通过学号）
    /// </summary>
    public Student GetStudent(string studentId)
    {
        return _studentList.FirstOrDefault(s => s.StudentId == studentId);
    }

    /// <summary>
    /// 获取所有学生
    /// </summary>
    public List<Student> GetAllStudents()
    {
        return new List<Student>(_studentList); // 返回副本，避免外部直接修改
    }

    /// <summary>
    /// 触发学生列表更新事件
    /// </summary>
    private void TriggerListUpdate()
    {
        EventCenter.Instance.TriggerEvent(EventDefine.UpdateStudentList, _studentList);
    }
}