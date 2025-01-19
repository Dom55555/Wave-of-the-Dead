using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildsMenuSection : MonoBehaviour
{
    public TMP_Text buildName;
    public TMP_Text currentAmount;
    public TMP_Text buyAmount;
    public Image buildImage;
    public Image tokenImage;
    public Button buyBtn;
    public Slider slider;

    public Gamemanager game;

    List<Button> buildsIcons;
    string chosenBuild;
    int buildAmount = 10;
    float startpos;
    bool changingColor = false;
    float timer1 = 999;

    void Start()
    {
        startpos = buildsIcons[0].transform.localPosition.y;
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
        else if (timer1 > 1f)
        {
            buyBtn.image.color = Color.Lerp(buyBtn.image.color, Color.white, Time.deltaTime * 6.2f);
            if (buyBtn.image.color.r >= 0.95f)
            {
                buyBtn.image.color = Color.white;
            }
        }
    }
    public void BuildChosen(string name)
    {
        buildImage.gameObject.SetActive(true);
        buyBtn.gameObject.SetActive(true);
        slider.gameObject.SetActive(true);

        chosenBuild = name;
        buildName.text = chosenBuild;
        currentAmount.text = "Left: " + game.builds[chosenBuild].totalAmount.ToString();
        OnChangingAmount(slider.value);
        buildImage.sprite = Resources.Load<Sprite>($"Images/Builds/{name}");
        tokenImage.gameObject.SetActive(game.builds[chosenBuild].tokenBuild?true:false);
    }
    public void OnChangingAmount(float value)
    {
        buildAmount = (int)value;
        buyAmount.text = "Amount: " + buildAmount;
        TMP_Text textBtn = buyBtn.GetComponentInChildren<TMP_Text>();
        textBtn.text = (game.builds[chosenBuild].tokenBuild ? "  " : "") + "Buy for " + (game.builds[chosenBuild].price * buildAmount) + (game.builds[chosenBuild].tokenBuild ? "" : "$");
        textBtn.alignment = game.builds[chosenBuild].tokenBuild ? TextAlignmentOptions.Left : TextAlignmentOptions.Center;
    }
    public void OnOpenMenu()
    {
        this.gameObject.SetActive(true);
        if (buildsIcons == null)
        {
            buildsIcons = new List<Button>(transform.Find("Builds").GetComponentsInChildren<Button>());
        }
        if (chosenBuild != null)
        {
            currentAmount.text = "Left: " + game.builds[chosenBuild].totalAmount.ToString();
        }
    }
    public void Buy()
    {
        if (!game.builds[chosenBuild].tokenBuild && game.Money >= game.builds[chosenBuild].price * buildAmount)
        {
            game.Money -= game.builds[chosenBuild].price * buildAmount;
            OnBuy();
            
        }
        else if (game.builds[chosenBuild].tokenBuild && game.Tokens >= game.builds[chosenBuild].price * buildAmount)
        {
            game.Tokens -= game.builds[chosenBuild].price * buildAmount;
            OnBuy();
        }
        else
        {
            timer1 = 0;
            buyBtn.image.color = new Color(1, 0.48f, 0.48f);
        }
    }
    void OnBuy()
    {
        game.builds[chosenBuild].totalAmount += buildAmount;
        currentAmount.text = "Left: " + game.builds[chosenBuild].totalAmount.ToString();
        if (buyBtn.image.color == Color.white || buyBtn.image.color == new Color(1, 0.48f, 0.48f))
        {
            changingColor = true;
        }
    }
    public void OnScrolling(float value)
    {
        int i = 0;
        float parentHeight = this.gameObject.GetComponent<RectTransform>().rect.height;
        foreach (var ammo in buildsIcons)
        {
            float y = startpos - (i / 2) * (parentHeight * 0.2f) + value * (parentHeight * 0.2f);
            ammo.transform.localPosition = new Vector3(ammo.transform.localPosition.x, y, ammo.transform.localPosition.z);
            i++;
        }
    }
}
