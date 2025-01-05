using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1f;        
    public float lifetime = 3f;     
    public float damage = 10f;    
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
            damage = Mathf.Round(Mathf.Clamp(damage-Time.deltaTime*20,0,25));
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
