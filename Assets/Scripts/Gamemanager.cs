using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Gamemanager : MonoBehaviour
{
    public Player player;

    public TMP_Text moneyText;
    public TMP_Text ammoText;
    public TMP_Text gunNameText;

    public GameObject shop;
    public GameObject gunsMenu;
    public GameObject ammoMenu;
    public Image screenFrame;
    public Image reloadingIcon;

    public int Money = 0;
    public int Tokens = 0;
    public bool inMenu = false;
    public bool buildMode = false;
    public string primaryGun;
    public string secondaryGun;

    public Dictionary<string,Gun>guns = new Dictionary<string,Gun>();
    public Dictionary<string,Ammo> playerAmmo = new Dictionary<string,Ammo>();

    bool changingColor = false;
    Color buildColor = new Color(1,1,0,0.156f);
    Color damageColor = new Color(1,0,0,0.235f);
    Color normalColor = new Color(1,0,0,0);
    float timer = 0f;

    public class Gun
    {
        public string name;
        public int price;
        public float damage;
        public float firerate;
        public string ammoType;
        public int magazineSize;
        public float reloadTime;
        public float recoilPower;
        public bool owned;
        public int currentMagazine;
        public float firerateTimer;
        public bool isAutomatic;

        public Gun(string Name,int Price,float Damage,float Firerate,string AmmoType,int MagazineSize,float ReloadTime,float RecoilPower,bool Owned, int CurrentMagazine, float FirerateTimer, bool IsAutomatic)
        {
            name = Name;
            price = Price;
            damage = Damage;
            firerate = Firerate;
            ammoType = AmmoType;
            magazineSize = MagazineSize;
            reloadTime = ReloadTime;
            recoilPower = RecoilPower;
            owned = Owned;
            currentMagazine = CurrentMagazine;
            firerateTimer = FirerateTimer;
            isAutomatic = IsAutomatic;
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
        string[] lines = textAsset.text.Split(new char[] {'\n'}, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            string[] parts = line.Split();
            guns.Add(parts[0],new Gun(parts[0], int.Parse(parts[1]),float.Parse(parts[2]), float.Parse(parts[3]), parts[4], int.Parse(parts[5]), float.Parse(parts[6]), float.Parse(parts[7]),false,0,0, bool.Parse(parts[8])));
        }
        guns["Pistol"].owned = true;
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
        string currentGunName = player.gameObject.GetComponent<GunShooting>().currentGun.name;
        if (true)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                player.ToggleCursor();
                ShopActivate();
            }
            if(Input.GetKeyDown(KeyCode.T))
            {
                BuildMode();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1) && primaryGun!=null)
            {
                GameObject newGunPrefab = Resources.Load<GameObject>($"GunPrefabs/{primaryGun}");
                player.gameObject.GetComponent<GunShooting>().ChangeGun(newGunPrefab);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && secondaryGun!=null)
            {
                GameObject newGunPrefab = Resources.Load<GameObject>($"GunPrefabs/{secondaryGun}");
                player.gameObject.GetComponent<GunShooting>().ChangeGun(newGunPrefab);
            }
        }
        moneyText.text = Money.ToString()+"$";
        gunNameText.text = currentGunName;
        ammoText.text = guns[currentGunName].currentMagazine.ToString() + " / " + guns[currentGunName].magazineSize.ToString() + " (" + playerAmmo[guns[currentGunName].ammoType].totalAmount.ToString() + ")";

        //screen color and transparency
        if(changingColor)
        {
            timer += 2 * Time.deltaTime;
            if (buildMode)
            {
                screenFrame.color = Color.Lerp(buildColor, damageColor, timer);
            }
            else
            {
                screenFrame.color = Color.Lerp(normalColor, damageColor, timer);
            }
            if(timer>1)
            {
                changingColor = false;
                timer = 0;
            }
        }
        else if (timer<=1 && screenFrame.color != normalColor)
        {
            timer += 2 * Time.deltaTime;
            if (buildMode)
            {
                screenFrame.color = Color.Lerp(damageColor, buildColor, timer);
            }
            else
            {
                screenFrame.color = Color.Lerp(damageColor, normalColor, timer);
            }
        }
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
    public void PlayerDamaged()
    {
        timer = 0;
        changingColor = true;
    }
    public void BuildMode()
    {
        buildMode = !buildMode;
        screenFrame.color = buildMode?buildColor:normalColor;
    }
}
