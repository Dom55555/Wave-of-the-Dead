using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Gamemanager : MonoBehaviour
{
    public Player player;
    public Buildsmanager buildsManager;

    public TMP_Text moneyText;
    public TMP_Text tokensText;
    public TMP_Text ammoText;
    public TMP_Text gunNameText;
    public TMP_Text hpText;
    public RectTransform moneyAddText;

    public GameObject shop;
    public GameObject gunsMenu;
    public GameObject ammoMenu;
    public GameObject buildsMenu;

    public Image screenFrame;
    public Image reloadingIcon;
    public Image hpBar;


    public int Money = 100;
    public int Tokens = 0;
    public bool inMenu = false;
    public bool buildMode = false;
    public string primaryGun;
    public string secondaryGun;

    public Dictionary<string,Gun>guns = new Dictionary<string,Gun>();
    public Dictionary<string,Ammo> playerAmmo = new Dictionary<string,Ammo>();
    public Dictionary<string,Build> builds = new Dictionary<string,Build>();

    bool changingColor = false;
    Color buildColor = new Color(1,1,0,0.05f);
    Color damageColor = new Color(1,0,0,0.2f);
    Color normalColor = new Color(1,0,0,0);
    float timer = 0f;

    Color maxHpColor = new Color(0.196f,0.627f,0.196f);
    Color zeroHpColor = new Color(0.745f,0.157f,0.157f);

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
        public bool tokenUnlocked;
        public int tokenPrice;

        public Gun(string Name,int Price,float Damage,float Firerate,string AmmoType,int MagazineSize,float ReloadTime,float RecoilPower,bool Owned, int CurrentMagazine, float FirerateTimer, bool IsAutomatic, bool TokenUnlocked, int TokenPrice)
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
            tokenUnlocked = TokenUnlocked;
            tokenPrice = TokenPrice;
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
    public class Build
    {
        public string buildName;
        public bool tokenBuild;
        public int price;
        public int totalAmount;
        public Build(string BuildName,bool TokenBuild,int Price, int TotalAmount)
        {
            buildName = BuildName;
            tokenBuild = TokenBuild;
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
            guns.Add(parts[0],new Gun(parts[0], int.Parse(parts[1]),float.Parse(parts[2]), float.Parse(parts[3]), parts[4], int.Parse(parts[5]), float.Parse(parts[6]), float.Parse(parts[7]),false,0,0, bool.Parse(parts[8]), bool.Parse(parts[9]), int.Parse(parts[10])));
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

        builds.Add("WoodenBlock",new Build("WoodenBlock",false,500,0));
        builds.Add("MetalBlock", new Build("MetalBlock", false, 5000, 0));
        builds.Add("SlowingTrap", new Build("SlowingTrap", true, 1, 0));
        builds.Add("DamageTrap", new Build("DamageTrap", true, 2, 0));
        builds.Add("C4", new Build("C4", true, 3, 0));

        ChangeBarHp(100);
    }
    public void Update()
    {
        string currentGunName = player.gameObject.GetComponent<GunShooting>().currentGun.name;
        if(Input.GetKeyDown(KeyCode.T) && !inMenu)
        {
            BuildMode();
        }
        if(!buildMode)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                player.ToggleCursor();
                ShopActivate();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1) && primaryGun!=null)
            {
                GameObject newGunPrefab = Resources.Load<GameObject>($"GunPrefabs/{primaryGun}");
                reloadingIcon.gameObject.SetActive(false);
                ammoText.gameObject.SetActive(true);
                player.gameObject.GetComponent<GunShooting>().ChangeGun(newGunPrefab);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && secondaryGun!=null)
            {
                GameObject newGunPrefab = Resources.Load<GameObject>($"GunPrefabs/{secondaryGun}");
                reloadingIcon.gameObject.SetActive(false);
                ammoText.gameObject.SetActive(true);
                player.gameObject.GetComponent<GunShooting>().ChangeGun(newGunPrefab);
            }
        }
        moneyText.text = Money.ToString()+"$";
        tokensText.text = Tokens.ToString();
        gunNameText.text = currentGunName;
        ammoText.text = guns[currentGunName].currentMagazine.ToString() + " / " + guns[currentGunName].magazineSize.ToString() + " (" + playerAmmo[guns[currentGunName].ammoType].totalAmount.ToString() + ")";
        ScreenColor();
        if (moneyAddText.gameObject.activeSelf)
        {
            MoneyTextAnimation();
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
        if(buildsMenu.activeSelf)
        {
            buildsMenu.SetActive(false);
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
        ammoText.gameObject.SetActive(!buildMode);
        gunNameText.gameObject.SetActive(!buildMode);
        reloadingIcon.gameObject.SetActive(false);
        buildsManager.BuildMode();
    }
    public void ChangeBarHp(int hp)
    {
        if (hp >= player.hpLimit / 2)
        {
            hpBar.color = Color.Lerp(Color.yellow, maxHpColor, hp / (float)player.hpLimit);
        }
        else
        {
            hpBar.color = Color.Lerp(zeroHpColor, Color.yellow, hp / ((float)player.hpLimit / 2));
        }
        hpBar.rectTransform.sizeDelta = new Vector2(200f * hp / (float)player.hpLimit, hpBar.rectTransform.sizeDelta.y);
        hpText.text = hp + " / " + player.hpLimit;
    }
    public void GetMoney(int moneyAmount)
    {
        Money += moneyAmount;
        moneyAddText.gameObject.SetActive(true);
        moneyAddText.GetComponent<TMP_Text>().color = new Color(0.255f, 0.94f, 0, 1);
        moneyAddText.GetComponent<TMP_Text>().text = "+"+moneyAmount.ToString()+"$";
    }
    void ScreenColor()
    {
        if (changingColor)
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
            if (timer > 1)
            {
                changingColor = false;
                timer = 0;
            }
        }
        else if (timer <= 1 && screenFrame.color != normalColor)
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
    void MoneyTextAnimation()
    {
        float y = moneyAddText.anchoredPosition.y + 25 * Time.deltaTime;
        moneyAddText.anchoredPosition = new Vector2(40, y);
        moneyAddText.GetComponent<TMP_Text>().color = Color.Lerp(new Color(0.255f, 0.94f, 0, 1), new Color(0.255f, 0.94f, 0, 0), (y - 54)/22);
        if (y > 76f)
        {
            moneyAddText.anchoredPosition = new Vector2(40, 54);
            moneyAddText.gameObject.SetActive(false);
        }
    }
    void GameOver()
    {

    }
}
