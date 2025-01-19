using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoMenuSection : MonoBehaviour
{
    public TMP_Text ammoName;
    public TMP_Text currentAmount;
    public TMP_Text buyAmount;
    public Image ammoImage;
    public Button buyBtn;
    public Slider slider;
    
    public Gamemanager game;

    List<Button> ammoIcons;
    string chosenAmmo;
    int ammoAmount = 10;
    float startpos;
    bool changingColor = false;
    float timer1 = 999;

    void Start()
    {
        startpos = ammoIcons[0].transform.localPosition.y;
        OnScrolling(0);
    }
    void Update()
    {
        if (timer1 < 1f)
        {
            timer1 += Time.deltaTime;
            if (timer1 > 1f)
            {
                buyBtn.image.color = Color.white;
            }
        }
        if (changingColor)
        {
            buyBtn.image.color = Color.Lerp(buyBtn.image.color, new Color(0.5f, 1f, 0.5f), Time.deltaTime * 7);
            if (buyBtn.image.color.r <= 0.6f)
            {
                changingColor = false;
            }
        }
        else if (timer1>1f)
        {
            buyBtn.image.color = Color.Lerp(buyBtn.image.color, Color.white, Time.deltaTime * 6.2f);
            if(buyBtn.image.color.r >=0.95f)
            {
                buyBtn.image.color = Color.white;
            }
        }
    }
    public void AmmoChosen(string name)
    {
        ammoImage.gameObject.SetActive(true);
        buyBtn.gameObject.SetActive(true);
        slider.gameObject.SetActive(true);

        chosenAmmo = name;
        ammoName.text = chosenAmmo;
        currentAmount.text = "Left: " + game.playerAmmo[chosenAmmo].totalAmount.ToString();
        OnChangingAmount(slider.value);
        ammoImage.sprite = Resources.Load<Sprite>($"Images/Ammo/{name}");
    }
    public void OnChangingAmount(float value)
    {
        ammoAmount = (int)value;
        buyAmount.text = "Amount: " + ammoAmount;
        buyBtn.GetComponentInChildren<TMP_Text>().text = "Buy for " + game.playerAmmo[chosenAmmo].price * ammoAmount + " $";

    }
    public void OnOpenMenu()
    {
        this.gameObject.SetActive(true);
        if (ammoIcons == null)
        {
            ammoIcons = new List<Button>(transform.Find("Ammo").GetComponentsInChildren<Button>());
        }
        if(chosenAmmo!=null)
        {
            currentAmount.text = "Left: " + game.playerAmmo[chosenAmmo].totalAmount.ToString();
        }
    }
    public void Buy()
    {
        if(game.Money>= game.playerAmmo[chosenAmmo].price * ammoAmount) // price * amount
        {
            game.Money -= game.playerAmmo[chosenAmmo].price * ammoAmount;
            game.playerAmmo[chosenAmmo].totalAmount += ammoAmount;
            currentAmount.text = "Left: "+game.playerAmmo[chosenAmmo].totalAmount.ToString();
            if(buyBtn.image.color == Color.white || buyBtn.image.color == new Color(1, 0.48f, 0.48f))
            {
                changingColor = true;
            }
        }
        else
        {
            timer1 = 0;
            buyBtn.image.color = new Color(1, 0.48f, 0.48f);
        }
    }
    public void OnScrolling(float value)
    {
        int i = 0;
        float parentHeight = this.gameObject.GetComponent<RectTransform>().rect.height;
        foreach (var ammo in ammoIcons)
        {
            float y = startpos - (i / 2) * (parentHeight * 0.2f) + value * (parentHeight * 0.4f);
            ammo.transform.localPosition = new Vector3(ammo.transform.localPosition.x, y, ammo.transform.localPosition.z);
            i++;
        }
    }
}
