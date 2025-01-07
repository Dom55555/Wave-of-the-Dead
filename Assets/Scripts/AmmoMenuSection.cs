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
    public TMP_Text buyButtonText;
    public Image ammoImage;
    public Button buyButton;
    public Slider slider;
    
    public Gamemanager game;

    List<Button> ammoIcons;
    string chosenAmmo;
    int ammoAmount = 10;
    float startpos;
    void Start()
    {
        startpos = ammoIcons[0].transform.localPosition.y;
        OnScrolling(0);
    }

    public void AmmoChosen(string name)
    {
        ammoImage.gameObject.SetActive(true);
        buyButton.gameObject.SetActive(true);
        slider.gameObject.SetActive(true);

        chosenAmmo = name;
        ammoName.text = chosenAmmo;
        currentAmount.text = "Left: " + game.playerAmmo[chosenAmmo].totalAmount.ToString();
        buyAmount.text = "Amount: " + ammoAmount;
        buyButtonText.text = "Buy for " + game.playerAmmo[chosenAmmo].price * ammoAmount + " $";
        ammoImage.sprite = Resources.Load<Sprite>($"Images/Ammo/{name}");
    }
    public void OnChangingAmount(float value)
    {
        ammoAmount = (int)value;
        buyAmount.text = "Amount: " + ammoAmount;
        buyButtonText.text = "Buy for " + game.playerAmmo[chosenAmmo].price * ammoAmount + " $";

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
        }
        else
        {
            print("Not enough money");
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
