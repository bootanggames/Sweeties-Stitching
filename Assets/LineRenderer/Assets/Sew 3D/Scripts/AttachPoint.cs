using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPoint : MonoBehaviour
{
    [Header(" Settings ")]
    private bool isAttached;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attach()
    {
        isAttached = true;

        Debug.Log("Here");
    }

    public bool IsAttached()
    {
        return isAttached;
    }
}
