using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;

    public Player player;
    public Gamemanager game;
    public Buildsmanager buildsManager;
    public int hp = 100;
    public int damage = 20;

    Transform dmgTxt;
    bool isAttacking = false;
    float timer = 0;
    float timer2 = 0;
    int comboDmg = 0;
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        dmgTxt = transform.Find("DamageTextBox");
    }
    void Update()
    {
        //walking
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
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 500f);
        }
        else
        {
            agent.SetDestination(transform.position);
            Destroy(gameObject,2.5f);
        }
        timer += Time.deltaTime;

        //attacking

        if (Vector3.Distance(player.transform.position, transform.position) <= 1.5f && !isAttacking)
        {
            isAttacking = true;
            animator.SetBool("IsAttacking", true);
        }
        else if (isAttacking && Vector3.Distance(player.transform.position, transform.position) > 1.5f)
        {
            isAttacking = false;
            timer2 = 0;
            animator.SetBool("IsAttacking", false);
        }
        if(isAttacking)
        {
            timer2 += Time.deltaTime;
            if(timer2>0.8f)
            {
                timer2 = 0;
                Attack();
            }
        }

        //damage text

        if(dmgTxt.gameObject.activeSelf)
        {
            float y = dmgTxt.localPosition.y + 0.2f * Time.deltaTime; 
            dmgTxt.localPosition = new Vector3(0,y,0);
            dmgTxt.GetComponent<TMP_Text>().color = Color.Lerp(Color.red,new Color(1,0,0,0),(y-2.15f)*5);
            if (y>2.35f)
            {
                dmgTxt.localPosition = new Vector3(0,2.15f,0);
                dmgTxt.gameObject.SetActive(false);
                comboDmg = 0;
            }
        }
    }
    void Attack()
    {
        player.hp -= damage;
        game.PlayerDamaged();
    }
    public void getDamage(int dmg)
    {
        if(hp>0)
        {
            hp = Mathf.Clamp(hp-dmg,0,100000000);
            comboDmg += dmg;
            dmgTxt.gameObject.SetActive(true);
            dmgTxt.localPosition = new Vector3(0, 2.15f, 0);
            dmgTxt.GetComponent<TMP_Text>().color = new Color(1,0,0,1);
            dmgTxt.GetComponent<TMP_Text>().text = comboDmg.ToString();
        }
    }
}
