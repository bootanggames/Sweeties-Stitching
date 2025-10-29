using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] GameObject sparklePrefabObj;
    [SerializeField] int totalCount;
    [SerializeField] float xValue;
    [SerializeField] float yValue;
    [SerializeField] Transform parent;
    [SerializeField] float loadSceneDelay;
    List<GameObject> sparkles;
    GameObject sparkle;
    private void Start()
    {
        Invoke("LoadScene", loadSceneDelay);
        //InvokeRepeating("InstantiateSingleObject", 0.2f, 1.25f);
    }

    void LoadScene()
    {
        SceneManager.LoadScene("HomeScreen");
    }
    void SetPosition(GameObject g)
    {
        g.transform.localPosition = Vector3.zero;
        float xPos = Random.Range(-xValue, xValue);
        float yPos = Random.Range(-yValue, yValue);

        g.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
    }
    void InstantiateSingleObject()
    {
        if (sparkle != null) Destroy(sparkle);
        GameObject g = Instantiate(sparklePrefabObj, parent, false);
        sparkle = g;
        g.transform.SetParent(parent);
        SetPosition(g);
    }
    void InstantiateSparkles()
    {
        for(int i = 0; i < totalCount; i++)
        {
            GameObject g = Instantiate(sparklePrefabObj, parent, false);
            if (!sparkles.Contains(g))
                sparkles.Add(g);

            g.transform.SetParent(parent);
            SetPosition(g);
        }
    }
}
