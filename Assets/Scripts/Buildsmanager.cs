using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Buildsmanager : MonoBehaviour
{
    public Gamemanager game;
    public Player player;

    public Transform toolSlot;
    public GameObject playerCamera;
    public GameObject buildSlots;
    public TMP_Text buildModeText;

    GameObject hammer;
    void Start()
    {
        GameObject hammerPrefab = Resources.Load<GameObject>($"BuildsPrefabs/Hammer");
        hammer = Instantiate(hammerPrefab, toolSlot.position, playerCamera.transform.rotation);
        hammer.transform.SetParent(toolSlot);
        hammer.SetActive(false);
    }
    void Update()
    {
        if(game.buildMode)
        {

        }
    }

    public void BuildMode()
    {
        player.GetComponent<GunShooting>().ToggleGun(!game.buildMode);
        buildModeText.gameObject.SetActive(game.buildMode);
        buildSlots.SetActive(game.buildMode);
        hammer.SetActive(game.buildMode);
    }
}
