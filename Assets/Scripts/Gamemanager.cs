using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    public Player player;
    public GameObject playerObject;
    public GameObject shop;
    public GameObject gunsMenu;


    public int Money = 0;
    public bool canShoot = true;

    public Dictionary<string, Dictionary<string, string>> gunsProperties = new Dictionary<string, Dictionary<string, string>>();
    void Start()
    {
        TextAsset gunPricesText = Resources.Load<TextAsset>("GunsProperties");
        string[] lines = gunPricesText.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            string[] parts = line.Split();
            Dictionary<string, string> gunProperties = new Dictionary<string, string>();
            gunProperties.Add("price", parts[1].Trim());
            gunProperties.Add("damage", parts[2].Trim());
            gunProperties.Add("firerate", parts[3].Trim());
            gunProperties.Add("ammoType", parts[4].Trim());
            gunProperties.Add("magazineSize", parts[5].Trim());
            gunProperties.Add("owned", "False");
            gunsProperties.Add(parts[0].Trim(), gunProperties);
        }
        gunsProperties["Pistol"]["owned"] = "True";
    }
    public void Update()
    {
        if (true)
        {
            if (Input.GetKeyDown(KeyCode.F)) // shop
            {
                player.ToggleCursor();
                ShopActivate();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                GameObject newGunPrefab = Resources.Load<GameObject>("GunPrefabs/Shotgun");
                playerObject.GetComponent<GunShooting>().ChangeGun(newGunPrefab);
            }
        }
    }
    void ShopActivate()
    {
        shop.SetActive(!shop.activeSelf);
        canShoot = !canShoot;
        if(gunsMenu.activeSelf)
        {
            gunsMenu.SetActive(false);
        }
    }
}
