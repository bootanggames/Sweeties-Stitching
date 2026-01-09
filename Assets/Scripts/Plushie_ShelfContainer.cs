using System.Linq;
using UnityEngine;

public class Plushie_ShelfContainer : MonoBehaviour
{
    public PlushieContainer[] plushieShelf;
    IPlushieInventory inventory;
    public ItemName itemName;
    [field: SerializeField] public RectTransform targetPos {  get; private set; }
    private void OnEnable()
    {
        inventory = ServiceLocator.GetService<IPlushieInventory>();
        UpdatePlushieCount();

    }
    //private void Start()
    //{
    //    inventory = ServiceLocator.GetService<IPlushieInventory>();
    //    UpdatePlushieCount();
    //}
    void UpdatePlushieCount()
    {
        int c = 0;
        foreach(PlushieContainer pc in plushieShelf)
        {
            foreach (GameObject g in pc.plushie)
            {
                int state = 0;
                Plushie_Details pd = g.GetComponent<Plushie_Details>();
                state = PlayerPrefs.GetInt(pd.plushieName);
                if (state.Equals(1))
                {
                    c++;
                    g.SetActive(true);
                }
                else
                    g.SetActive(false);
            }
        }
        if (inventory != null)
            inventory.NoPlushieIncrement(c);
    }
    public void EnablePlushies(int shelfIndex, int id)
    {
        for(int j = 0; j <= shelfIndex; j++)
        {
            for (int i = 0; i < id; i++)
            {
                plushieShelf[j].plushie[i].SetActive(true);
            }
        }
        
    }
    public void CheckIfAllPlushiesStitched()
    {
        int c = 0;
        int total = 0;
        foreach (PlushieContainer pc in plushieShelf)
        {
            total += pc.plushie.Length;
            foreach (GameObject g in pc.plushie)
            {
                int state = 0;
                Plushie_Details pd = g.GetComponent<Plushie_Details>();
                state = PlayerPrefs.GetInt(pd.plushieName);
                if (state.Equals(1))
                    c++;
                
            }
        }
        if (c.Equals(total))
        {
            foreach (PlushieContainer pc in plushieShelf)
            {
                foreach (GameObject g in pc.plushie)
                {
                    g.SetActive(false);
                }
            }
        }

    }
}
