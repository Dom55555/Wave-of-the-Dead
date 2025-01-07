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

    public float maxRange = 20f;
    public float fireRate = 0.15f;
    public bool reloading = false;

    public GameObject currentGun;
    Transform gunMuzzle;
    float reloadTimer = 0f;


    void Start()
    {
        GameObject gunPrefab = Resources.Load<GameObject>("GunPrefabs/Pistol");
        ChangeGun(gunPrefab);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !game.inMenu && float.Parse(game.gunsProperties[currentGun.name]["firerateTimer"]) >= fireRate && !reloading) 
        {
            game.gunsProperties[currentGun.name]["firerateTimer"] = "0";
            Shoot();
        }
        int loadedAmmo= int.Parse(game.gunsProperties[currentGun.name]["currentMagazine"]);
        int maxSize= int.Parse(game.gunsProperties[currentGun.name]["magazineSize"]);
        int totalAmmo = game.playerAmmo[game.gunsProperties[currentGun.name]["ammoType"]][1];
        if (Input.GetKeyDown(KeyCode.R) && !reloading && !game.inMenu && loadedAmmo < maxSize && totalAmmo > 0)
        {
            reloading = true;
            reloadTimer = 0f;
        }
        if(reloading && reloadTimer > float.Parse(game.gunsProperties[currentGun.name]["reloadTime"]))
        {
            ReloadGun();
            reloading=false;
            reloadTimer = 0f;
        }
        foreach(var gun in game.gunsProperties)
        {
            var props = gun.Value;
            props["firerateTimer"] = (float.Parse(props["firerateTimer"])+Time.deltaTime).ToString();
        }
        reloadTimer += Time.deltaTime;
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
            GameObject bullet = Instantiate(bulletPrefab, gunMuzzle.position, Quaternion.LookRotation(direction));

            Bullet Bul = bullet.GetComponent<Bullet>();
            Bul.damage = float.Parse(game.gunsProperties[currentGun.name]["damage"]);
            Bul.gunType = currentGun.name;
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
            fireRate = 0.15f;
        }
        else
        {
            fireRate = float.Parse(game.gunsProperties[currentGun.name]["firerate"]);
        }
    }
    public void ReloadGun()
    {
        int currentMagazineAmount = int.Parse(game.gunsProperties[currentGun.name]["currentMagazine"]);
        int magazineSize = int.Parse(game.gunsProperties[currentGun.name]["magazineSize"]);
        string ammoType = game.gunsProperties[currentGun.name]["ammoType"];
        int totalAmmo = game.playerAmmo[ammoType][1];
        int reloadAmount = magazineSize - currentMagazineAmount;
        if(totalAmmo<reloadAmount)
        {
            reloadAmount = totalAmmo;
        }
        game.gunsProperties[currentGun.name]["currentMagazine"] = (currentMagazineAmount+reloadAmount).ToString();
        game.playerAmmo[ammoType][1] = totalAmmo - reloadAmount;
    }
}
