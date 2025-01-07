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
    public Button button;

    public GunShooting player;
    public Gamemanager game;

    List<Button> guns;
    float startpos;
    string chosenGun;

    void Start()
    {
        startpos = guns[0].transform.localPosition.y;
        OnScrolling(0);
    }
    public void GunChosen(string name)
    {
        button.gameObject.SetActive(true);
        image.gameObject.SetActive(true);
        chosenGun = name;
        gunName.text = name;
        damage.text = "Damage: " + game.gunsProperties[name]["damage"];
        if(chosenGun == "Shotgun"||chosenGun == "Spas-12")
        {
            damage.text += "x3";
        }
        firerate.text = "Firerate: " + game.gunsProperties[name]["firerate"]+"s";
        ammoType.text = "Ammo: " + game.gunsProperties[name]["ammoType"];
        image.sprite = Resources.Load<Sprite>($"Images/Guns/{name}");
    }
    public void GetGun()
    {
        GameObject gunPrefab = Resources.Load<GameObject>($"GunPrefabs/{chosenGun}");
        if (bool.Parse(game.gunsProperties[chosenGun]["owned"]))
        {
            player.ChangeGun(gunPrefab);
        }
        else
        {
            if (game.Money >= int.Parse(game.gunsProperties[chosenGun]["price"]))
            {
                game.Money -= int.Parse(game.gunsProperties[chosenGun]["price"]);
                game.gunsProperties[chosenGun]["owned"] = "True";
                Button gun = guns.Find(gun=>gun.name==chosenGun);
                TMP_Text[] texts = gun.GetComponentsInChildren<TMP_Text>();
                texts[1].text = "Owned";
                player.ChangeGun(gunPrefab);
            }
            else
            {
                print("Not enough money");
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
            texts[1].text = game.gunsProperties[gun.name]["price"] + "$";
            if (game.gunsProperties[gun.name]["owned"] == "True")
            {
                texts[1].text = "Owned";
            }
        }
    }
}
