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
    void Update()
    {
        
    }
    public void GunChosen(string name)
    {
        gunName.text = name;
        switch (name)
        {
            case "Pistol":
                damage.text = "Damage: 10";
                firerate.text = "Firerate: 0.15s";
                ammoType.text = "Ammo: 9mm";
                break;
            case "Shotgun":
                damage.text = "Damage: 25x3";
                firerate.text = "Firerate: 0.6s";
                ammoType.text = "Ammo: 20 gauge";
                break;
        }
        image.sprite = Resources.Load<Sprite>($"Images/Guns/{name}");
    }
    public void GetGun()
    {

    }
}
