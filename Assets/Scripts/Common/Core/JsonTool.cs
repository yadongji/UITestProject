using System;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// JSON工具类（静态）- 处理对象与JSON的转换
/// </summary>
public static class JsonTool
{
    // 本地JSON存储路径（PersistentDataPath：Unity持久化目录，跨平台兼容）
    private static string LocalJsonPath => Path.Combine(Application.persistentDataPath, "Student.json");

    /// <summary>
    /// 对象序列化为JSON字符串
    /// </summary>
    public static string Serialize(object obj)
    {
        if (obj == null) return string.Empty;
        // Unity自带JsonUtility（需给类加[Serializable]标签）
        return JsonUtility.ToJson(obj, prettyPrint: true); // prettyPrint：格式化JSON（便于阅读）
    }

    /// <summary>
    /// JSON字符串反序列化为对象
    /// </summary>
    public static T Deserialize<T>(string json) where T : class
    {
        if (string.IsNullOrEmpty(json)) return null;
        return JsonUtility.FromJson<T>(json);
    }

    /// <summary>
    /// 保存对象到本地JSON文件
    /// </summary>
    public static bool SaveToLocal(object obj)
    {
        try
        {
            string json = Serialize(obj);
            File.WriteAllText(LocalJsonPath, json);
            DebugHelper.Instance.Log($"JSON保存成功：{LocalJsonPath}");
            return true;
        }
        catch (System.Exception e)
        {
            DebugHelper.Instance.LogError($"JSON保存失败：{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 从本地JSON文件加载对象
    /// </summary>
    public static T LoadFromLocal<T>() where T : class
    {
        try
        {
            if (!File.Exists(LocalJsonPath))
            {
                DebugHelper.Instance.LogWarning($"本地JSON文件不存在，返回默认对象{Application.persistentDataPath}");
                return Activator.CreateInstance<T>(); // 创建默认实例
            }
            string json = File.ReadAllText(LocalJsonPath);
            return Deserialize<T>(json);
        }
        catch (System.Exception e)
        {
            DebugHelper.Instance.LogError($"JSON加载失败：{e.Message}");
            return null;
        }
    }

    /// <summary>
    /// 模拟网络请求（上传JSON数据）
    /// </summary>
    public static IEnumerator UploadJson(string url, object data, Action<bool> onComplete)
    {
        string json = Serialize(data);
        using (UnityWebRequest request = UnityWebRequest.Put(url, json))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                DebugHelper.Instance.Log("JSON上传成功！");
                onComplete?.Invoke(true);
            }
            else
            {
                DebugHelper.Instance.LogError($"JSON上传失败：{request.error}");
                onComplete?.Invoke(false);
            }
        }
    }

    /// <summary>
    /// 模拟网络请求（下载JSON数据）
    /// </summary>
    public static IEnumerator DownloadJson<T>(string url, Action<T> onComplete) where T : class
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                T data = Deserialize<T>(request.downloadHandler.text);
                DebugHelper.Instance.Log("JSON下载成功！");
                onComplete?.Invoke(data);
            }
            else
            {
                DebugHelper.Instance.LogError($"JSON下载失败：{request.error}");
                onComplete?.Invoke(null);
            }
        }
    }
}