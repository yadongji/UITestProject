using UnityEngine;

public class GameStarter : MonoBehaviour
{
    void Start()
    {
        InitializeMVC();
    }

    void InitializeMVC()
    {
        // 创建模型
        //UserModel userModel = new UserModel();
        //userModel.UpdateUserInfo("Player1", 1, 0);
        //
        //// 获取视图（假设场景中已有UserUIView组件）
        //UserUIView userView = FindObjectOfType<UserUIView>();
        //
        //// 创建控制器并注册
        //UserController userController = new UserController(userView, userModel);
        //MVCManager.Instance.RegisterController("UserController", userController);
    }

    void OnDestroy()
    {
        MVCManager.Instance.RemoveController("UserController");
    }
}