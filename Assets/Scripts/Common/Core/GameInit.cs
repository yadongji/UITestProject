using UnityEngine;

public class GameInit : MonoBehaviour
{
    private void Start()
    {
        // 初始化控制器（自动加载UI和数据）
        StudentController.Instance.Init();
    }
}