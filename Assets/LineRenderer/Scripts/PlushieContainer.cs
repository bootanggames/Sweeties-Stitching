using System.Linq;
using UnityEngine;

public class PlushieContainer : MonoBehaviour
{
    public int shelfId;
    public GameObject[] plushie;
    public bool full = false;

    private void OnEnable()
    {
        UpdatePlushieCount();
    }
    private void Start()
    {
        UpdatePlushieCount();
    }
    void UpdatePlushieCount()
    {
        int c = 0;
        foreach (GameObject g in plushie)
        {
            int state = 0;
            Plushie_Details pd = g.GetComponent<Plushie_Details>();
            state = PlayerPrefs.GetInt(pd.plushieName);
            if (state.Equals(1))
            {
                c++;
                g.SetActive(true);
                var IplushieInventory = ServiceLocator.GetService<IPlushieInventory>();
                if (IplushieInventory != null)
                    IplushieInventory.NoPlushieIncrement(c);
            }
            else
                g.SetActive(false);
        }
    }
    public GameObject GetPlushie(string name)
    {
        GameObject requiredPlushie = null;
        foreach(GameObject g in plushie)
        {
            if (g.GetComponent<Plushie_Details>().plushieName.Equals(name))
            {
                requiredPlushie = g; break;
            }
        }
        return requiredPlushie;
    }
}
