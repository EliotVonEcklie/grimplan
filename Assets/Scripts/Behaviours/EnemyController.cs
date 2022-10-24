using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IDamageable
{
    NavMeshAgent agent;
    Animator animator;

    int isWalkingHash;

    public float stoppingDistance = 3.0f;
    public float Health { get; private set; }

    private Vector3 GetPlayerPos()
    {
        return GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");

        Health = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.destination != GetPlayerPos())
        {
            agent.destination = GetPlayerPos();
        }
        
        animator.SetBool(isWalkingHash, agent.remainingDistance > stoppingDistance);
    }

    public void OnDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Destroy(gameObject);
        } 
    }
}
