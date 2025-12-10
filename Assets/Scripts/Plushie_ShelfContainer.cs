using UnityEngine;

public class Plushie_ShelfContainer : MonoBehaviour
{
    public PlushieContainer[] plushieShelf;
    IPlushieInventory inventory;
    public DecorItemName itemName;
    private void OnEnable()
    {
        inventory = ServiceLocator.GetService<IPlushieInventory>();
        UpdatePlushieCount();
    }
    private void Start()
    {
        inventory = ServiceLocator.GetService<IPlushieInventory>();
        UpdatePlushieCount();
    }
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
}
