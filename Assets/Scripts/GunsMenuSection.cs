using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunsMenuSection: MonoBehaviour
{
    public TMP_Text gunName;
    public TMP_Text damage;
    public TMP_Text firerate;
    public TMP_Text ammoType;
    public Image image;
    public string chosenGun;

    public GunShooting player;
    public Gamemanager game;

    void Update()
    {
        
    }
    public void GunChosen(string name)
    {
        chosenGun = name;
        gunName.text = name;
        damage.text = "Damage: " + game.gunsProperties[name]["damage"];
        firerate.text = "Firerate: " + game.gunsProperties[name]["firerate"];
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
            }
            else
            {
                print("Not enough money");
            }
        }
    }
}
