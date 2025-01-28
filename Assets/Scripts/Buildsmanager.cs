using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Buildsmanager : MonoBehaviour
{
    public Gamemanager game;
    public LayerMask layerDefault;
    public LayerMask layerBlock;

    public Transform toolSlot;
    public Transform playerCamera;
    public GameObject buildSlots;
    public TMP_Text buildModeText;
    public TMP_Text buildsLimitText;
    public List<BuildObject> buildsOnMap = new List<BuildObject>();
    public string chosenBuild = "";

    GameObject hammer;
    GameObject chosenBuildModel;
    Renderer chosenBuildRenderer;

    int combinedLayer;
    bool canBuild = true;

    Color chosenColor = new Color(0.114f,0.7f,0.82f,0.72f);
    Color defaultColor = new Color(0.77f,0.68f,0.68f,0.32f);
    Color chosenBuildDefaultColor;

    AudioClip errorSound;
    AudioClip buildSound;
    void Start()
    {
        GameObject hammerPrefab = Resources.Load<GameObject>($"BuildsPrefabs/Hammer");
        hammer = Instantiate(hammerPrefab, toolSlot.position, playerCamera.transform.rotation);
        hammer.transform.SetParent(toolSlot);
        hammer.SetActive(false);
        errorSound = Resources.Load<AudioClip>("Sounds/Error");
        buildSound = Resources.Load<AudioClip>("Sounds/Build");
        combinedLayer = layerDefault | layerBlock;
    }
    void Update()
    {
        if(game.buildMode && game.player.hp!=0)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                chosenBuild = "WoodenBlock";
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                chosenBuild = "MetalBlock";
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                chosenBuild = "SlowingTrap";
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                chosenBuild = "DamageTrap";
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                chosenBuild = "C4";
            }
            //
            if(Input.GetMouseButtonDown(0)&&chosenBuild !="")
            {
                if(canBuild)
                {
                    BuildDone();
                }
                else
                {
                    game.PlaySound(errorSound, false);
                }
            }
            if (Input.GetMouseButtonDown(0) && chosenBuild == "")
            {
                //if looking at block, heal it
            }

            if (Input.GetMouseButtonDown(1))
            {
                chosenBuild = "";
                CheckChosenBuild();
            }
            if(Input.anyKeyDown)
            {
                CheckChosenBuild();
            }
            if(chosenBuildModel!=null)
            {
                PositionBuildModel();
            }
            buildsLimitText.text = $"Blocks: {buildsOnMap.Count}/10";
        }
    }

    public void BuildMode()
    {
        game.player.GetComponent<GunShooting>().ToggleGun(!game.buildMode);
        buildModeText.gameObject.SetActive(game.buildMode);
        buildsLimitText.gameObject.SetActive(game.buildMode);
        buildSlots.SetActive(game.buildMode);
        hammer.SetActive(game.buildMode);
        foreach (Transform slot in buildSlots.transform)
        {
            slot.Find("Amount").GetComponent<TMP_Text>().text = "x"+game.builds[slot.name].totalAmount.ToString();
        }
        if(game.buildMode && chosenBuild!=null)
        {
            CheckChosenBuild();
        }
        if(!game.buildMode)
        {
            Destroy(chosenBuildModel);
        }
    }
    void CheckChosenBuild()
    {
        foreach(Transform slot in buildSlots.transform)
        {
            slot.GetComponent<Image>().color = defaultColor;
        }
        if(chosenBuildModel!=null)
        {
            Destroy(chosenBuildModel);
        }
        if(chosenBuild!="")
        {
            if (game.builds[chosenBuild].totalAmount == 0)
            {
                chosenBuild = "";
                return;
            }
            if(buildsOnMap.Count==10 && chosenBuild.Contains("Block"))
            {
                chosenBuild = "";
                return;
            }
            buildSlots.transform.Find(chosenBuild).GetComponent<Image>().color = chosenColor;
            CreateChosenBuild();
        }
    }
    void CreateChosenBuild()
    {
        GameObject prefab = Resources.Load<GameObject>($"BuildsPrefabs/{chosenBuild}");
        chosenBuildModel = Instantiate(prefab);
        chosenBuildModel.GetComponent<BoxCollider>().enabled = false;
        chosenBuildRenderer = chosenBuildModel.transform.Find("Model").GetComponent<Renderer>();
        chosenBuildDefaultColor = chosenBuildRenderer.material.color;
        chosenBuildDefaultColor.a = 0.9f;
        RaycastHit hit;
        Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, 4.01f, combinedLayer);
        if ((hit.distance < 2 || hit.distance > 4))
        {
            canBuild = true;
        }
        else
        {
            canBuild = false;
        }
        SetColor(hit);
    }
    void PositionBuildModel()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, 4.01f, combinedLayer))
        {
            chosenBuildModel.transform.position = hit.point;
        }
        else
        {
            chosenBuildModel.transform.position = playerCamera.position + playerCamera.forward * 4.01f;
        }
        SetColor(hit);
        chosenBuildModel.transform.rotation = Quaternion.Euler(0,playerCamera.rotation.eulerAngles.y,0);
    }
    void SetColor(RaycastHit hit)
    {
        if ((hit.distance < 2 || hit.distance > 4) && canBuild)
        {
            chosenBuildRenderer.material.color = new Color(1, 0, 0, 0.9f);
            canBuild = false;
        }
        else if ((hit.distance >= 2 && hit.distance <= 4) && !canBuild)
        {
            chosenBuildRenderer.material.color = chosenBuildDefaultColor;
            canBuild = true;
        }
    }
    void BuildDone()
    {
        game.PlaySound(buildSound,true);
        chosenBuildDefaultColor.a = 1;
        chosenBuildRenderer.material.color = chosenBuildDefaultColor;
        chosenBuildModel.layer = LayerMask.NameToLayer("Block");
        chosenBuildModel.GetComponent<BoxCollider>().enabled = true;
        BuildObject buildObject = chosenBuildModel.GetComponent<BuildObject>();
        buildObject.game = game;
        buildObject.buildName = chosenBuild;
        buildObject.placed = true;
        if(chosenBuild == "WoodenBlock")
        {
            buildObject.hp = 150;
            buildObject.maxHp = 150;
            buildObject.GetComponent<NavMeshObstacle>().enabled = true;
            GetReachablePosition(buildObject);
        }
        else if (chosenBuild=="MetalBlock")
        {
            buildObject.hp = 500;
            buildObject.maxHp = 500;
            buildObject.GetComponent<NavMeshObstacle>().enabled = true;
            GetReachablePosition(buildObject);
        }
        else if (chosenBuild=="SlowingTrap" || chosenBuild=="DamageTrap")
        {
            buildObject.duration = 240;
        }
        game.builds[chosenBuild].totalAmount--;
        foreach (Transform slot in buildSlots.transform)
        {
            slot.Find("Amount").GetComponent<TMP_Text>().text = "x" + game.builds[slot.name].totalAmount.ToString();
        }
        chosenBuildModel = null;
        CheckChosenBuild();
    }
    void GetReachablePosition(BuildObject block)
    {
        RaycastHit hit;
        Vector3 newPosition = block.transform.position + Vector3.up * 0.05f;
        if (Physics.Raycast(newPosition, Vector3.down, out hit, Mathf.Infinity,layerDefault))
        {
            newPosition = hit.point; 
        }
        else
        {
            Debug.LogWarning("No ground detected below the air position!");
        }
        block.reachablePosition = newPosition;
        buildsOnMap.Add(block);
    }
}
