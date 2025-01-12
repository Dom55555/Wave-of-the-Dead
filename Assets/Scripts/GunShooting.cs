using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;

public class GunShooting : MonoBehaviour
{
    public Gamemanager game;  
    
    public GameObject currentGun;
    public Transform gunSlot;     
    public Camera playerCamera;
    public GameObject defaultBulletPrefab;
    public GameObject flameBulletPrefab;

    public float maxRange = 20f;
    public float fireRate = 0.15f;
    public bool reloading = false;

    Transform gunMuzzle;
    float reloadTimer = 0f;

    void Start()
    {
        GameObject gunPrefab = Resources.Load<GameObject>("GunPrefabs/Pistol");
        ChangeGun(gunPrefab);
    }
    void Update()
    {
        int loadedAmmo= game.guns[currentGun.name].currentMagazine;
        int maxSize= game.guns[currentGun.name].magazineSize;
        int totalAmmo = game.playerAmmo[game.guns[currentGun.name].ammoType].totalAmount;
        //shooting
        if(!game.inMenu && game.guns[currentGun.name].firerateTimer >= fireRate && !reloading && loadedAmmo > 0)
        {
            if (!game.guns[currentGun.name].isAutomatic)
            {
                if (Input.GetMouseButtonDown(0)) 
                {
                    Shoot();
                }
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    if(currentGun.name == "Flamethrower" && !currentGun.transform.Find("Flames").GetComponent<ParticleSystem>().isPlaying)
                    {
                        currentGun.transform.Find("Flames").GetComponent<ParticleSystem>().Play();
                    }
                    Shoot();
                }
                else
                {
                    if (currentGun.name == "Flamethrower" && currentGun.transform.Find("Flames").GetComponent<ParticleSystem>().isPlaying)
                    {
                        currentGun.transform.Find("Flames").GetComponent<ParticleSystem>().Stop();
                    }
                }
            }
        }
        if(currentGun.name == "Flamethrower"&&loadedAmmo==0)
        {
            currentGun.transform.Find("Flames").GetComponent<ParticleSystem>().Stop();
        }

        //reloading
        if (Input.GetKeyDown(KeyCode.R) && !reloading && !game.inMenu && loadedAmmo < maxSize && totalAmmo > 0)
        {
            reloading = true;
            reloadTimer = 0f;
        }
        if(reloading && reloadTimer > game.guns[currentGun.name].reloadTime)
        {
            ReloadGun();
            reloading=false;
            reloadTimer = 0f;
        }
        foreach(var gun in game.guns)
        {
            gun.Value.firerateTimer = gun.Value.firerateTimer + Time.deltaTime;
        }
        reloadTimer += Time.deltaTime;
    }

    void Shoot()
    {
        game.guns[currentGun.name].firerateTimer = 0;
        game.guns[currentGun.name].currentMagazine = game.guns[currentGun.name].currentMagazine - 1;
        currentGun.GetComponent<AudioSource>().PlayOneShot(currentGun.GetComponent<AudioSource>().clip);

        transform.gameObject.GetComponent<Player>().Recoil(game.guns[currentGun.name].recoilPower);
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, maxRange) && hit.collider.gameObject.name != "Bullet")
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(maxRange);
        }
        Vector3 direction = (targetPoint - gunMuzzle.position).normalized;

        int pelletsAmount = 1;
        if (currentGun.name == "Shotgun" || currentGun.name == "Spas-12")
        {
            pelletsAmount = 3;
        }
        for(int i = 0; i<pelletsAmount;i++)
        {
            if(currentGun.name == "Shotgun"||currentGun.name == "Spas-12")
            {
                Quaternion spread = Quaternion.Euler(new Vector3(Random.Range(-1.5f, 1.5f),Random.Range(-1.5f, 1.5f),0f));
                direction = spread * direction;
            }
            GameObject bullet;
            if(currentGun.name!="Flamethrower")
            {
                bullet = Instantiate(defaultBulletPrefab, gunMuzzle.position, Quaternion.LookRotation(direction));
            }
            else
            {
                bullet = Instantiate(flameBulletPrefab, gunMuzzle.position, Quaternion.LookRotation(direction));
            }
            Bullet Bul = bullet.GetComponent<Bullet>();
            if(currentGun.name == "Flamethrower")
            {
                Bul.lifetime = 2f;
                Bul.speed = 1.5f;
            }
            Bul.gunType = currentGun.name;
            Bul.damage = game.guns[currentGun.name].damage;
        }
    }
    public void ChangeGun(GameObject newGun)
    {
        Destroy(currentGun); 
        reloading = false;
        reloadTimer = 0;
        currentGun = Instantiate(newGun, gunSlot.position, playerCamera.transform.rotation);
        currentGun.transform.SetParent(gunSlot.transform);
        gunMuzzle = currentGun.transform.Find("GunMuzzle");
        currentGun.name = newGun.name;
        if(currentGun.name == "Pistol")
        {
            fireRate = 0.11f;
        }
        else
        {
            fireRate = game.guns[currentGun.name].firerate;
        }
    }
    public void ReloadGun()
    {
        int currentMagazineAmount = game.guns[currentGun.name].currentMagazine;
        int magazineSize = game.guns[currentGun.name].magazineSize;
        string ammoType = game.guns[currentGun.name].ammoType;
        int totalAmmo = game.playerAmmo[ammoType].totalAmount;
        int reloadAmount = magazineSize - currentMagazineAmount;
        if(totalAmmo<reloadAmount)
        {
            reloadAmount = totalAmmo;
        }
        game.guns[currentGun.name].currentMagazine = currentMagazineAmount + reloadAmount;
        game.playerAmmo[ammoType].totalAmount = totalAmmo - reloadAmount;
    }
}
