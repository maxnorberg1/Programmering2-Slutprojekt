using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    private Vector3 randomPos;

    private GameObject target;
    private NavMeshAgent agent;

    public AudioSource groanSFX;

    private Animator anim;

    private bool isWalking;
    private bool isRunning;

    [SerializeField]
    private int startingHealth = 5;

    private int currentHealth;

    private int destroyTime = 2;
    private bool zombieAlive = true;

    private void Start()
    {
        randomPos = transform.position;
        target = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        WalkToRandomSpot();
    }

    private void OnEnable()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        agent.speed = 0f;
        zombieAlive = false;
        anim.SetTrigger("isDead");
        StartCoroutine(WaitThenDie());

    }

    IEnumerator WaitThenDie()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }

    private void Update()
    {
        if(MapManager.instance.zombiesCanMove && zombieAlive)
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= 5)
            {
                // Jaga spelaren
                ChasePlayer();
            }
            else if (isRunning)
            {
                // Sluta jaga
                WalkToRandomSpot();
            }

            if (isWalking)
            {
                if (Vector3.Distance(transform.position, randomPos) <= 1)
                {
                    WalkToRandomSpot();
                }
            }

            if (Vector3.Distance(transform.position, target.transform.position) <= 1)
            {
                anim.SetTrigger("attack");
            }
        }


    }

    private void ChasePlayer()
    {
        agent.SetDestination(target.transform.position);

        if(!isRunning)
        {
            groanSFX.Play();
            isRunning = true;
            isWalking = false;
            agent.speed = 2;
            // Animationer
            anim.SetBool("isRunning", isRunning);
            anim.SetBool("isWalking", isWalking);
        }

    }

    private void WalkToRandomSpot()
    {
        agent.speed = 0.75f;
        randomPos = MapManager.instance.GetRandomPos();

        agent.SetDestination(randomPos);

        isRunning = false;
        isWalking = true;

        // Animationer
        anim.SetBool("isRunning", isRunning);
        anim.SetBool("isWalking", isWalking);

    }

}
