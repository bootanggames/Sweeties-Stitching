using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpoolManager : MonoBehaviour,ISpoolManager
{
    [SerializeField] GameObject spoolParent;
    [SerializeField] GameObject spoolPrefab;
    [field: SerializeField] public List<GameObject> spoolList {  get; private set; }
    private void OnEnable()
    {
        RegisterService();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    void CreateSpool()
    {
        GameObject sp = Instantiate(spoolPrefab);
        sp.transform.SetParent(spoolParent.transform);
        sp.transform.localPosition = Vector3.zero;
        sp.transform.localRotation = Quaternion.identity;
        sp.transform.localScale = Vector3.one;
        sp.transform.localEulerAngles = Vector3.zero;

        if(!spoolList.Contains(sp))
            spoolList.Add(sp);
    }

    public void SpoolList(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreateSpool();
        }
    }
    public void ChangeSpriteOfSpools(Sprite sp)
    {
        foreach(GameObject g in spoolList)
        {
            SpoolInfo sInfo = g.GetComponent<SpoolInfo>();
            g.GetComponent<Image>().sprite = sp;
            Image spImage = sInfo.spoolImage;
            spImage.sprite = sp;
        }
    }
    public GameObject GetSpool(int index)
    {
        return spoolList[index];
    }
    public void RegisterService()
    {
        ServiceLocator.RegisterService<ISpoolManager>(this);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<ISpoolManager>(this);
    }
}
