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
    public Button button;
    public string chosenGun;
    public RectTransform area;

    public Slider slider;
    public Button[] guns;

    public GunShooting player;
    public Gamemanager game;

    float startpos;

    void Start()
    {
        startpos = guns[0].transform.localPosition.y;
        int i = 0;
        float parentHeight = area.rect.height;
        foreach (var gun in guns)
        {
            float y = startpos - (i / 2) * (parentHeight * 0.45f);
            gun.transform.localPosition = new Vector3(gun.transform.localPosition.x, y, gun.transform.localPosition.z);
            i++;
        }
    }
    void Update()
    {
        
    }
    public void GunChosen(string name)
    {
        button.gameObject.SetActive(true);
        image.gameObject.SetActive(true);
        chosenGun = name;
        gunName.text = name;
        damage.text = "Damage: " + game.gunsProperties[name]["damage"];
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
        float parentHeight = area.rect.height; 
        foreach (var gun in guns)
        {
            float y = startpos - (i / 2) * (parentHeight * 0.45f) + value * (parentHeight * 0.8f);
            gun.transform.localPosition = new Vector3(gun.transform.localPosition.x, y, gun.transform.localPosition.z);
            i++;
        }
        //int i = 0;
        //foreach (var gun in guns)
        //{
        //    float y = startpos - (i / 2) * 250 + value * 800;
        //    gun.transform.position = new Vector3(gun.transform.position.x, y, gun.transform.position.z);
        //    i++;

        }
}
