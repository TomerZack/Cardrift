using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class DeckCraftControl : MonoBehaviour, IUnityAdsListener
{
    int deckId;
    int cards_c;
    public ChoosableCard[] cards = new ChoosableCard[30];
    public Button buyButton;
    public GameObject infoPanel;
    public Text buyText;
    public Text Coins;


    // ads
    public GameObject gift;
    public Animator giftPanel;
#if UNITY_IOS
            public const string gameID = "3476511";
#elif UNITY_ANDROID
    public const string gameID = "3476510";
#elif UNITY_EDITOR
            public const string gameID = "1111111";
#endif
    public string placementId = "rewardedVideo";
    public bool testMode = true;
    void Start()
    {
        //PlayerPrefs.SetInt("giftCounter", 0);
        //PlayerPrefs.SetInt("cardsOwnedId", 1 + 2 + 4 + 8 + 16 + 32 + 1024); PlayerPrefs.SetInt("deckId", 1 + 2 + 4 + 8 + 16 + 32 + 1024); PlayerPrefs.SetInt("cardsBought", 0);
        //PlayerPrefs.SetInt("coins", 7000);
        Coins.text = PlayerPrefs.GetInt("coins") + "";
        buyText.text = (500 + (PlayerPrefs.GetInt("cardsBought") * 100)) + " coins for a card!";
        buyButton.GetComponentInChildren<Animator>().gameObject.SetActive(PlayerPrefs.GetInt("coins") > (500 + (PlayerPrefs.GetInt("cardsBought") * 100)));
        if (PlayerPrefs.GetInt("cardsOwnedId") == (2 ^ 20) - 1) buyButton.enabled = false;
        Debug.Log(PlayerPrefs.GetInt("cardsOwnedId"));
        int tempOwned = PlayerPrefs.GetInt("cardsOwnedId");
        for (int p = 19, e = 0; p >= 0; p--)
        {
            if (tempOwned >= Mathf.Pow(2, p))
            {
                tempOwned -= (int)Mathf.Pow(2, p);
                foreach (ChoosableCard c in cards)
                {
                    if (c.id == Mathf.Pow(2, p))
                    {
                        c.setOwned();
                        break;
                    }
                }
            }
        }
        int tempdeck = PlayerPrefs.GetInt("deckId");
        for (int p = 19, e = 0; p >= 0; p--)
        {
            if (tempdeck >= Mathf.Pow(2, p))
            {
                tempdeck -= (int)Mathf.Pow(2, p);
                foreach (ChoosableCard c in cards)
                {
                    if (c.id == Mathf.Pow(2, p))
                    {
                        c.addCard();
                        break;
                    }
                }
            }
        }
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameID, testMode);
        gift.SetActive(PlayerPrefs.GetInt("giftCounter") >= 0);
    }

   public bool isFilled()
    {
        if (cards_c < 10) return false;
        return true;
    }
    public void addCard(int id)
    {
        deckId += id;
        cards_c++;
    }
    public void removeCard(int id)
    {
        deckId -= id;
        cards_c--;
    }
    public void saveDeck()
    {
        if (cards_c < 7) CardsCountMismatch();
        else
        {
            Advertisement.RemoveListener(this);
            GetComponentInParent<Animator>().Play("Unload");
        }
    }
    public void Unload()
    {
        Loader.saveDeck(deckId);
    }
    public void prepareCover() {
        gift.SetActive(false);
    }
    public void CardsCountMismatch()
    {
        foreach (Text t in GetComponentsInChildren<Text>())
        {
            if (t.name == "Text Limit") t.GetComponent<Animator>().Play("TextRedBold");
        }
    }
    public void buyCard()
    {
        if (PlayerPrefs.GetInt("coins") >= 500 + (PlayerPrefs.GetInt("cardsBought") * 100))
        {
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") - (500 + (PlayerPrefs.GetInt("cardsBought") * 100)));
            Coins.text = PlayerPrefs.GetInt("coins") + "";
            PlayerPrefs.SetInt("cardsBought", PlayerPrefs.GetInt("cardsBought") + 1);
            buyText.text = (500 + (PlayerPrefs.GetInt("cardsBought") * 100)) + " coins for a card!";
            ChoosableCard card = null;
            while (card == null || card.Owned || (card.id == 524288 && PlayerPrefs.GetInt("cardsBought") < 5))
            {
                card = cards[Random.Range(0, cards.Length)];
            }
            card.setOwned();
            PlayerPrefs.SetInt("cardsOwnedId", PlayerPrefs.GetInt("cardsOwnedId") + card.id);
            cardInfo(card.id);
            buyButton.enabled = !(PlayerPrefs.GetInt("cardsOwnedId") == (2 ^ 20) - 1);
        }
    }
    public void ExitInfo()
    {
        infoPanel.SetActive(false);
    }
    public void cardInfo(int id)
    {
        infoPanel.SetActive(true);
        foreach (cardId i in infoPanel.GetComponentsInChildren<cardId>(true))
        {
            if (i.id == id)
            {
                i.gameObject.SetActive(true);
                i.gameObject.GetComponent<Animator>().Play("Show");
            }
            else i.gameObject.SetActive(false);
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
    }

    public void OnUnityAdsDidError(string message)
    {
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        gift.SetActive(false);
        PlayerPrefs.SetInt("giftCounter", Random.Range(-3, -1));

    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        giftPanel.gameObject.SetActive(true);
        giftPanel.Play("Show");
        PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + 100);
        Coins.text = PlayerPrefs.GetInt("coins") + "";
        buyButton.GetComponentInChildren<Animator>(true).gameObject.SetActive(PlayerPrefs.GetInt("coins") > (500 + (PlayerPrefs.GetInt("cardsBought") * 100)));
    }
    public void ShowRewardedAd()
    {
        if (Advertisement.GetPlacementState(placementId) == PlacementState.Ready)
        {
            Advertisement.Show(placementId);
        }
    }
    public void closeGiftPanel()
    {
        giftPanel.Play("Close");
    }
}
