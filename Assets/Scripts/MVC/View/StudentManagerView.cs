using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Model;
/// <summary>
/// 学生管理UI面板（View层核心）
/// </summary>
public class StudentManagerView : UIBase
{
    [Header("输入框")]
    [SerializeField] private InputField _studentIdInput; // 学号输入框
    [SerializeField] private InputField _nameInput;       // 姓名输入框
    [SerializeField] private InputField _ageInput;        // 年龄输入框
    [SerializeField] private InputField _scoreInput;      // 成绩输入框

    [Header("按钮")]
    [SerializeField] private Button _addBtn;      // 添加按钮
    [SerializeField] private Button _deleteBtn;   // 删除按钮
    [SerializeField] private Button _saveBtn;     // 保存按钮
    [SerializeField] private Button _loadBtn;     // 加载按钮

    [Header("学生列表")]
    [SerializeField] private Transform _listContent; // 滚动视图内容区
    [SerializeField] private GameObject _studentItemPrefab; // 学生项预制体

    // 已实例化的学生项
    private List<GameObject> _studentItemList = new List<GameObject>();

    public override void Init()
    {
        base.Init();
        // 绑定按钮事件
        _addBtn.onClick.AddListener(OnAddBtnClick);
        _deleteBtn.onClick.AddListener(OnDeleteBtnClick);
        _saveBtn.onClick.AddListener(OnSaveBtnClick);
        _loadBtn.onClick.AddListener(OnLoadBtnClick);

        // 注册事件（监听学生列表更新）
        EventCenter.Instance.RegisterEvent(EventDefine.UpdateStudentList, OnStudentListUpdate);
        EventCenter.Instance.RegisterEvent(EventDefine.LoadDataSuccess, OnLoadDataSuccess);
    }

    /// <summary>
    /// 添加按钮点击事件
    /// </summary>
    private void OnAddBtnClick()
    {
        // 校验输入
        if (!CheckInput(out string studentId, out string name, out int age, out float score))
        {
            return;
        }

        // 创建学生实例，通过事件通知Controller
        Student newStudent = new Student(studentId, name, age, score);
        EventCenter.Instance.TriggerEvent(EventDefine.AddStudent, newStudent);

        // 清空输入框
        ClearInputFields();
    }

    /// <summary>
    /// 删除按钮点击事件
    /// </summary>
    private void OnDeleteBtnClick()
    {
        string studentId = _studentIdInput.text.Trim();
        if (string.IsNullOrEmpty(studentId))
        {
            Debug.LogWarning("请输入要删除的学号！");
            return;
        }

        // 通过事件通知Controller
        EventCenter.Instance.TriggerEvent(EventDefine.DeleteStudent, studentId);
        ClearInputFields();
    }

    /// <summary>
    /// 保存按钮点击事件
    /// </summary>
    private void OnSaveBtnClick()
    {
        // 通知Controller保存数据（View不直接操作Model）
        StudentController.Instance.SaveData();
    }

    /// <summary>
    /// 加载按钮点击事件
    /// </summary>
    private void OnLoadBtnClick()
    {
        // 通知Controller加载数据
        StudentController.Instance.LoadLocalData();
    }

    /// <summary>
    /// 校验输入合法性
    /// </summary>
    private bool CheckInput(out string studentId, out string name, out int age, out float score)
    {
        studentId = _studentIdInput.text.Trim();
        name = _nameInput.text.Trim();
        age = 0;
        score = 0;

        if (string.IsNullOrEmpty(studentId))
        {
            Debug.LogWarning("学号不能为空！");
            return false;
        }
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("姓名不能为空！");
            return false;
        }
        if (!int.TryParse(_ageInput.text.Trim(), out age) || age < 0 || age > 100)
        {
            Debug.LogWarning("年龄格式错误（0-100）！");
            return false;
        }
        if (!float.TryParse(_scoreInput.text.Trim(), out score) || score < 0 || score > 100)
        {
            Debug.LogWarning("成绩格式错误（0-100）！");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 清空输入框
    /// </summary>
    private void ClearInputFields()
    {
        _studentIdInput.text = "";
        _nameInput.text = "";
        _ageInput.text = "";
        _scoreInput.text = "";
    }

    /// <summary>
    /// 监听学生列表更新事件 - 更新UI
    /// </summary>
    private void OnStudentListUpdate(object data)
    {
        if (data is List<Student> studentList)
        {
            // 销毁旧的学生项
            foreach (var item in _studentItemList)
            {
                Destroy(item);
            }
            _studentItemList.Clear();

            // 创建新的学生项
            foreach (var student in studentList)
            {
                GameObject itemObj = Instantiate(_studentItemPrefab, _listContent);
                itemObj.name = student.StudentId;
                // 给学生项赋值（假设预制体有Text组件显示信息）
                Text[] texts = itemObj.GetComponentsInChildren<Text>();
                if (texts.Length >= 4)
                {
                    texts[0].text = student.StudentId;
                    texts[1].text = student.Name;
                    texts[2].text = student.Age.ToString();
                    texts[3].text = student.Score.ToString("F1");
                }
                _studentItemList.Add(itemObj);
            }
        }
    }

    /// <summary>
    /// 数据加载成功回调
    /// </summary>
    private void OnLoadDataSuccess(object data)
    {
        Debug.Log("数据加载成功，更新UI列表");
        OnStudentListUpdate(StudentController.Instance.GetAllStudents());
    }

    /// <summary>
    /// 销毁时移除事件监听
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventCenter.Instance.RemoveEvent(EventDefine.UpdateStudentList, OnStudentListUpdate);
        EventCenter.Instance.RemoveEvent(EventDefine.LoadDataSuccess, OnLoadDataSuccess);
    }
}