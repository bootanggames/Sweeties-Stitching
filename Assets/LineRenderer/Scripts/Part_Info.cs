using System.Collections.Generic;
using UnityEngine;

public class Part_Info : MonoBehaviour
{
    

    public  List<GameObject> joints;

    public void EnableJoint(PlushieActiveStitchPart partType, bool val)
    {
        foreach(GameObject g in joints)
        {
            ObjectInfo o_Info = g.GetComponent<ObjectInfo>();
            if (o_Info != null)
            {
                if (o_Info.partType.Equals(partType))
                {
                    foreach (SewPoint s in o_Info.connectPoints)
                    {
                        s.gameObject.SetActive(val);
                    }
                }
            }
        } 
    }
}
