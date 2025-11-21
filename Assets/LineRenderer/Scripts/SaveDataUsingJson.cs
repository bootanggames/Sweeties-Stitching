using System.IO;
using UnityEngine;

public class SaveDataUsingJson : MonoBehaviour,ISaveDataUsingJson
{
    [SerializeField]string path;
    private void Start()
    {
        path = Application.persistentDataPath;
    }
    private void Awake()
    {
        RegisterService();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    public void SaveData<T>(string fileName, T data)
    {
        string fullPath = Path.Combine(path, fileName);
        if(!File.Exists(fullPath)) 
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(fullPath, json);
    }

    public T LoadData<T>(string fileName) where T : class
    {
        string fullPath = Path.Combine(path, fileName);
        if (!File.Exists(fullPath))
            return null;

        string json = File.ReadAllText(fullPath);

        return JsonUtility.FromJson<T>(json);
    }

    public void RegisterService()
    {
        ServiceLocator.RegisterService<ISaveDataUsingJson>(this);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<ISaveDataUsingJson>(this);
    }
}
