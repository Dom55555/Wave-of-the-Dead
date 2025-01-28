using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesManager : MonoBehaviour
{
    public Gamemanager game;

    public bool trapsDamage = true;
    public int maxPlayerHp = 100;
    public float moneyMultiplier = 1;
    public int[] owned = new int[3] { 1, 1, 1 };

    void Start()
    {
        if(!PlayerPrefs.HasKey("Played"))
        {
            PlayerPrefs.SetInt("Played",1);
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("HpUpgrade",1);
            PlayerPrefs.SetInt("TrapsUpgrade",1);
            PlayerPrefs.SetInt("MoneyUpgrade",1);
        }
        owned[0] = PlayerPrefs.GetInt("HpUpgrade");
        owned[1] = PlayerPrefs.GetInt("TrapsUpgrade");
        owned[2] = PlayerPrefs.GetInt("MoneyUpgrade");
        ChangeValues();
    }

    public void ChangeValues()
    {
        if(game.player.hp == maxPlayerHp)
        {
            game.player.hp = 100 + (owned[0] - 1) * 50;
        }
        maxPlayerHp = 100 + (owned[0] - 1) * 50;
        if (owned[1]==2)
        {
            trapsDamage = false;
        }
        if (owned[2]==2)
        {
            moneyMultiplier = 1.15f;
        }
        if (owned[2]==3)
        {
            moneyMultiplier = 1.3f;
        }
        if (owned[2]==4)
        {
            moneyMultiplier = 1.5f;
        }
        PlayerPrefs.SetInt("HpUpgrade", owned[0]);
        PlayerPrefs.SetInt("TrapsUpgrade", owned[1]);
        PlayerPrefs.SetInt("MoneyUpgrade", owned[2]);
        game.PlayerDamaged(0,true);
    }
}
