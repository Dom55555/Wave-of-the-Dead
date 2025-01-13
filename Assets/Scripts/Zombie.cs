using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    public Player player;
    public int hp = 100;

    float timer = 0f;
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        hp = Mathf.Clamp(hp, 0, 1000);
        animator.SetInteger("Hp", hp);
        if (hp > 0)
        {
            if (timer > 0.5f)
            {
                agent.SetDestination(player.transform.position);
                timer = 0f;
            }
            if (agent.velocity.magnitude > 0)
            {
                animator.SetBool("IsWalking", true);
            }
            else
            {
                animator.SetBool("IsWalking", false);
            }
        }
        else
        {
            agent.SetDestination(transform.position);
            Destroy(gameObject,2.5f);
        }
        print("Hp: "+hp);
        timer += Time.deltaTime;

    }
}
