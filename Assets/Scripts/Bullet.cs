using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 16;        
    public float lifetime = 1;     
    public float damage = 10;    
    public string gunType = "Pistol";

    void Start()
    {
        Destroy(gameObject, lifetime); 
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if (gunType == "Shotgun")
        {
            damage = Mathf.Clamp(damage-Time.deltaTime * 40,0,20);
        }
        if (gunType == "Spas-12")
        {
            damage = Mathf.Clamp(damage - Time.deltaTime * 70, 0, 35);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name != "PLAYER")
        {
            if (collision.collider.CompareTag("Body"))
            {
                collision.gameObject.GetComponent<Zombie>().getDamage((int)damage);
            }
            if(collision.collider.CompareTag("Head"))
            {
                collision.gameObject.GetComponent<Zombie>().getDamage((int)(damage*2));
            }
            Destroy(gameObject);
        }
    }
}
