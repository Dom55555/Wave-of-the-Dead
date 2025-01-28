using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunShooting : MonoBehaviour
{
    public Gamemanager game;  
    
    public GameObject currentGun;
    public Transform gunSlot;     
    public Camera playerCamera;

    public GameObject defaultBulletPrefab;
    public GameObject flameBulletPrefab;
    public GameObject laserBulletPrefab;
    public GameObject rocketPrefab;

    public float maxRange = 20f;
    public float fireRate = 0.15f;
    public bool reloading = false;

    Transform gunMuzzle;
    GameObject bullet;
    GameObject rocket;
    float reloadTimer = 0f;
    float fuelAppendTimer = 0f;
    int loadedAmmo;
    int maxSize;
    int totalAmmo;

    //sounds
    AudioClip reloadedSound;
    AudioClip emptySound;
    AudioClip reloadSound;
    AudioClip gunSwapSound;
    void Start()
    {
        GameObject gunPrefab = Resources.Load<GameObject>("GunPrefabs/Pistol");
        ChangeGun(gunPrefab);
        reloadedSound = Resources.Load<AudioClip>("Sounds/Reloaded");
        emptySound = Resources.Load<AudioClip>("Sounds/EmptyGun");
        reloadSound = Resources.Load<AudioClip>("Sounds/Reload");
        gunSwapSound = Resources.Load<AudioClip>("Sounds/GunSwap");
    }
    void Update()
    {
        if(game.player.hp==0)
        {
            return;
        }
        loadedAmmo= game.guns[currentGun.name].currentMagazine;
        maxSize= game.guns[currentGun.name].magazineSize;
        totalAmmo = game.playerAmmo[game.guns[currentGun.name].ammoType].totalAmount;
        if(!game.inMenu && game.guns[currentGun.name].firerateTimer >= fireRate && !reloading && !game.buildMode)
        {
            if(loadedAmmo > 0)
            {
                if (!game.guns[currentGun.name].isAutomatic)
                {
                    if (Input.GetMouseButtonDown(0)) 
                    {
                        Shoot();
                    }
                }
                else if (Input.GetMouseButton(0))
                {
                    if (currentGun.name == "Flamethrower" && !currentGun.transform.Find("Flames").GetComponent<ParticleSystem>().isPlaying)
                    {
                        currentGun.transform.Find("Flames").GetComponent<ParticleSystem>().Play();
                    }
                    Shoot();
                }
                else if (currentGun.name == "Flamethrower" && currentGun.transform.Find("Flames").GetComponent<ParticleSystem>().isPlaying)
                {
                    currentGun.transform.Find("Flames").GetComponent<ParticleSystem>().Stop();
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                game.PlaySound(emptySound, true);
            }
        }
        if(currentGun.name == "Flamethrower"&&loadedAmmo==0)
        {
            currentGun.transform.Find("Flames").GetComponent<ParticleSystem>().Stop();
        }
        Reloading();
        Timers();
    }

    void Shoot()
    {
        game.guns[currentGun.name].firerateTimer = 0;
        game.guns[currentGun.name].currentMagazine = game.guns[currentGun.name].currentMagazine - 1;
        game.PlaySound(currentGun.GetComponent<AudioSource>().clip,true);
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
                Quaternion spread = Quaternion.Euler(new Vector3(Random.Range(-2.5f, 2.5f),Random.Range(-2.5f, 2.5f),0f));
                direction = spread * direction;
            }

            if (currentGun.name == "Flamethrower")
            {
                bullet = Instantiate(flameBulletPrefab, gunMuzzle.position, Quaternion.LookRotation(direction));
            }
            else if (currentGun.name == "LaserGun")
            {
                bullet = Instantiate(laserBulletPrefab, gunMuzzle.position, Quaternion.LookRotation(direction));
            }
            else if (currentGun.name == "RPG")
            {
                bullet = Instantiate(rocketPrefab, gunMuzzle.position, Quaternion.LookRotation(direction));
            }
            else
            {
                bullet = Instantiate(defaultBulletPrefab, gunMuzzle.position, Quaternion.LookRotation(direction));
            }
            Bullet Bul = bullet.GetComponent<Bullet>();
            Bul.gunType = currentGun.name;
            Bul.damage = game.guns[currentGun.name].damage;
            Bul.game = game;
            if(currentGun.name=="RPG")
            {
                rocket.SetActive(false);
            }
        }
    }
    public void ChangeGun(GameObject newGun, bool bought = false)
    {
        if(bought == false && gunSwapSound!=null)
        {
            game.PlaySound(gunSwapSound, false);
        }
        if(currentGun != null)
        {
            Destroy(currentGun); 
        }
        game.reloadingIcon.gameObject.SetActive(false);
        game.ammoText.gameObject.SetActive(true);
        reloading = false;
        reloadTimer = 0;
        currentGun = Instantiate(newGun, gunSlot.position, playerCamera.transform.rotation);
        currentGun.transform.SetParent(gunSlot.transform);
        gunMuzzle = currentGun.transform.Find("GunMuzzle");
        currentGun.name = newGun.name;
        fireRate = currentGun.name == "Pistol" ? 0.1f : game.guns[currentGun.name].firerate;
        if(currentGun.name=="RPG")
        {
            rocket = currentGun.transform.Find("Model").transform.Find("Rocket").gameObject;
            rocket.SetActive(game.guns["RPG"].currentMagazine == 1?true:false);
        }
    }
    public void ReloadGun()
    {
        game.PlaySound(reloadedSound,true);
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
        if(currentGun.name == "RPG")
        {
            rocket.SetActive(true);
        }
    }
    public void ToggleGun(bool visible)
    {
        currentGun.gameObject.SetActive(visible);
    }
    private void Reloading()
    {
        if (Input.GetKeyDown(KeyCode.R) && !reloading && !game.inMenu && loadedAmmo < maxSize && totalAmmo > 0 && !game.buildMode && currentGun.name != "Flamethrower")
        {
            game.PlaySound(reloadSound, true);
            reloading = true;
            reloadTimer = 0f;
            game.reloadingIcon.gameObject.SetActive(true);
            game.ammoText.gameObject.SetActive(false);
        }
        if (reloading && reloadTimer >= game.guns[currentGun.name].reloadTime)
        {
            ReloadGun();
            reloading = false;
            reloadTimer = 0f;
            if (!game.buildMode)
            {
                game.reloadingIcon.gameObject.SetActive(false);
                game.ammoText.gameObject.SetActive(true);
            }
        }
    }
    private void Timers()
    {
        foreach (var gun in game.guns)
        {
            if (gun.Value.firerateTimer < fireRate)
            {
                gun.Value.firerateTimer += Time.deltaTime;
            }
            else if (gun.Value.name == "Flamethrower")
            {
                gun.Value.firerateTimer += Time.deltaTime;
            }
        }
        if (reloadTimer < game.guns[currentGun.name].reloadTime)
        {
            reloadTimer += Time.deltaTime;
            game.reloadingIcon.transform.Rotate(0, 0, -480 * Time.deltaTime);
        }
        if(fuelAppendTimer>0.1f && game.guns["Flamethrower"].firerateTimer-game.guns["Flamethrower"].firerate>0.5f)
        {
            if (game.guns["Flamethrower"].currentMagazine < game.guns["Flamethrower"].magazineSize)
            {
                game.guns["Flamethrower"].currentMagazine++;
                fuelAppendTimer = 0;
            }
        }
        fuelAppendTimer+= Time.deltaTime;
    }
}
