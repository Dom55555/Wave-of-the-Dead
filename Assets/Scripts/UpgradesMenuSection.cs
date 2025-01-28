using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Reflection;

public class UpgradesMenuSection : MonoBehaviour
{
    public Gamemanager game;
    List<GameObject>upgrades = new List<GameObject>();
    
    List<int>[] prices = new List<int>[3] { new List<int>() { 10,50,100} ,new List<int>() { 75},new List<int>() { 25,75,150} };
    AudioClip buySound;
    AudioClip errorSound;

    void Start()
    {
        buySound = Resources.Load<AudioClip>("Sounds/Unlocked");
        errorSound = Resources.Load<AudioClip>("Sounds/Error");
    }
    public void OnOpenMenu()
    {
        this.gameObject.SetActive(true);
        if(upgrades.Count==0)
        {
            upgrades.Add(transform.Find("HpUpgrade").gameObject);
            upgrades.Add(transform.Find("TrapUpgrade").gameObject);
            upgrades.Add(transform.Find("MoneyUpgrade").gameObject);
        }
        checkOwned();
    }
    public void OnButtonPress(int index)
    {
        if (game.Tokens >= prices[index][game.upgradesManager.owned[index]-1])
        {
            game.Tokens-=prices[index][game.upgradesManager.owned[index]-1];
            PlayerPrefs.SetInt("Tokens",game.Tokens);
            game.upgradesManager.owned[index]++;
            game.upgradesManager.ChangeValues();
            game.PlaySound(buySound, true);
            checkOwned();
            game.upgradesManager.ChangeValues();
        }
        else
        {
            game.PlaySound(errorSound,false);
        }
    }
    void checkOwned()
    {
        for (int i = 0; i < game.upgradesManager.owned.Length; i++)
        {
            for (int j = 2; j < 5; j++)
            {
                if (game.upgradesManager.owned[i] >= j)
                {
                    upgrades[i].transform.Find($"{j}").GetComponent<Image>().color = new Color(0.674f, 1, 0.64f);
                }
                if (game.upgradesManager.owned[i]==j && game.upgradesManager.owned[i] <= prices[i].Count)
                {
                    upgrades[i].transform.Find("UpgradeBtn").transform.Find("Text").GetComponent<TMP_Text>().text = prices[i][game.upgradesManager.owned[i] - 1].ToString();
                }
                if (game.upgradesManager.owned[i] > prices[i].Count)
                {
                    upgrades[i].transform.Find("UpgradeBtn").gameObject.SetActive(false);
                }
                if (i == 1)
                {
                    break;
                }
            }
        }
    }
}
