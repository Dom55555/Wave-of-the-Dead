using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{

    public Gamemanager game;

    public GameObject zombiePrefab;
    public GameObject deadScreen;
    public TMP_Text waveText;
    public TMP_Text zombiesRemainingText;
    public TMP_Text timerText;
    public TMP_Text earnedTokensText;
    public List<Transform> spawnPoints = new List<Transform>();

    public float gameTimer = 15;    
    public int wave = 0;
    public int zombiesLeft = 0;
    public int zombiesRemaining = 0;

    bool waveGoing = false;
    bool changingTokens = false;
    float spawnTimer = 0;
    float currentTokens = 0;
    float speed = 0;
    int totalTokens = 0;

    AudioSource audioSource;
    AudioClip waveBackgroundSound;
    AudioClip waveEndSound;
    AudioClip loseSound;

    void Start()
    {
        audioSource = game.player.transform.Find("Camera").GetComponent<AudioSource>();
        waveBackgroundSound = Resources.Load<AudioClip>("Sounds/BackgroundWaveSound");
        waveEndSound = Resources.Load<AudioClip>("Sounds/WaveEndSound");
        loseSound = Resources.Load<AudioClip>("Sounds/LoseSound");
    }
    void Update()
    {
        if(changingTokens)
        {
            speed += 5*Time.deltaTime;
            currentTokens += speed*Time.deltaTime;
            earnedTokensText.text = "+"+((int)currentTokens).ToString();
            if(currentTokens>=totalTokens)
            {
                earnedTokensText.text = "+" + totalTokens;
                changingTokens = false;
                game.Tokens += totalTokens;
                PlayerPrefs.SetInt("Tokens",game.Tokens);
            }
        }
        if(game.player.hp==0&&waveGoing)
        {
            waveGoing = false;
            GameOver();
        }
        if(game.player.hp==0)
        {
            return;
        }
        if(waveGoing)
        {
            if(zombiesRemaining>0)
            {
                if(spawnTimer>1)
                {
                    SpawnZombie();
                    spawnTimer = 0;
                }
            }
            gameTimer -= Time.deltaTime;
            spawnTimer += Time.deltaTime;
            if(zombiesRemaining+zombiesLeft == 0)
            {
                WaveEnd();
            }
            if(gameTimer<=0)
            {
                waveGoing = false;
                GameOver();
            }
        }
        else
        {
            gameTimer -= Time.deltaTime;
            if(gameTimer <= 0)
            {
                WaveStart();
            }
        }
        zombiesRemainingText.text = (zombiesRemaining + zombiesLeft).ToString();
        timerText.text = ((int)gameTimer).ToString()+"s";
        waveText.text = waveGoing ? wave.ToString() : "Intermission";
        waveText.fontSize = waveGoing ? 30 : 14;
    }
    void SpawnZombie()
    {
        zombiesLeft++;
        zombiesRemaining--;
        GameObject zombie = Instantiate(zombiePrefab, spawnPoints[Random.Range(0, spawnPoints.Count)].position,Quaternion.identity);
        zombie.GetComponent<Zombie>().player = game.player;
        zombie.GetComponent<Zombie>().game = game;
        zombie.GetComponent<Zombie>().buildsManager = game.buildsManager;
        zombie.GetComponent<Zombie>().waveManager = this;
        zombie.GetComponent<Zombie>().hp = 100 + (wave-1) * 8;
        zombie.GetComponent<Zombie>().maxHp = 100 + (wave-1) * 8;
        zombie.GetComponent<Zombie>().damage= 20+(int)(wave-1)/2;
        zombie.GetComponent<Zombie>().maxSpeed = 3f;
        if(wave >10)
        {
            int randomNum = Random.Range(0, 10);
            if(randomNum == 1)
            {
                zombie.GetComponent<Zombie>().maxSpeed = 4f;
            }
        }
    }
    void WaveStart()
    {
        audioSource.Play();
        waveGoing = true;
        wave++;
        zombiesRemaining = Mathf.Clamp(wave,1,30);
        gameTimer = 120;
    }
    void WaveEnd()
    {
        audioSource.Stop();
        game.PlaySound(waveEndSound,true);
        waveGoing = false;
        gameTimer = 10;
    }
    public void ZombieDead()
    {
        zombiesLeft--;
    }
    void GameOver()
    {
        audioSource.Stop();
        game.PlaySound(loseSound,true);
        if (game.shop.activeSelf)
        {
            game.player.ToggleCursor();
            game.ShopActivate();
        }
        deadScreen.SetActive(true);
        changingTokens = true;
        game.player.ToggleCursor();
        game.gunNameBar.SetActive(false);
        game.ammoText.gameObject.SetActive(false);
        game.gunNameText.gameObject.SetActive(false);
        totalTokens = (wave - 1) + (wave > 11 ? (wave - 11): 0) + (wave > 21 ? (wave - 21) : 0) + (wave > 31 ? (wave - 31) : 0) + (wave > 41 ? (wave - 41) : 0);
    }
    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
