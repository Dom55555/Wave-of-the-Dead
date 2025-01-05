using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEngine;

public class GunShooting : MonoBehaviour
{
    public Gamemanager game;  
    
    public GameObject bulletPrefab;
    public Transform gunSlot;     
    public Camera playerCamera;

    public float maxRange = 40f;
    public float fireRate = 0.15f;

    //private
    GameObject currentGun;
    Transform gunMuzzle;
    float timer = 0f;


    void Start()
    {
        GameObject gunPrefab = Resources.Load<GameObject>("GunPrefabs/Pistol");
        ChangeGun(gunPrefab);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && game.canShoot && timer>fireRate) 
        {
            Shoot();
            timer = 0f;
        }
        timer += Time.deltaTime;

        
    }

    void Shoot()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, maxRange))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(maxRange);
        }
        Vector3 direction = (targetPoint - gunMuzzle.position).normalized;

        int bulletAmount = 1;
        if (currentGun.name == "Shotgun")
        {
            bulletAmount = 3;
        }
        for(int i = 0; i<bulletAmount;i++)
        {
            if(currentGun.name == "Shotgun")
            {
                Quaternion spread = Quaternion.Euler(new Vector3(Random.Range(-5f, 5f),Random.Range(-5f, 5f),0f));
                direction = gunMuzzle.rotation * spread * Vector3.forward;
            }
            GameObject bullet = Instantiate(bulletPrefab, gunMuzzle.position, Quaternion.LookRotation(direction));

            Bullet Bul = bullet.GetComponent<Bullet>();
            switch (currentGun.name)
            {
                case "Pistol":
                    Bul.damage = 10f;
                    break;
                case "Shotgun":
                    Bul.damage = 25f;
                    break;
            }
            Bul.gunType = currentGun.name;
        }
    }
    public void ChangeGun(GameObject newGun)
    {
        Destroy(currentGun);
        currentGun = Instantiate(newGun, gunSlot.position, playerCamera.transform.rotation);
        currentGun.transform.SetParent(gunSlot.transform);
        gunMuzzle = currentGun.transform.Find("GunMuzzle");
        currentGun.name = newGun.name;
        switch (currentGun.name)
        {
            case "Pistol":
                fireRate = 0.15f;
                break;
            case "Shotgun":
                fireRate = 0.6f;
                break;
        }
    }
}
