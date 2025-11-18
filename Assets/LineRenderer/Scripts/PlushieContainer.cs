using System.Linq;
using UnityEngine;

public class PlushieContainer : MonoBehaviour
{
    public int shelfId;
    public GameObject[] plushie;
    public bool full = false;

    private void OnEnable()
    {
        int c = 0;

        foreach (GameObject g in plushie)
        {
            int state = 0;
            state = PlayerPrefs.GetInt(g.name);
            if (state.Equals(1))
            {
                c++;
                g.SetActive(true);
                var IplushieInventory = ServiceLocator.GetService<IPlushieInventory>();
                if(IplushieInventory != null)
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
