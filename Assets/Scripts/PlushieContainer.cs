using System.Linq;
using UnityEngine;

public class PlushieContainer : MonoBehaviour
{
    public int shelfId;
    public GameObject[] plushie;
    public bool full = false;

    public GameObject GetPlushie(string name)
    {
        GameObject requiredPlushie = null;
        foreach (GameObject g in plushie)
        {
            if (g.GetComponent<Plushie_Details>().plushieName.Equals(name))
            {
                requiredPlushie = g; break;
            }
        }
        return requiredPlushie;
    }
}
