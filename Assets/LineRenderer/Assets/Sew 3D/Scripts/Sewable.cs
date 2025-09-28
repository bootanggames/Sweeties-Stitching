using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sewable : MonoBehaviour
{
    enum State { Idle, AutoWelding, Welded }
    private State state;

    [Header(" Elements ")]
    [SerializeField] List<AttachPair> attachPairs;
    private bool isComplete;


    [Header(" Events ")]
    public static Action OnComplete;
    public static Action OnAutoWeldStarted;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ManageState();
    }

    private void ManageState()
    {
        switch (state) 
        {
            case State.Idle:
                CheckForCompletion();
                break;

            case State.AutoWelding:
                Weld();
                break;
        }
    }

    private void CheckForCompletion()
    {
        for (int i = 0; i < attachPairs.Count; i++)
            if (!attachPairs[i].IsLinked() || !attachPairs[i].AreAttached())
                return;

        Complete();
    }

    private void Complete()
    {
        if (isComplete)
            return;

        isComplete = true;
        
        StartAutoWeld();
    }

    private void StartAutoWeld()
    {
        state = State.AutoWelding;

        Taptic.Medium();

        OnAutoWeldStarted?.Invoke();
    }

    private void Weld()
    {
        FindObjectOfType<NewLinker>().ApplyForcesToLinks();

        for (int i = 0; i < attachPairs.Count; i++)
            if (!attachPairs[i].IsStuck())
                return;

        SetLevelComplete();        
    }

    private void SetLevelComplete()
    {
        state = State.Welded;

        Taptic.Heavy();

        OnComplete?.Invoke();
    }

    [NaughtyAttributes.Button]
    public void Configure()
    {
        attachPairs.Clear();

        AttachPoint[] attachPoints = GetComponentsInChildren<AttachPoint>();

        Debug.Log("Attach points count : " + attachPoints.Length);

        for (int i = 0; i < attachPoints.Length; i++)
        {

            // Check if this attach point hasn't already been added

            bool contained = false;
            
            for (int k = 0; k < attachPairs.Count; k++)
                if (attachPairs[k].Has(attachPoints[i].transform))
                {
                    contained = true;
                    break;
                }

            if (contained)
                continue;

            

            for (int j = 0; j < attachPoints.Length; j++)
            {
                if (i == j)
                    continue;

                float distanceBetweenPoints = Vector3.Distance(attachPoints[i].transform.position, attachPoints[j].transform.position);


                if (distanceBetweenPoints > .01f)
                    continue;


                attachPairs.Add(new AttachPair(attachPoints[i].transform, attachPoints[j].transform));
            }
        }
    }
}

[System.Serializable]
public struct AttachPair
{
    public Transform attach0;
    public Transform attach1;

    public AttachPair(Transform a0, Transform a1)
    {
        attach0 = a0;
        attach1 = a1;
    }

    public bool AreAttached()
    {
        return attach0.GetComponent<AttachPoint>().IsAttached() && attach1.GetComponent<AttachPoint>().IsAttached();
    }

    public bool IsLinked()
    {
        return Vector3.Distance(attach0.position, attach1.position) < .05f;
    }

    public bool IsStuck()
    {
        return Vector3.Distance(attach0.position, attach1.position) < .01f;
    }


    public bool Has(Transform attach)
    {
        return attach0 == attach || attach1 == attach;
    }
}