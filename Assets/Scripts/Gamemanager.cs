using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    public Player player;
    public GameObject playerObject;
    public GameObject shop;
    public int Money = 0;


    public bool canShoot = true;
    public void Update()
    {
        if (true)
        {
            if (Input.GetKeyDown(KeyCode.F)) // shop
            {
                player.ToggleCursor();
                ShopActivate();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                GameObject newGunPrefab = Resources.Load<GameObject>("GunPrefabs/Shotgun");
                playerObject.GetComponent<GunShooting>().ChangeGun(newGunPrefab);
            }
        }
    }
    void ShopActivate()
    {
        shop.SetActive(!shop.activeSelf);
        canShoot = !canShoot;
    }
}
