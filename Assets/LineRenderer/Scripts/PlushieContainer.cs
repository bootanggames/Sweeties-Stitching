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
        //for(int i=0; i< LevelsHandler.instance.levelStructure.Count;i++)
        //{
        //    int state = 0;

        //    for (int j=0;j< LevelsHandler.instance.levelStructure[i].plushie.Length; j++)
        //    {
        //        Level_Metadata l_Plushie = LevelsHandler.instance.levelStructure[i].plushie[j];
        //        state = PlayerPrefs.GetInt("Level_" + i + "Plushie_" + j);
        //        if (state.Equals(1))
        //        {
        //            c++;
        //            foreach(GameObject g in plushie)
        //            {
        //                if (g.GetComponent<Plushie_Details>().plushieName.Equals(l_Plushie.levelName))
        //                {
        //                    Debug.LogError(" " + g.GetComponent<Plushie_Details>().plushieName + " " + l_Plushie.levelName);
        //                    g.SetActive(true);
        //                    break;
        //                }
        //            }
        //            var IplushieInventory = ServiceLocator.GetService<IPlushieInventory>();
        //            if (IplushieInventory != null)
        //                IplushieInventory.NoPlushieIncrement(c);
        //        }
         
        //    }

           
        //}
        foreach (GameObject g in plushie)
        {
            int state = 0;

            state = PlayerPrefs.GetInt(g.name);
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
