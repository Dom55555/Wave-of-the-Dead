using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 16;        
    public float lifetime = 1;     
    public float damage = 10;    
    public string gunType = "Pistol";
    public Gamemanager game;

    float timer = 0;

    void Start()
    {
        Destroy(gameObject, lifetime); 
    }

    void Update()
    {
        if (gunType == "Shotgun")
        {
            damage = Mathf.Clamp(damage-Time.deltaTime * 40,0,20);
        }
        if (gunType == "Spas-12")
        {
            damage = Mathf.Clamp(damage - Time.deltaTime * 50, 0, 40);
        }
        if(gunType=="RPG" && transform.Find("RocketExplosion").gameObject.activeSelf && timer<0.7f)
        {
            if(timer<0.5f)
            {
                transform.Find("RocketExplosion").transform.GetComponent<SphereCollider>().enabled = true;
            }
            else
            {
                transform.Find("RocketExplosion").transform.GetComponent<SphereCollider>().enabled = false;
            }
        }
        timer += Time.deltaTime;
        if (gunType == "RPG" && transform.Find("RocketExplosion").gameObject.activeSelf)
        {
            return;
        }
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name != "PLAYER")
        {
            if(gunType!="RPG")
            {
                if (collision.collider.CompareTag("Body"))
                {
                    collision.gameObject.GetComponent<Zombie>().GetDamage((int)damage);
                }
                if(collision.collider.CompareTag("Head"))
                {
                    collision.gameObject.GetComponent<Zombie>().GetDamage((int)(damage*2));
                }
                if(gunType!="Flamethrower")
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                game.PlaySound(transform.GetComponent<AudioSource>().clip, true);
                transform.Find("RocketExplosion").gameObject.SetActive(true);
                transform.Find("RocketExplosion").transform.Find("Boom").GetComponent<ParticleSystem>().Play();
                transform.GetComponent<BoxCollider>().enabled = false;
                transform.Find("Model").transform.GetComponent<MeshRenderer>().enabled = false;
                timer = 0f;
                Destroy(gameObject, 2);
            }
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name != "PLAYER")
        {
            if (gunType == "Flamethrower")
            {
                if (collision.collider.CompareTag("Body"))
                {
                    collision.gameObject.GetComponent<Zombie>().GetDamage((int)damage);
                }
                if (collision.collider.CompareTag("Head"))
                {
                    collision.gameObject.GetComponent<Zombie>().GetDamage((int)(damage * 2));
                }
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != "PLAYER")
        {
            if (other.CompareTag("Body"))
            {
                other.gameObject.GetComponent<Zombie>().GetDamage((int)damage);
            }
            if (other.CompareTag("Head"))
            {
                other.transform.parent.GetComponent<Zombie>().GetDamage((int)(damage * 2));
            }
        }
    }
}
