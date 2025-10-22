using UnityEngine;
using UnityEngine.AI;

public class player : MonoBehaviour
{

    public NavMeshAgent agent;
    public Transform target;
    public Animator anim;
    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position);
        if (Vector3.Distance(target.transform.position, agent.transform.position) <= 0.01f)
            anim.SetTrigger("Die");
    }
}
