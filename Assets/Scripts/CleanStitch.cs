using System.Collections.Generic;
using UnityEngine;

public class CleanStitch : MonoBehaviour
{
    [SerializeField] GameObject pointPrefab;
    ObjectInfo o_Info;
    public List<GameObject> cleanStitchPoint;

    [SerializeField] float yVal = 0;
    [SerializeField] float xVal = 0;
    void OnEnable()
    {
        o_Info = GetComponent<ObjectInfo>();
        if(cleanStitchPoint.Count == 0)
            CleanStitchPoints();
    }

    void CleanStitchPoints()
    {
        if (o_Info.partType.Equals(PlushieActiveStitchPart.righteye) || o_Info.partType.Equals(PlushieActiveStitchPart.lefteye))
        {
            GameObject point1 = Instantiate(pointPrefab);
            point1.transform.SetParent(o_Info.connectPoints[0].transform);
            point1.transform.localPosition = Vector3.zero;
            point1.transform.localEulerAngles = Vector3.zero;
            point1.transform.localScale = Vector3.one;
            o_Info.connectPoints[0].cleanStitchPoint = point1.transform;
            point1.transform.localPosition = new Vector3(-xVal, yVal, 0); // child of head

            GameObject point2 = Instantiate(pointPrefab);
            point2.transform.SetParent(o_Info.connectPoints[1].transform);
            point2.transform.localPosition = Vector3.zero;
            point2.transform.localEulerAngles = Vector3.zero;
            point2.transform.localScale = Vector3.one;
            o_Info.connectPoints[1].cleanStitchPoint = point2.transform;
            point2.transform.localPosition = new Vector3(xVal, yVal, 0); // child of head

            GameObject point3 = Instantiate(pointPrefab);
            point3.transform.SetParent(o_Info.connectPoints[2].transform);
            point3.transform.localPosition = Vector3.zero;
            point3.transform.localEulerAngles = Vector3.zero;
            point3.transform.localScale = Vector3.one;
            o_Info.connectPoints[2].cleanStitchPoint = point3.transform;
            point3.transform.localPosition = new Vector3(-xVal, -yVal, 0); // child of head

            GameObject point4 = Instantiate(pointPrefab);
            point4.transform.SetParent(o_Info.connectPoints[3].transform);
            point4.transform.localPosition = Vector3.zero;
            point4.transform.localEulerAngles = Vector3.zero;
            point4.transform.localScale = Vector3.one;
            o_Info.connectPoints[3].cleanStitchPoint = point4.transform;
            point4.transform.localPosition = new Vector3(xVal, -yVal, 0); // child of head
        }
        
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
                    g.transform.localPosition = new Vector3(0, yVal, 0);
                else
                    g.transform.localPosition = new Vector3(0, -yVal, 0);
            }
            if (o_Info.partType.Equals(PlushieActiveStitchPart.rightear))
            {
                Part_Info p_Info = this.transform.parent.GetComponent<Part_Info>();
                if (p_Info != null)
                    g.transform.localPosition = new Vector3(0, -yVal, 0); // child of head
                else
                    g.transform.localPosition = new Vector3(xVal, 0, 0);
            }
            if (o_Info.partType.Equals(PlushieActiveStitchPart.leftear))
            {
                Part_Info p_Info = this.transform.parent.GetComponent<Part_Info>();
                if (p_Info != null)
                    g.transform.localPosition = new Vector3(0, -yVal, 0);// child of head
                else
                    g.transform.localPosition = new Vector3(-xVal, 0, 0);
            }
            if (o_Info.partType.Equals(PlushieActiveStitchPart.rightarm))
            {
                Part_Info p_Info = this.transform.parent.GetComponent<Part_Info>();
                if (p_Info != null)
                    g.transform.localPosition = new Vector3(-xVal, 0, 0);// child of body
                else
                    g.transform.localPosition = new Vector3(xVal, 0, 0);
            }
            if (o_Info.partType.Equals(PlushieActiveStitchPart.leftarm))
            {
                Part_Info p_Info = this.transform.parent.GetComponent<Part_Info>();
                if (p_Info != null)
                    g.transform.localPosition = new Vector3(xVal, 0, 0);// child of body
                else
                    g.transform.localPosition = new Vector3(-xVal, 0, 0);
            }
            if (o_Info.partType.Equals(PlushieActiveStitchPart.leftleg))
            {
                Part_Info p_Info = this.transform.parent.GetComponent<Part_Info>();
                if (p_Info != null)
                    g.transform.localPosition = new Vector3(0, yVal, 0);// child of body
                else
                    g.transform.localPosition = new Vector3(0, -yVal, 0);
            }
            if (o_Info.partType.Equals(PlushieActiveStitchPart.rightleg))
            {
                Part_Info p_Info = this.transform.parent.GetComponent<Part_Info>();
                if (p_Info != null)
                    g.transform.localPosition = new Vector3(0, yVal, 0);// child of body
                else
                    g.transform.localPosition = new Vector3(0, -yVal, 0);
            }
            if (!cleanStitchPoint.Contains(g))
                cleanStitchPoint.Add(g);
        }
        //if (!o_Info.partType.Equals(PlushieActiveStitchPart.lefteye) && !o_Info.partType.Equals(PlushieActiveStitchPart.righteye))
        //{

        //}
        //else
        //{

        //}
    }

    public void UnParentPoints()
    {
        foreach (GameObject g in cleanStitchPoint)
        {
            g.transform.SetParent(null);
        }
    }
   
}
