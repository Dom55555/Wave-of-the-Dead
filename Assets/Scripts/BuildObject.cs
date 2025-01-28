using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class BuildObject : MonoBehaviour
{
    public Gamemanager game;

    public string buildName;
    public int maxHp;
    public int hp;
    public float duration = 0;
    public bool placed = false;
    public Vector3 reachablePosition;

    List<GameObject> targets = new List<GameObject>();
    TMP_Text textObject;
    GameObject currentHpBar;
    GameObject hpBarChild;
    GameObject maxHpBar;

    float damageTimer = 0;
    float explodeTimer = 999;

    void Start()
    {
        textObject = transform.Find("Text").GetComponent<TMP_Text>();
        currentHpBar = transform.Find("CurrentHpBar").gameObject;
        maxHpBar = transform.Find("FullHpBar").gameObject;
        hpBarChild = currentHpBar.transform.Find("Bar").gameObject;
    }
    void Update()
    {
        if(buildName=="C4")
        {
            if(explodeTimer<0.5f)
            {
                explodeTimer += Time.deltaTime;
                transform.Find("ExplosionSphere").transform.GetComponent<SphereCollider>().enabled = true;
                if(explodeTimer>=0.5f)
                {
                    transform.Find("ExplosionSphere").transform.GetComponent<SphereCollider>().enabled = false;
                }
            }
            return;
        }
        damageTimer += Time.deltaTime;
        if(targets!=null && damageTimer > 0.45f)
        {
            foreach (var target in targets)
            {
                if(target == null)
                {
                    targets.Remove(target);
                    break;
                }
                if(target.name.Contains("Zombie"))
                {
                    target.GetComponent<Zombie>().GetDamage(target.GetComponent<Zombie>().maxHp / 20);
                }
                else if (target.name.Contains("PLAYER"))
                {
                    game.PlayerDamaged(game.upgradesManager.maxPlayerHp/15);
                }
            }
            damageTimer = 0;
        }
        if(placed && Vector3.Distance(game.player.transform.position,transform.position)<3 && game.buildMode && game.buildsManager.chosenBuild=="")
        {
            showInfo();
        }
        else
        {
            textObject.gameObject.SetActive(false);
            currentHpBar.SetActive(false);
            maxHpBar.SetActive(false);
        }
        if(duration>0)
        {
            duration -= Time.deltaTime;
        }
        else if (buildName.Contains("Trap") && placed)
        {
            game.player.currentSlowingTraps.Remove(gameObject);
            if(game.player.currentSlowingTraps.Count==0)
            {
                game.player.currentSpeed = game.player.maxSpeed;
            }
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        GameObject humanoid = other.gameObject;
        if (humanoid.name.Contains("Zombie"))
        {
            if (buildName == "SlowingTrap")
            {
                humanoid.GetComponent<Zombie>().currentSlowingTraps.Add(gameObject);
                humanoid.GetComponent<NavMeshAgent>().speed = humanoid.GetComponent<Zombie>().maxSpeed / 6f;
            }
            if(buildName=="DamageTrap")
            {
                targets.Add(humanoid);
                damageTimer = 0.5f;
            }
            if(buildName=="C4")
            {
                explodeTimer = 0;
                game.PlaySound(transform.GetComponent<AudioSource>().clip,true);
                transform.Find("ExplosionSphere").gameObject.SetActive(true);
                transform.Find("ExplosionSphere").transform.Find("Boom").GetComponent<ParticleSystem>().Play();
                transform.GetComponent<BoxCollider>().enabled = false;
                foreach(Transform obj in transform.Find("Model"))
                {
                    obj.GetComponent<MeshRenderer>().enabled = false;
                }
                Destroy(gameObject,2);
            }
        }
        if (humanoid.name.Contains("PLAYER"))
        {
            if (buildName == "SlowingTrap" && game.upgradesManager.trapsDamage)
            {
                game.player.currentSlowingTraps.Add(gameObject);
                game.player.currentSpeed = game.player.maxSpeed / 6f;
            }
            if (buildName == "DamageTrap" && game.upgradesManager.trapsDamage)
            {
                targets.Add(humanoid);
                damageTimer = 0.5f;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        GameObject humanoid = other.gameObject;
        if (humanoid.name.Contains("Zombie"))
        {
            if (buildName == "SlowingTrap")
            {
                humanoid.GetComponent<Zombie>().currentSlowingTraps.Remove(gameObject);
                if(humanoid.GetComponent<Zombie>().currentSlowingTraps.Count==0)
                {
                    humanoid.GetComponent<NavMeshAgent>().speed = humanoid.GetComponent<Zombie>().maxSpeed;
                }
            }
            if (buildName == "DamageTrap")
            {
                targets.Remove(humanoid);
            }
        }
        if (humanoid.name.Contains("PLAYER"))
        {
            if (buildName == "SlowingTrap")
            {
                game.player.currentSlowingTraps.Remove(gameObject);
                if(game.player.currentSlowingTraps.Count == 0)
                {
                    game.player.currentSpeed = game.player.maxSpeed;
                }
            }
            if (buildName == "DamageTrap")
            {
                targets.Remove(humanoid);
            }
        }
    }
    void showInfo()
    {
        if(buildName == "WoodenBlock" || buildName == "MetalBlock")
        {
            textObject.text = hp+"/"+maxHp;
            textObject.gameObject.SetActive(true);
            currentHpBar.SetActive(true);
            maxHpBar.SetActive(true);
            hpBarChild.transform.localScale = new Vector3(0.4f*hp/maxHp,0.15f,0.004f);
            hpBarChild.transform.localPosition = new Vector3(0+((0.4f-(0.4f * hp / maxHp))/2f),0,0);
            if (hp >= maxHp / 2)
            {
                hpBarChild.GetComponent<Renderer>().material.color = Color.Lerp(Color.yellow, Color.green, (float)(hp-(maxHp/2)) / (maxHp/2));
            }
            else
            {
                hpBarChild.GetComponent<Renderer>().material.color = Color.Lerp(Color.red, Color.yellow, hp*2/(float)maxHp);
            }
            textObject.transform.LookAt(game.player.cameraTransform.position);
            currentHpBar.transform.LookAt(game.player.cameraTransform.position);
            maxHpBar.transform.LookAt(game.player.cameraTransform.position);
        }
        else if (buildName=="SlowingTrap"||buildName=="DamageTrap")
        {
            textObject.text = (int)duration + " s";
            textObject.gameObject.SetActive(true);
            currentHpBar.SetActive(true);
            maxHpBar.SetActive(true);
            hpBarChild.transform.localScale = new Vector3(0.4f * duration / 240, 0.15f, 0.004f);
            hpBarChild.transform.localPosition = new Vector3(0 + ((0.4f - (0.4f * duration / 240)) / 2f), 0, 0);
            if (duration >= 90)
            {
                hpBarChild.GetComponent<Renderer>().material.color = Color.Lerp(Color.yellow, Color.green, (duration-120) / 120);
            }
            else
            {
                hpBarChild.GetComponent<Renderer>().material.color = Color.Lerp(Color.red, Color.yellow, 2*duration / 240);
            }
            textObject.transform.LookAt(game.player.cameraTransform.position);
            currentHpBar.transform.LookAt(game.player.cameraTransform.position);
            maxHpBar.transform.LookAt(game.player.cameraTransform.position);
        }
    }
    public void GetDamaged(int damage)
    {
        hp -= damage;
        if(hp<=0)
        {
            game.buildsManager.buildsOnMap.Remove(gameObject.GetComponent<BuildObject>());
            Destroy(gameObject);
        }
    }


}
