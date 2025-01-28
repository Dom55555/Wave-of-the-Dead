using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingMenu : MonoBehaviour
{
    public GameObject zombie;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        zombie.transform.position = new Vector3(1.35f,0.5f,-4.25f);
    }
    public void OnPlay()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
}
