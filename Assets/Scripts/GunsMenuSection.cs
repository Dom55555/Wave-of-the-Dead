using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GunsMenuSection: MonoBehaviour
{

    public GunShooting player;
    public Gamemanager game;

    public TMP_Text gunName;
    public TMP_Text damage;
    public TMP_Text firerate;
    public TMP_Text ammoType;
    public TMP_Text magazineSize;
    public Image image;
    public Button getBtn;
    public Button primaryBtn;
    public Button secondaryBtn;

    List<Button> guns;
    float startpos;
    string chosenGun;
    float colorTimer = 999f;
    bool changingColor = false;
    Color buttonSetColor = new Color(0.22f, 0.8f, 1);

    //sounds
    AudioClip switchSound;
    AudioClip selectSound;
    AudioClip buySound;
    AudioClip errorSound;
    AudioClip unlockSound;

    void Start()
    {
        guns = new List<Button>(transform.Find("Guns").GetComponentsInChildren<Button>());
        foreach (var gun in guns)
        {
            TMP_Text[] texts = gun.GetComponentsInChildren<TMP_Text>();
            texts[1].text = game.guns[gun.name].price.ToString() + "$";
            if (game.guns[gun.name].owned)
            {
                texts[1].text = "Owned";
            }
            if (game.guns[gun.name].name == "LaserGun" && game.guns["LaserGun"].tokenUnlocked)
            {
                gun.transform.Find("locked").gameObject.SetActive(false);
            }
            if (game.guns[gun.name].name == "Flamethrower" && game.guns["Flamethrower"].tokenUnlocked)
            {
                gun.transform.Find("locked").gameObject.SetActive(false);
            }
            if (game.guns[gun.name].name == "RPG" && game.guns["RPG"].tokenUnlocked)
            {
                gun.transform.Find("locked").gameObject.SetActive(false);
            }
        }
        startpos = guns[0].transform.localPosition.y;
        OnScrolling(0);
        switchSound = Resources.Load<AudioClip>("Sounds/Switch");
        selectSound = Resources.Load<AudioClip>("Sounds/Select");
        buySound = Resources.Load<AudioClip>("Sounds/Bought");
        errorSound = Resources.Load<AudioClip>("Sounds/Error");
        unlockSound = Resources.Load<AudioClip>("Sounds/Unlocked");
    }
    void Update()
    {
        if (colorTimer < 1f)
        {
            colorTimer += Time.deltaTime;
            if (colorTimer > 1f)
            {
                getBtn.image.color = Color.white;
            }
        }
        if (changingColor)
        {
            getBtn.image.color = Color.Lerp(getBtn.image.color, new Color(0.5f, 1f, 0.5f), Time.deltaTime * 7);
            if (getBtn.image.color.r <= 0.6f)
            {
                changingColor = false;
            }
        }
        else if (colorTimer > 1f)
        {
            getBtn.image.color = Color.Lerp(getBtn.image.color, Color.white, Time.deltaTime * 6.2f);
            if (getBtn.image.color.r >= 0.95f)
            {
                getBtn.image.color = Color.white;
            }
        }
    }
    public void GunChosen(string name)
    {
        getBtn.gameObject.SetActive(true);
        image.gameObject.SetActive(true);
        primaryBtn.gameObject.SetActive(true);
        secondaryBtn.gameObject.SetActive(true);
        chosenGun = name;
        gunName.text = name;
        game.PlaySound(selectSound, false);
        CheckOwned();
        primaryBtn.image.color = Color.white;
        secondaryBtn.image.color = Color.white;

        if(chosenGun == game.primaryGun)
        {
            primaryBtn.image.color = buttonSetColor;
        }
        if(chosenGun == game.secondaryGun)
        {
            secondaryBtn.image.color = buttonSetColor;
        }

        damage.text = "Damage: " + game.guns[name].damage.ToString();
        if(chosenGun == "Shotgun"||chosenGun == "Spas-12")
        {
            damage.text += "x3";
        }
        firerate.text = "Firerate: " + game.guns[name].firerate.ToString()+"s";
        ammoType.text = "Ammo: " + game.guns[name].ammoType;
        magazineSize.text = "Magazine size: " + game.guns[name].magazineSize.ToString();
        image.sprite = Resources.Load<Sprite>($"Images/Guns/{name}");
    }
    public void GetGun()
    {
        GameObject gunPrefab = Resources.Load<GameObject>($"GunPrefabs/{chosenGun}");
        if (game.guns[chosenGun].owned)
        {
            if (getBtn.image.color == Color.white || getBtn.image.color == new Color(1, 0.48f, 0.48f))
            {
                changingColor = true;
            }

            player.ChangeGun(gunPrefab);
        }
        else
        {
            if (game.guns[chosenGun].tokenUnlocked)
            {
                if (game.Money >= game.guns[chosenGun].price)
                {
                    game.Money -= game.guns[chosenGun].price;
                    game.guns[chosenGun].owned = true;
                    game.PlaySound(buySound,true);
                    CheckOwned();
                    if (getBtn.image.color == Color.white || getBtn.image.color == new Color(1, 0.48f, 0.48f))
                    {
                        changingColor = true;
                    }
                    Button gun = guns.Find(gun=>gun.name==chosenGun);
                    TMP_Text[] texts = gun.GetComponentsInChildren<TMP_Text>();
                    texts[1].text = "Owned";
                    player.ChangeGun(gunPrefab,true);
                }
                else
                {
                    colorTimer = 0;
                    game.PlaySound(errorSound, false);
                    getBtn.image.color = new Color(1, 0.48f, 0.48f);
                }
            }
            else
            {
                if (game.Tokens >= game.guns[chosenGun].tokenPrice)
                {
                    game.Tokens -= game.guns[chosenGun].tokenPrice;
                    PlayerPrefs.SetInt("Tokens", game.Tokens);
                    if(chosenGun=="LaserGun")
                    {
                        PlayerPrefs.SetInt("LaserGun",1);
                    }
                    if (chosenGun == "Flamethrower")
                    {
                        PlayerPrefs.SetInt("Flamethrower", 1);
                    }
                    if (chosenGun == "RPG")
                    {
                        PlayerPrefs.SetInt("RPG", 1);
                    }
                    game.guns[chosenGun].tokenUnlocked = true;
                    Button currentGun = guns.FirstOrDefault(a => a.name == chosenGun);
                    currentGun.transform.Find("locked").gameObject.SetActive(false);
                    game.PlaySound(unlockSound,true);
                    CheckOwned();
                    if (getBtn.image.color == Color.white || getBtn.image.color == new Color(1, 0.48f, 0.48f))
                    {
                        changingColor = true;
                    }
                }
                else
                {
                    colorTimer = 0;
                    game.PlaySound(errorSound,false);
                    getBtn.image.color = new Color(1, 0.48f, 0.48f);
                }
            }
        }
    }
    public void OnScrolling(float value)
    {
        int i = 0;
        float parentHeight = this.gameObject.GetComponent<RectTransform>().rect.height;
        foreach (var gun in guns)
        {
            float y = startpos - (i / 2) * (parentHeight * 0.2f) + value * parentHeight;
            gun.transform.localPosition = new Vector3(gun.transform.localPosition.x, y, gun.transform.localPosition.z);
            i++;
        }
    }
    public void OnOpenMenu()
    {
        this.gameObject.SetActive(true);
    }
    private void CheckOwned()
    {
        getBtn.GetComponentInChildren<TMP_Text>().text = "Buy";
        if (game.guns[chosenGun].owned)
        {
            getBtn.GetComponentInChildren<TMP_Text>().text = "Take";
        }
        if (!game.guns[chosenGun].tokenUnlocked)
        {
            getBtn.GetComponentInChildren<TMP_Text>().text = "Unlock";
        }
    }
    public void SetGunSlot(int slot)
    {
        if (!game.guns[chosenGun].owned)
        {
            return;
        }
        game.PlaySound(switchSound, false);
        if (slot == 1)
        {
            game.primaryGun = chosenGun;
            primaryBtn.image.color = buttonSetColor;
            if(secondaryBtn.image.color == buttonSetColor)
            {
                secondaryBtn.image.color = Color.white;
                game.secondaryGun = null;
            }
        }
        if(slot == 2)
        {
            game.secondaryGun = chosenGun;
            secondaryBtn.image.color = buttonSetColor;
            if (primaryBtn.image.color == buttonSetColor)
            {
                primaryBtn.image.color = Color.white;
                game.primaryGun = null;
            }
        }
    }
}
