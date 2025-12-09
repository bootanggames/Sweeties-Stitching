using System;

[Serializable]
public class LevelStructure
{
    public int id;
    public Level_Metadata[] plushie;
    public bool completed = false;

    public void DisableAllPlushies()
    {
        foreach(Level_Metadata p in plushie)
        {
            p.gameObject.SetActive(false);
        }
    }
}
