using UnityEditor;
using System.IO;
using System.Text;
using UnityEngine;

public class ScriptEncodingFixer : EditorWindow
{
    [MenuItem("Tools/修复脚本编码")]
    static void FixAllScriptsEncoding()
    {
        string scriptsPath = Application.dataPath;
        string[] csFiles = Directory.GetFiles(scriptsPath, "*.cs", SearchOption.AllDirectories);

        int fixedCount = 0;
        foreach (string filePath in csFiles)
        {
            if (FixFileEncoding(filePath))
                fixedCount++;
        }

        Debug.Log($"已修复 {fixedCount} 个脚本文件的编码");
        AssetDatabase.Refresh();
    }

    static bool FixFileEncoding(string filePath)
    {
        try
        {
            // 读取文件内容
            string content = File.ReadAllText(filePath, Encoding.UTF8);
            // 以UTF-8带BOM重新保存
            File.WriteAllText(filePath, content, new UTF8Encoding(true));
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"无法修复文件 {filePath}: {e.Message}");
            return false;
        }
    }
}