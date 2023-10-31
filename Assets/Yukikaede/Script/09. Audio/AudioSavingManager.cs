using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioSavingManager : MonoBehaviour
{
    public static AudioSavingManager inst;

    private void Awake()
    {
        inst = this;
    }
}

public static class JsonDataManager
{
    // 保存 JSON 数据的方法
    public static void SaveJsonData(string json)
    {
        // 指定保存的文件路径和文件名
        string filePath = Path.Combine(Application.persistentDataPath, "data.json");

        // 将 JSON 数据写入文件
        File.WriteAllText(filePath, json);
        Debug.Log("JSON data saved.");
    }

    // 加载 JSON 数据的方法
    public static string LoadJsonData()
    {
        // 指定加载的文件路径和文件名
        string filePath = Path.Combine(Application.persistentDataPath, "data.json");

        if (File.Exists(filePath))
        {
            // 从文件中读取 JSON 数据
            string json = File.ReadAllText(filePath);
            Debug.Log("JSON data loaded.");
            return json;
        }
        else
        {
            Debug.Log("No JSON data found.");
            return null;
        }
    }
}

