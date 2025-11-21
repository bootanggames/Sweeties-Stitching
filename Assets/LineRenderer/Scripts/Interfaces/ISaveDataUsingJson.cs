public interface ISaveDataUsingJson : IGameService
{
    void SaveData<T>(string fileName, T data);
    T LoadData<T>(string fileName) where T : class;
}
