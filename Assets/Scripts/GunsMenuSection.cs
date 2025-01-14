using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UI;

public class GunsMenuSection: MonoBehaviour
{
    public TMP_Text gunName;
    public TMP_Text damage;
    public TMP_Text firerate;
    public TMP_Text ammoType;
    public Image image;
    public Button getBtn;
    public Button primaryBtn;
    public Button secondaryBtn;

    public GunShooting player;
    public Gamemanager game;

    List<Button> guns;
    float startpos;
    string chosenGun;
    float timer1 = 999f;
    bool changingColor = false;

    void Start()
    {
        startpos = guns[0].transform.localPosition.y;
        OnScrolling(0);
    }
    void Update()
    {
        if (timer1 < 1f)
        {
            timer1 += Time.deltaTime;
            if (timer1 > 1f)
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
        else if (timer1 > 1f)
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
        chosenGun = name;
        CheckOwned();
        gunName.text = name;
        damage.text = "Damage: " + game.guns[name].damage.ToString();
        if(chosenGun == "Shotgun"||chosenGun == "Spas-12")
        {
            damage.text += "x3";
        }
        firerate.text = "Firerate: " + game.guns[name].firerate.ToString()+"s";
        ammoType.text = "Ammo: " + game.guns[name].ammoType;
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
            if (game.Money >= game.guns[chosenGun].price)
            {
                game.Money -= game.guns[chosenGun].price;
                game.guns[chosenGun].owned = true;
                CheckOwned();
                if (getBtn.image.color == Color.white || getBtn.image.color == new Color(1, 0.48f, 0.48f))
                {
                    changingColor = true;
                }
                Button gun = guns.Find(gun=>gun.name==chosenGun);
                TMP_Text[] texts = gun.GetComponentsInChildren<TMP_Text>();
                texts[1].text = "Owned";
                player.ChangeGun(gunPrefab);
            }
            else
            {
                timer1 = 0;
                getBtn.image.color = new Color(1, 0.48f, 0.48f);
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
        if(guns==null)
        {
            guns = new List<Button>(transform.Find("Guns").GetComponentsInChildren<Button>());
        }
        foreach (var gun in guns)
        {
            TMP_Text[] texts = gun.GetComponentsInChildren<TMP_Text>();
            texts[1].text = game.guns[gun.name].price.ToString() + "$";
            if (game.guns[gun.name].owned)
            {
                texts[1].text = "Owned";
            }
        }
    }
    private void CheckOwned()
    {
        getBtn.GetComponentInChildren<TMP_Text>().text = "Buy";
        if (game.guns[chosenGun].owned)
        {
            getBtn.GetComponentInChildren<TMP_Text>().text = "Take";
        }
    }
}
