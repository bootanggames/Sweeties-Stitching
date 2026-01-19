using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PointsDetectionData 
{
    [field: SerializeField] public List<SewPoint> points { get; private set; }
    [field: SerializeField] public List<Connections> connections { get; private set; }
    [field: SerializeField] public List<SewPoint> wrongConnectPoint { get; private set; }
    [field: SerializeField] public Material correctPointMaterial { get; private set; }
    [field: SerializeField] public Material wrongPointMaterial { get; private set; }
    [field: SerializeField] public Material originalMaterial { get; private set; }
    [field: SerializeField] public Material startToDetectMaterial { get; private set; }

    public void InitializePointsList()
    {
        points = new List<SewPoint>();
    }
    public void InitializeConnectionsList()
    {
        connections = new List<Connections>();
    }
    public void ResetPointsList(List<SewPoint> list)
    {
        points = list;
    }
}
