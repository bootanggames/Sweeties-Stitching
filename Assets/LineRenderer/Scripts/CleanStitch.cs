using System.Collections.Generic;
using UnityEngine;

public class CleanStitch : MonoBehaviour
{
    [SerializeField] GameObject pointPrefab;
    ObjectInfo o_Info;
    public List<GameObject> cleanStitchPoint;
   
    void Start()
    {
        o_Info = GetComponent<ObjectInfo>();
        CleanStitchPoints();
    }

    void CleanStitchPoints()
    {
        if (!o_Info.partType.Equals(PlushieActiveStitchPart.lefteye) && !o_Info.partType.Equals(PlushieActiveStitchPart.righteye))
        {
            foreach (SewPoint s in o_Info.connectPoints)
            {
                GameObject g = Instantiate(pointPrefab);
                g.transform.SetParent(s.transform);
                g.transform.localPosition = Vector3.zero;
                g.transform.localEulerAngles = Vector3.zero;
                g.transform.localScale = Vector3.one;
                s.cleanStitchPoint = g.transform;
                if (o_Info.partType.Equals(PlushieActiveStitchPart.neck))
                {
                    if (o_Info.head)
                        g.transform.localPosition = new Vector3(0, 0.4f, 0);
                    else
                        g.transform.localPosition = new Vector3(0, -0.4f, 0);
                }
                if (o_Info.partType.Equals(PlushieActiveStitchPart.rightear))
                {
                    Part_Info p_Info = this.transform.parent.GetComponent<Part_Info>();
                    if (p_Info != null)
                        g.transform.localPosition = new Vector3(0, -0.4f, 0);
                    else
                        g.transform.localPosition = new Vector3(0.4f, 0, 0);
                }
                if (o_Info.partType.Equals(PlushieActiveStitchPart.leftear))
                {
                    Part_Info p_Info = this.transform.parent.GetComponent<Part_Info>();
                    if (p_Info != null)
                        g.transform.localPosition = new Vector3(0, -0.4f, 0);
                    else
                        g.transform.localPosition = new Vector3(-0.4f, 0, 0);
                }
                if (o_Info.partType.Equals(PlushieActiveStitchPart.rightarm))
                {
                    Part_Info p_Info = this.transform.parent.GetComponent<Part_Info>();
                    if (p_Info != null)
                        g.transform.localPosition = new Vector3(-0.4f, 0, 0);
                    else
                        g.transform.localPosition = new Vector3(0.4f, 0, 0);
                }
                if (o_Info.partType.Equals(PlushieActiveStitchPart.leftarm))
                {
                    Part_Info p_Info = this.transform.parent.GetComponent<Part_Info>();
                    if (p_Info != null)
                        g.transform.localPosition = new Vector3(0.4f, 0, 0);
                    else
                        g.transform.localPosition = new Vector3(-0.4f, 0, 0);
                }
                if (o_Info.partType.Equals(PlushieActiveStitchPart.leftleg))
                {
                    Part_Info p_Info = this.transform.parent.GetComponent<Part_Info>();
                    if (p_Info != null)
                        g.transform.localPosition = new Vector3(0, 0.4f, 0);
                    else
                        g.transform.localPosition = new Vector3(0, -0.4f, 0);
                }
                if (o_Info.partType.Equals(PlushieActiveStitchPart.rightleg))
                {
                    Part_Info p_Info = this.transform.parent.GetComponent<Part_Info>();
                    if (p_Info != null)
                        g.transform.localPosition = new Vector3(0, 0.4f, 0);
                    else
                        g.transform.localPosition = new Vector3(0, -0.4f, 0);
                }
                if (!cleanStitchPoint.Contains(g))
                    cleanStitchPoint.Add(g);
            }
        }
      
    }

    public void UnParentPoints()
    {
        foreach (GameObject g in cleanStitchPoint)
        {
            g.transform.SetParent(null);
        }
    }
   
}
