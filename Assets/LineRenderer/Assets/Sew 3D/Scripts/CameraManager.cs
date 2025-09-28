using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private GameObject sunRays;

    // Start is called before the first frame update
    void Start()
    {
        Sewable.OnComplete += Center;       
    }

    private void OnDestroy()
    {
        Sewable.OnComplete -= Center;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Center()
    {
        EnableSunRays();

        Rigidbody[] rigidbodies = FindObjectsOfType<Rigidbody>();

        Vector3 center = Vector3.zero;
        for (int i = 0; i < rigidbodies.Length; i++)
            center += rigidbodies[i].position;

        center /= rigidbodies.Length;

        center.z = transform.position.z;

        LeanTween.move(gameObject, center, .5f);
    }

    private void EnableSunRays()
    {
        sunRays.SetActive(true);
    }
}
