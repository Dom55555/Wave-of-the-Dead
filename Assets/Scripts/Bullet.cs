using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 28f;        
    public float lifetime = 1f;     
    public float damage = 10f;    
    public string gunType = "Pistol";

    void Start()
    {
        Destroy(gameObject, lifetime); 
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if (gunType == "Shotgun" || gunType == "Spas-12")
        {
            damage = Mathf.Clamp(damage-Time.deltaTime*20,0,25);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name != "PLAYER")
        {
            Destroy(gameObject);
        }
    }
}
