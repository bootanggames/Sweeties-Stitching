using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlushiePartStitchData 
{
    public int noOfConnections;
    public bool IsStitched;

    public List<Vector3> movedPositions = new List<Vector3>();
}
