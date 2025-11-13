using UnityEngine;

/// <summary>
/// 泛型单例基类（Unity场景内单例，自动创建实例）
/// </summary>
/// <typeparam name="T">子类类型</typeparam>
public class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // 查找场景中是否已有实例
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    // 没有则自动创建实例
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }
                // 切换场景不销毁（可选，根据需求关闭）
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        // 防止重复创建实例
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}