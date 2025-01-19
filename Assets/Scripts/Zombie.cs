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
    public int maxHp = 100;
    public int damage = 20;

    Transform dmgTxt;
    bool isAttacking = false;
    bool changePlayerHp = false;
    bool dead = false;
    int targetPlayerHp;
    int currentHp;
    float timer = 0;
    float timer2 = 0;
    float timer3 = 0;
    int comboDmg = 0;
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        dmgTxt = transform.Find("DamageTextBox");
    }
    void Update()
    {
        animator.SetInteger("Hp", hp);
        if (hp > 0)
        {
            Walking();
            Attacking();
            if(changePlayerHp)
            {
                ChangingPlayerHp();
            }
        }
        else if (!dead)
        {
            dead = true;
            game.GetMoney((int)Random.Range((float)maxHp/4, (float)maxHp/4 * 1.5f));
            agent.SetDestination(transform.position);
            Destroy(gameObject,2.5f);
        }
        if(dmgTxt.gameObject.activeSelf)
        {
            DamageText();
        }
        timer += Time.deltaTime;
    }
    void Attack()
    {
        changePlayerHp = true;
        currentHp = player.hp;
        player.hp -= damage;
        targetPlayerHp = player.hp;
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
    void Walking()
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
    void Attacking()
    {
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
        if (isAttacking)
        {
            timer2 += Time.deltaTime;
            if (timer2 > 0.9f)
            {
                Attack();
                timer2 = 0;
            }
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 120f);
        }
    }
    void ChangingPlayerHp()
    {
        timer3 += Time.deltaTime * 3;
        currentHp =(int)Mathf.Lerp(targetPlayerHp + damage, targetPlayerHp, timer3);
        if (currentHp == targetPlayerHp)
        {
            changePlayerHp = false;
            timer3 = 0;
        }
        game.ChangeBarHp(currentHp);
    }
    void DamageText()
    {
        float y = dmgTxt.localPosition.y + 0.2f * Time.deltaTime;
        dmgTxt.localPosition = new Vector3(0, y, 0);
        dmgTxt.GetComponent<TMP_Text>().color = Color.Lerp(Color.red, new Color(1, 0, 0, 0), (y - 2.15f) * 5);
        if (y > 2.35f)
        {
            dmgTxt.localPosition = new Vector3(0, 2.15f, 0);
            dmgTxt.gameObject.SetActive(false);
            comboDmg = 0;
        }
    }
}
