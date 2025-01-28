using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    Transform dmgTxt;
    BuildObject chosenTarget;

    public List<GameObject> currentSlowingTraps = new List<GameObject>();
    public Player player;
    public Gamemanager game;
    public Buildsmanager buildsManager;
    public WaveManager waveManager;
    public int hp = 100;
    public int maxHp = 100;
    public int damage = 20;
    public float maxSpeed;

    bool dead = false;
    bool attackingPlayer = false;
    bool attackingBlock = false;
    bool blockTargetted = false;
    bool playerTargetted = false;
    int comboDmg = 0;
    float destinationTimer = 0;
    float attackTimer = 0;
    float blockAttackTimer = 0;
    float routeTimer = 0;

    AudioSource deadSound;
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = maxSpeed;
        dmgTxt = transform.Find("DamageTextBox");
        agent.avoidancePriority = Random.Range(0, 1000);
        deadSound = transform.GetComponent<AudioSource>();
    }
    void Update()
    {
        animator.SetInteger("Hp", hp);
        if (hp > 0)
        {
            SetRoute();
            Walking();
            Attacking();
            routeTimer += Time.deltaTime;
        }
        else if (!dead)
        {
            dead = true;
            game.GetMoney((int)Random.Range((float)maxHp *0.6f, (float)maxHp *0.85f));
            agent.SetDestination(transform.position);
            waveManager.ZombieDead();
            deadSound.Play();
            Destroy(gameObject, 1.35f);
        }
        if (dmgTxt.gameObject.activeSelf)
        {
            DamageText();
        }
        destinationTimer += Time.deltaTime;
    }
    public void GetDamage(int dmg, bool visible = true)
    {
        if (hp > 0)
        {
            hp = Mathf.Clamp(hp - dmg, 0, 100000000);
            comboDmg += dmg;
            dmgTxt.gameObject.SetActive(visible);
            dmgTxt.localPosition = new Vector3(0, 2.15f, 0);
            dmgTxt.GetComponent<TMP_Text>().color = new Color(1, 0, 0, 1);
            dmgTxt.GetComponent<TMP_Text>().text = comboDmg.ToString();
        }
    }
    void Walking()
    {
        animator.SetBool("IsWalking",agent.velocity.magnitude>0?true:false);
        if (destinationTimer > 0.2f)
        {
            if (playerTargetted)
            {
                agent.SetDestination(player.transform.position);
            }
            destinationTimer = 0f;
            foreach (var trap in currentSlowingTraps)
            {
                if (trap == null)
                {
                    currentSlowingTraps.Remove(trap);
                    if (currentSlowingTraps.Count == 0)
                    {
                        agent.speed = maxSpeed;
                    }
                    break;
                }
            }
        }
    }
    void Attacking()
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= 1.3f && !attackingPlayer && playerTargetted)
        {
            attackingPlayer = true;
            animator.SetBool("IsAttacking", true);
        }
        else if (attackingPlayer && Vector3.Distance(player.transform.position, transform.position) > 1.3f)
        {
            attackingPlayer = false;
            attackTimer = 0;
            animator.SetBool("IsAttacking", false);
        }
        if (attackingPlayer)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer > 0.9f)
            {
                game.PlayerDamaged(damage);
                attackTimer = 0;
            }
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 120f);

        }
    }
    void DamageText()
    {
        float y = dmgTxt.localPosition.y + 0.2f * Time.deltaTime;
        dmgTxt.localPosition = new Vector3(0, y, 0);
        dmgTxt.transform.LookAt(player.transform.position);
        dmgTxt.rotation = Quaternion.Euler(0, dmgTxt.rotation.eulerAngles.y + 180, 0);
        dmgTxt.GetComponent<TMP_Text>().color = Color.Lerp(Color.red, new Color(1, 0, 0, 0), (y - 2.15f) * 5);
        if (y > 2.35f)
        {
            dmgTxt.localPosition = new Vector3(0, 2.15f, 0);
            dmgTxt.gameObject.SetActive(false);
            comboDmg = 0;
        }
    }
    void SetTargetBlock()
    {
        chosenTarget = null;
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(player.transform.position, path);
        foreach (var block in buildsManager.buildsOnMap)
        {
            if(block == null)
            {
                break;
            }
            agent.CalculatePath(block.reachablePosition, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                chosenTarget = block;
                agent.SetDestination(chosenTarget.reachablePosition);
                return;
            }
        }
    }
    void SetRoute()
    {
        NavMeshPath path = new NavMeshPath();
        if(routeTimer>0.2f)
        {
            agent.CalculatePath(player.transform.position, path);
            routeTimer = 0;
            if (!playerTargetted && path.status == NavMeshPathStatus.PathComplete)
            {
                playerTargetted = true;
                blockTargetted = false;
                attackingBlock = false;
                animator.SetBool("IsAttacking", false);
            }
            else if (!blockTargetted && path.status == NavMeshPathStatus.PathPartial)
            {
                SetTargetBlock();
                blockTargetted = true;
                playerTargetted = false;
            }
        }
        if (blockTargetted)
        {
            agent.CalculatePath(chosenTarget.reachablePosition, path);
            if(path.status == NavMeshPathStatus.PathPartial)
            {
                blockTargetted = false;
                return;
            }
            if (chosenTarget == null || chosenTarget.hp <= 0)
            {
                blockTargetted = false;
                animator.SetBool("IsAttacking", false);
                attackingBlock = false;
                return;
            }
            if (Vector3.Distance(transform.position, chosenTarget.reachablePosition) < 1.25f && !attackingBlock)
            {
                attackingBlock = true;
                animator.SetBool("IsAttacking", true);
                agent.SetDestination(transform.position);
            }
            else if (Vector3.Distance(transform.position, chosenTarget.reachablePosition) >= 1.5f && attackingBlock)
            {
                attackingBlock = false;
                animator.SetBool("IsAttacking", false);
                agent.SetDestination(chosenTarget.reachablePosition);
            }
            if (attackingBlock)
            {
                blockAttackTimer += Time.deltaTime;
                if (blockAttackTimer > 0.8f)
                {
                    chosenTarget.GetDamaged(damage);
                    transform.LookAt(new Vector3(chosenTarget.reachablePosition.x, transform.position.y, chosenTarget.reachablePosition.z));
                    blockAttackTimer = 0;
                }
            }
        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.name=="ExplosionSphere")
        {
            GetDamage(10000,false);
        }
    }
}
