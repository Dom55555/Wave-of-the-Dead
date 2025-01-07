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

    public Dictionary<string,Gun>guns = new Dictionary<string,Gun>();
    public Dictionary<string,Ammo> playerAmmo = new Dictionary<string,Ammo>();

    public class Gun
    {
        public string name;
        public int price;
        public float damage;
        public float firerate;
        public string ammoType;
        public int magazineSize;
        public float reloadTime;
        public bool owned;
        public int currentMagazine;
        public float firerateTimer;

        public Gun(string Name,int Price,float Damage,float Firerate,string AmmoType,int MagazineSize,float ReloadTime,bool Owned, int CurrentMagazine, float FirerateTimer)
        {
            name = Name;
            price = Price;
            damage = Damage;
            firerate = Firerate;
            ammoType = AmmoType;
            magazineSize = MagazineSize;
            reloadTime = ReloadTime;
            owned = Owned;
            currentMagazine = CurrentMagazine;
            firerateTimer = FirerateTimer;
        }
    }
    public class Ammo
    {
        public string ammoType;
        public int price;
        public int totalAmount;
        public Ammo(string AmmoType, int Price, int TotalAmount)
        {
            ammoType = AmmoType;
            price = Price;
            totalAmount = TotalAmount;
        }
    }


    void Start()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("GunsProperties");
        string[] lines = textAsset.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines) //setting guns properties and values
        {
            string[] parts = line.Split();
            guns.Add(parts[0],new Gun(parts[0], int.Parse(parts[1]),float.Parse(parts[2]), float.Parse(parts[3]), parts[4], int.Parse(parts[5]), float.Parse(parts[6]), false,0,0));
        }
        guns["Pistol"].owned = true;
        //setting ammo details
        playerAmmo.Add("9mm", new Ammo("9mm",7,0));
        playerAmmo.Add("12gauge", new Ammo("12gauge", 10, 0));
        playerAmmo.Add("7.62mm", new Ammo("7.62mm", 11, 0));
        playerAmmo.Add("4.6mm", new Ammo("4.6mm", 8, 0));
        playerAmmo.Add("5.56mm", new Ammo("5.56mm", 14, 0));
        playerAmmo.Add("44Magnum", new Ammo("44Magnum", 8, 0));
        playerAmmo.Add("?", new Ammo("?", 5, 0));
        playerAmmo.Add("??", new Ammo("??", 5, 0));
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
        ammoText.text = guns[currentGunName].currentMagazine.ToString() + " / " + guns[currentGunName].magazineSize.ToString() + " (" + playerAmmo[guns[currentGunName].ammoType].totalAmount.ToString() + ")";

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
