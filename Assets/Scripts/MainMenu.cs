using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class MainMenu : MonoBehaviour
{
    string targetedScene;
    int environmemt = 0;
    int eithanTimer = 0;
    public Tilemap RoadMap;
    Image bottom_bar;
    public Animator MarkOnDeckButton;
    bool slided = false;
    #if UNITY_IOS
        public const string gameID = "3476511";
    #elif UNITY_ANDROID
        public const string gameID = "3476510";
    #elif UNITY_EDITOR
        public const string gameID = "1111111";
    #endif
    public string placementId = "bannerInMenu";
    public bool testMode = true;

    public void Start()
    {
        bottom_bar = GetComponentInChildren<Image>();
        foreach (Text T in bottom_bar.GetComponentsInChildren<Text>())
        {
            if (T.gameObject.name == "Best")
                T.text = "Best: " + PlayerPrefs.GetInt("highscore");
            if (T.gameObject.name == "Best Distance")
                T.text = "Best Distance: " + PlayerPrefs.GetInt("highscore");
            if (T.gameObject.name == "Second Best Distance")
                T.text = "Second Best Distance: " + PlayerPrefs.GetInt("secHighscore");
            if (T.gameObject.name == "Coin Number")
                T.text = "" + PlayerPrefs.GetInt("coins");
            if (T.gameObject.name == "Average Distance")
                T.text = "Average Distance: " + (int)PlayerPrefs.GetFloat("average");
            if (T.gameObject.name == "Total Runs")
                T.text = "Total Runs: " + PlayerPrefs.GetInt("gamesPlayed");
            if (T.gameObject.name == "Distance Traveled")
                T.text = "Distance Traveled: " + PlayerPrefs.GetInt("totalDistance");
            if (T.gameObject.name == "Favorite Card")
                T.text = "Favorite Card: " + PlayerPrefs.GetString("favoriteCard");
            if (T.gameObject.name == "Worst Enemy")
                T.text = "Worst Enemy: " + PlayerPrefs.GetString("worstEnemy");
            if (T.gameObject.name == "Obsticles Colided")
                T.text = "Obsticles Colided: " + PlayerPrefs.GetInt("obsticlesColided");
            if (T.gameObject.name == "Obsticles Burned")
                T.text = "Obsticles Burned: " + PlayerPrefs.GetInt("obsticlesBurned");
            if (T.gameObject.name == "Total Coins Collected")
                T.text = "Total Coins Collected: " + PlayerPrefs.GetInt("totalCoins");
            adBanner();
            MarkOnDeckButton.gameObject.SetActive(PlayerPrefs.GetInt("coins") > (500 + (PlayerPrefs.GetInt("cardsBought") * 100))|| PlayerPrefs.GetInt("giftCounter") >= 0);
        }
    }
    void adBanner()
    {
        Advertisement.Initialize(gameID, testMode);
        StartCoroutine(ShowBannerWhenReady());
        Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
    }
    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady(placementId)&&!Advertisement.isInitialized)
        {
            yield return new WaitForSeconds(0.3f);
        }
        Advertisement.Banner.Show(placementId);
    }
    void Update()
    {
        if (RoadMap.GetComponent<TilemapControl>().checkY())
        {
            eithanTimer++;
            if (eithanTimer == 20)
            {
                eithanTimer = 0;
                environmemt = Random.Range(0, 0);
            }

            RoadMap.GetComponent<TilemapControl>().generateRoad(Random.Range(0, 9), environmemt);
        }
    }
    public void startGame()
    {
        GetComponent<Animator>().Play("Unload");
        Advertisement.Banner.Hide();
        targetedScene = "SampleScene";
    }
    public void toDeck()
    {
        GetComponent<Animator>().Play("Unload");
        Advertisement.Banner.Hide();
        targetedScene ="DeckCraft";
    }
    public void unloadMenu()
    {
        Loader.LoadScene(targetedScene);
        Loader.unloadMenu();
    }
    public void slide()
    {
        if (slided)
        {
            bottom_bar.GetComponent<Animator>().Play("Menu Slide Down");
        }
        else
        {
            bottom_bar.GetComponent<Animator>().Play("Menu Slide Up");
        }
        foreach (Button T in bottom_bar.GetComponentsInChildren<Button>())
        {
            if (T.gameObject.name == "Down Button")
                T.transform.rotation = Quaternion.Euler(0, 0, T.transform.rotation.z*-180 + 180);
            else T.enabled = !T.enabled;
        }
        slided = !slided;
    }
    public void resetCards()
    {
        PlayerPrefs.SetInt("giftCounter", -2);
        PlayerPrefs.SetInt("cardsOwnedId", 1 + 2 + 4 + 8 + 16 + 32 + 1024); PlayerPrefs.SetInt("deckId", 1 + 2 + 4 + 8 + 16 + 32 + 1024); PlayerPrefs.SetInt("cardsBought", 0);
        PlayerPrefs.SetInt("coins", 7000);
    }
}
