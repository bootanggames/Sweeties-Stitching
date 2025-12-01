using System.IO;
using UnityEngine;

public class SaveDataUsingJson : Singleton<SaveDataUsingJson>
{
    [SerializeField]string path;
    public override void SingletonAwake()
    {
        base.SingletonAwake();
        path = Application.persistentDataPath;
    }
    public override void SingletonOnDestroy()
    {
        base.SingletonOnDestroy();
    }
    public void SaveData<T>(string fileName, T data)
    {
        string folderName = Path.Combine(path, "Stitching_BackUpFiles");
        string fullPath = Path.Combine(folderName, fileName);
        if(!File.Exists(fullPath)) 
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(fullPath, json);
    }

    public T LoadData<T>(string fileName) where T : class
    {
        string folderName = Path.Combine(path, "Stitching_BackUpFiles");

        string fullPath = Path.Combine(folderName, fileName);
        if (!File.Exists(fullPath))
            return null;

        string json = File.ReadAllText(fullPath);

        return JsonUtility.FromJson<T>(json);
    }

}
