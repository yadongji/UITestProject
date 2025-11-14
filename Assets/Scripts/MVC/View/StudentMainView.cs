// 1. 学生管理主面板（普通面板子类）

using UnityEngine;
using UnityEngine.UI;

public class StudentMainView : UINormalBase
{
    [Header("主面板UI")] [SerializeField] private Button _addStudentBtn;
    [SerializeField] private Button _editStudentBtn;
    [SerializeField] private Button _showDetailBtn;

    public override void Init()
    {
        base.Init();
        // 绑定业务按钮事件
        _addStudentBtn.onClick.AddListener(() =>
        {
            // 打开添加学生弹窗（跳转逻辑）
            UIManager.Instance.OpenView<StudentAddPopup>("StudentAddPopup");
        });
        _showDetailBtn.onClick.AddListener(() =>
        {
            // 打开学生详情面板（替换当前面板）
            UIManager.Instance.OpenView<StudentDetailView>("StudentDetailView", openMode: ViewOpenMode.ReplaceCurrent);
        });
    }

    // 重写刷新数据方法（业务逻辑）
    public override void RefreshData(object data)
    {
        base.RefreshData(data);
        // 从Controller获取学生列表并更新UI
        var studentList = StudentController.Instance.GetAllStudents();
        // TODO: 更新主面板学生列表显示
    }

    // 重写解绑事件（必须实现）
    protected override void UnbindAllEvents()
    {
        base.UnbindAllEvents();
        // 移除当前面板注册的事件
        EventCenter.Instance.RemoveEvent(EventDefine.UpdateStudentList, RefreshData);
    }
}

// 2. 添加学生弹窗（弹窗面板子类）
public class StudentAddPopup : UIPopupBase
{
    [Header("添加学生UI")] [SerializeField] private InputField _studentIdInput;
    [SerializeField] private InputField _nameInput;
    [SerializeField] private Button _confirmBtn;
    [SerializeField] private Button _cancelBtn;

    public override void Init()
    {
        base.Init();
        // 绑定弹窗按钮事件
        _confirmBtn.onClick.AddListener(OnConfirmAdd);
        _cancelBtn.onClick.AddListener(() => { UIManager.Instance.CloseView(ViewName); });
    }

    // 确认添加学生
    private void OnConfirmAdd()
    {
        string studentId = _studentIdInput.text.Trim();
        string name = _nameInput.text.Trim();
        if (string.IsNullOrEmpty(studentId) || string.IsNullOrEmpty(name))
        {
            // 打开提示面板
            UIManager.Instance.OpenView<UIToastBase>("ToastView",
                initAction: toast => toast.SetToastContent("学号和姓名不能为空！", isSuccess: false));
            return;
        }

        // 通知Controller添加学生
        // TODO测试数据
        Student studentData = new Student(studentId, name, 18, 80);
        EventCenter.Instance.TriggerEvent(EventDefine.AddStudent, studentData);
        UIManager.Instance.CloseView(ViewName);

        // 显示添加成功提示
        UIManager.Instance.OpenView<UIToastBase>("ToastView",
            initAction: toast => toast.SetToastContent("添加学生成功！", isSuccess: true));

        // 刷新主面板数据
        UIManager.Instance.GetView<StudentMainView>("StudentMainView")?.RefreshData(studentData);
    }

    protected override void UnbindAllEvents()
    {
        base.UnbindAllEvents();
        // 移除当前面板的事件监听（如有）
    }
}

// 3. 学生详情面板（普通面板子类）
public class StudentDetailView : UINormalBase
{
    [Header("详情面板UI")] [SerializeField] private Text _studentInfoText;
    [SerializeField] private Button _backBtn;

    public override void Init()
    {
        base.Init();
        _backBtn.onClick.AddListener(() =>
        {
            // 返回上一级面板
            UIManager.Instance.GoBack();
        });
    }

    // 接收参数（显示指定学生详情）
    public void SetStudentDetail(Student student)
    {
        _studentInfoText.text = $"学号：{student.StudentId}\n姓名：{student.Name}\n年龄：{student.Age}\n成绩：{student.Score}";
    }

    protected override void UnbindAllEvents()
    {
        base.UnbindAllEvents();
    }
}