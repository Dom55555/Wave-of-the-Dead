using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    public Player player;
    public GameObject playerObject;
    public TMP_Text moneyText;

    public TMP_Text ammoText;
    public TMP_Text gunNameText;

    public GameObject shop;
    public GameObject gunsMenu;
    public GameObject ammoMenu;

    public int Money = 0;
    public bool inMenu = false;

    public Dictionary<string, Dictionary<string, string>> gunsProperties = new Dictionary<string, Dictionary<string, string>>();
    public Dictionary<string, int[]> playerAmmo = new Dictionary<string, int[]>();
    void Start()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("GunsProperties");
        string[] lines = textAsset.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines) //setting guns properties and values
        {
            string[] parts = line.Split();
            Dictionary<string, string> gunProperties = new Dictionary<string, string>();
            gunProperties.Add("price", parts[1].Trim());
            gunProperties.Add("damage", parts[2].Trim());
            gunProperties.Add("firerate", parts[3].Trim());
            gunProperties.Add("ammoType", parts[4].Trim());
            gunProperties.Add("magazineSize", parts[5].Trim());
            gunProperties.Add("reloadTime", parts[6].Trim());
            gunProperties.Add("owned", "False");
            gunProperties.Add("currentMagazine","0");
            gunProperties.Add("firerateTimer","0");
            gunsProperties.Add(parts[0].Trim(), gunProperties);
        }
        gunsProperties["Pistol"]["owned"] = "True";
        //setting ammo details
        playerAmmo.Add("9mm", new int[] { 7, 0 });
        playerAmmo.Add("12gauge", new int[] { 10, 0 });
        playerAmmo.Add("7.62mm", new int[] { 11, 0 });
        playerAmmo.Add("4.6mm", new int[] { 8, 0 });
        playerAmmo.Add("5.56mm", new int[] { 14, 0 });
        playerAmmo.Add("44Magnum", new int[] { 8, 0 });
        playerAmmo.Add("?", new int[] { 10, 0 });
        playerAmmo.Add("??", new int[] { 10, 0 });
        gunsProperties["Pistol"]["owned"] = "True";
    }
    public void Update()
    {
        string currentGunName = playerObject.GetComponent<GunShooting>().currentGun.name;
        if (true)
        {
            if (Input.GetKeyDown(KeyCode.F))
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
        moneyText.text = Money.ToString()+"$";
        gunNameText.text = currentGunName;
        ammoText.text = gunsProperties[currentGunName]["currentMagazine"] + " / " + gunsProperties[currentGunName]["magazineSize"] + " (" + playerAmmo[gunsProperties[currentGunName]["ammoType"]][1].ToString() + ")";

    }
    void ShopActivate()
    {
        shop.SetActive(!shop.activeSelf);
        inMenu = !inMenu;
        if(gunsMenu.activeSelf)
        {
            gunsMenu.SetActive(false);
        }
        if(ammoMenu.activeSelf)
        {
            ammoMenu.SetActive(false);
        }
    }
}
