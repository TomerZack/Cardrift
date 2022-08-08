using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class GameControl : MonoBehaviour, IUnityAdsListener
{
    // obsticles
    float obsticleTimer = 0;
    float obsticleMulti = 1;
    public GameObject[] Obsticles = new GameObject[2];

    // map
    int environmemt = 0;
    int eithanTimer = 0;
    public Tilemap RoadMap;

    DeckControl UI;
    GameObject[] Cards = new GameObject[15];
    public GameObject[] AllCard = new GameObject[30];
    ArrayList WatingCards = new ArrayList();
    public LinkedList<GameObject> cardOrder = new LinkedList<GameObject>();
    public Canvas canvas;
    public float speed = 0;
    static public float DistTimer;
    public GameObject pausePanel;
    public GameObject lostPanel;
    public GameObject secondChancePanel;
    public static int coins = 0;
    public int deckId;
    int lastObsticle;
    string targetedScene;

    // stats
    static public int ObsticlesColided = 0;
    static public int ObsticlesBurned = 0;
    public int[] cardsUsed = new int[30];
    public int[] obsticlesDamage = new int[10];

    // ads
    #if UNITY_IOS
            public const string gameID = "3476511";
    #elif UNITY_ANDROID
        public const string gameID = "3476510";
    #elif UNITY_EDITOR
            public const string gameID = "1111111";
    #endif
    public string placementId = "rewardedVideo";
    public bool testMode = true;
    public int chances = 2;

    public void portal(bool active)
    {
        if (active)
            GetComponent<Animator>().Play("PortalSpawn");
        else
            GetComponent<Animator>().Play("PortalEnd");
    }

    public void addToCount( GameObject card )
    {
        int i = 0;
        foreach (GameObject c in AllCard)
        {
            if (c.name + "(Clone)" == card.name)
            {
                cardsUsed[i]++;
                return;
            }
            i++;
        }
    }
    public void addDamageToCount(GameObject obsticle, int damage)
    {
        int i = -1;
        foreach (GameObject o in Obsticles)
        {
            if (o != null)
            {
                i++;
                if (obsticle.name == "Explosion")
                {
                    if (obsticle.GetComponentInChildren<Obsticle>().explosionOrigin == o.name + "(Clone)")
                        obsticlesDamage[i] += damage;
                }
                else
                if (o.name + "(Clone)" == obsticle.name)
                    obsticlesDamage[i] += damage;
            }
        }
    }
   
    void Start()
    {
        deckId = PlayerPrefs.GetInt("deckId");
        for (int p = 19, e = 0; p >= 0; p--)
        {
            if (deckId >= Mathf.Pow(2, p))
            {
                Debug.Log("Card id: " + Mathf.Pow(2, p) + " . and his name is: " + AllCard[p].name);
                deckId -= (int)Mathf.Pow(2, p);
                Cards[e] = AllCard[p];
                e++;
            }
        }
        Time.timeScale = 1;
        foreach ( Text C in canvas.GetComponentsInChildren<Text>())
        {
            if (C.gameObject.name == "BestDistance")
                     C.text = "Best: " + PlayerPrefs.GetInt("highscore");
        }
        List<GameObject> ct = new List<GameObject>();
        for (int j = 0; Cards[j] != null; j++)
        {
            ct.Add(Cards[j]);
        }
        for (int i = Random.Range(0, ct.Count); ct.Count > 0; i = Random.Range(0, ct.Count))
        {
            int j = 0;
            foreach (GameObject g in ct)
            {
                if (j == i)
                {
                    cardOrder.AddFirst(g);
                }
                j = j + 1;
            }
            ct.RemoveAt(i);
        }
        UI = canvas.GetComponentInChildren<DeckControl>();
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameID, testMode);
    }


    void Update()
    {
        obsticleTimer = obsticleTimer + speed * obsticleMulti * Time.deltaTime;
        if (obsticleTimer >= 20)
        {
            obsticleTimer = 0;
            if (obsticleMulti < 2) obsticleMulti = obsticleMulti + 0.01f;
            if (obsticleMulti >= 1.08)
            {
                int k = lastObsticle;
                if (obsticleMulti >= 1.3f)
                {
                    for (; k == lastObsticle; k = Random.Range(0, 52)) ;
                    place(k);
                }
                else
                {

                    for (; k == lastObsticle; k = Random.Range(0, 38)) ;
                    place(k);
                }
            }
            else
            {
                place(Random.Range(0, 18));
            }
            { }
            if (obsticleMulti >= 1.05 && obsticleMulti < 1.15) place(Random.Range(100, 108));
            else 
            if (obsticleMulti >= 1.15) place(Random.Range(100, 116));
        }
        RoadMap.GetComponent<TilemapControl>().speed = speed;
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
        while (!UI.fullDeck)
        {
            UI.addCard(cardOrder.First.Value);
            WatingCards.Add(cardOrder.First.Value);
            cardOrder.RemoveFirst();
        }
        DistTimer += Time.deltaTime * speed;
        foreach (Text T in canvas.GetComponentsInChildren<Text>())
        {
            if (T.gameObject.name == "Distance")
            {
                T.text = "" + (int)DistTimer;
                if ((int)DistTimer >= 100 && (int)DistTimer < 1000) T.fontSize = 18;
                if ((int)DistTimer >= 1000) T.fontSize = 14;
            }
            if (T.gameObject.name == "Coins")
            {
                T.text = "" + coins;
            }
        }
    }
    public void setCard(string card)
    {
        GetComponentInChildren<WheelControl>().acticateCard(card);
        foreach (GameObject g in WatingCards)
        {
            if (g.name + "(Clone)" == card)
            {
                cardOrder.AddLast(g);
                WatingCards.Remove(g);
                break;
            }
        }
    }
    void placeMirror(GameObject obsticle, float x, float y = 12)
    {
        Vector3 p1 = new Vector3(x, y);
        Vector3 p2 = new Vector3(-x, y);
        Instantiate(obsticle, p1, Quaternion.identity, RoadMap.GetComponent<Transform>());
        Instantiate(obsticle, p2, Quaternion.identity, RoadMap.GetComponent<Transform>());
    }
    void placeTrio(GameObject obsticle, float x, float y = 13, float spacing = 1)
    {
        Vector3 p = new Vector3(x, y-spacing);
        Instantiate(obsticle, p, Quaternion.identity, RoadMap.GetComponent<Transform>());
        p.Set(x, y, 0);
        Instantiate(obsticle, p, Quaternion.identity, RoadMap.GetComponent<Transform>());
        p.Set(x, y + spacing, 0);
        Instantiate(obsticle, p, Quaternion.identity, RoadMap.GetComponent<Transform>());
    }
    public void setMissle(int x)
    {
        Vector3 p = new Vector3(x, 9, 0);
        GameObject obj =Instantiate(Obsticles[4], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
        obj.SetActive(true);
        obj.GetComponent<Obsticle>().speed = -15;
    }
    void place(int k)
    {
        lastObsticle = k;
        if (k >= 0 && k < 4)
        {
            // random single box
            int f = Random.Range(-4, 4);
            Vector3 p = new Vector3(f, 12);
            Instantiate(Obsticles[0], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            // coins on opposite side
            obsticleTimer = 2;
            if (f != 0)
                placeTrio( Obsticles[3], -f, 12);
        }
        if (k >= 4 && k < 6)
        {
            // random three boxes
            int f = Random.Range(-3, 2);
            Vector3 p = new Vector3(f, 12);
            Instantiate(Obsticles[0], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            p = new Vector3(f + 1f, 12);
            Instantiate(Obsticles[0], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            p = new Vector3(f + 2, 12);
            Instantiate(Obsticles[0], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            placeTrio(Obsticles[3], f+1, 16);
        }
        if (k >= 6 && k < 8)
        {
            // random vertical three cones
            int f = Random.Range(-1, 1) * 4;
            placeTrio(Obsticles[1], f, 14);
            // Coins in the ocer side
            if (f != 0)
                placeTrio(Obsticles[3], -(int)(f*0.75f), 12);
        }
        if (k >= 8 && k < 11)
        {
            // diagonal three cones left
            Vector3 p = new Vector3(4, 12);
            Instantiate(Obsticles[1], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            p = new Vector3(3, 13);
            Instantiate(Obsticles[1], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            p = new Vector3(2, 14);
            Instantiate(Obsticles[1], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            // diagonal coins after
            p = new Vector3(2, 16);
            Instantiate(Obsticles[3], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            p = new Vector3(3, 16.8f);
            Instantiate(Obsticles[3], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            p = new Vector3(4, 17.6f);
            Instantiate(Obsticles[3], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
        }
        if (k >= 11 && k < 14)
        {
            // diagonal three cones right
            Vector3 p = new Vector3(-4, 12);
            Instantiate(Obsticles[1], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            p = new Vector3(-3, 13);
            Instantiate(Obsticles[1], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            p = new Vector3(-2, 14);
            Instantiate(Obsticles[1], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            // diagonal coins after
            p = new Vector3(-2.2f, 16);
            Instantiate(Obsticles[3], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            p = new Vector3(-3, 16.8f);
            Instantiate(Obsticles[3], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            p = new Vector3(-3.8f, 17.6f);
            Instantiate(Obsticles[3], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
        }
        if (k >= 14 && k < 18)
        {
            // box on player position
            int t = (int)Mathf.Round(GetComponentInChildren<WheelControl>().GetComponent<Transform>().position.x);
            Vector3 p = new Vector3(t, 12);
            Instantiate(Obsticles[0], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            placeTrio(Obsticles[3], -1, 18);
            placeTrio(Obsticles[3], 0, 18);
            placeTrio(Obsticles[3], 1, 18);
            obsticleTimer = -1;
        }
        if (k >= 18 && k < 22)
        {
            // cone of cones
            placeMirror(Obsticles[1], 4);
            placeMirror(Obsticles[1], 3, 13);
            placeMirror(Obsticles[1], 2, 14);
            placeMirror(Obsticles[1], 1, 15);
            placeMirror(Obsticles[1], 1, 16);
            placeMirror(Obsticles[1], 1, 17);
            placeMirror(Obsticles[1], 1, 18);
            placeTrio(Obsticles[3], 0, 15);
            placeTrio(Obsticles[3], 0, 18);
            obsticleTimer = -5;
        }
        if (k >= 22 && k < 26)
        {
            // three cones on sides then four in the middle
            placeTrio(Obsticles[1], 4);
            placeTrio(Obsticles[1], -4);
            Vector3 p = new Vector3(0, 15);
            Instantiate(Obsticles[1], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            placeTrio(Obsticles[1], 0, 17);
            obsticleTimer = -2;
            // coins
            placeTrio(Obsticles[3], 2, 15);
            placeTrio(Obsticles[3], -2, 15);
        }
        if (k >= 26 && k < 28)
        {
            // boxes in corners of a diamond shape
            Vector3 p = new Vector3(0, 13);
            Instantiate(Obsticles[0], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            placeMirror(Obsticles[0], 3, 16);
            p.y = 19;
            Instantiate(Obsticles[0], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            // X of coins
            placeMirror(Obsticles[3], 3.2f, 12.8f);
            placeMirror(Obsticles[3], 2.4f, 13.6f);
            placeMirror(Obsticles[3], 1.6f, 14.4f);
            placeMirror(Obsticles[3], 0.8f, 15.2f);
            p.y = 16;
            Instantiate(Obsticles[3], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            placeMirror(Obsticles[3], 0.8f, 16.8f);
            placeMirror(Obsticles[3], 1.6f, 17.6f);
            placeMirror(Obsticles[3], 2.4f, 18.4f);
            placeMirror(Obsticles[3], 3.2f, 19.2f);
            obsticleTimer = -1;
        }
        if (k >= 28 && k < 31)
        {
            //three rows of cones
            placeTrio(Obsticles[1], -1, 15, 3);
            placeTrio(Obsticles[1], 0, 15, 3);
            placeTrio(Obsticles[1], 1, 15, 3);
            obsticleTimer = -2;
        }
        if (k>=31 && k< 34)
        {
            // oil barrel surrounded with cones
            int i = Random.Range(-3,3);
            Vector3 p = new Vector3(i, 13);
            Instantiate(Obsticles[6], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            placeTrio(Obsticles[1], i + 1.2f,13,1.2f);
            placeTrio(Obsticles[1], i - 1.2f,13,1.2f);
            p.y = p.y +1.2f;
            Instantiate(Obsticles[1], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            p.y = p.y - 2.4f;
            Instantiate(Obsticles[1], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
        }
        if (k >= 34 && k < 38)
        {
            // bunch of barrel and boxes
            int i = 1+Random.Range(-1, 1)*2;
            Vector3 p = new Vector3(2*i, 14);
            Instantiate(Obsticles[6], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            p.Set(2.6f*i,13.35f,0);
            GameObject obj = Instantiate(Obsticles[6], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            obj.GetComponent<SpriteRenderer>().color = new Color(0.6886792f, 0.6886792f, 0.6886792f);
            p.Set(1.6f * i, 13.25f, 0);
            Instantiate(Obsticles[0], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            p.Set(0.6f * i, 13.6f, 0);
            obj = Instantiate(Obsticles[0], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            obj.GetComponent<SpriteRenderer>().color = new Color(0.9811321f, 0.821061f, 0.6432894f);
            p.Set(3.8f * i, 14.8f, 0);
            obj = Instantiate(Obsticles[0], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            obj.GetComponent<SpriteRenderer>().color = new Color(0.9811321f, 0.821061f, 0.6432894f);
            obsticleTimer = 4;
        }
        if (k >= 38 && k < 43)
        {
            // 2 barrels on two side and a coins bag in the middle
            placeMirror(Obsticles[6], 2.5f, 13);
            Instantiate(Obsticles[7], new Vector3(0, 13), Quaternion.identity, RoadMap.transform);
            obsticleTimer = 4;
        }
        if (k >= 43 && k < 48)
        {
            // three boxes and 3 cones on other side
            int i = 1 + Random.Range(-1, 1) * 2;
            placeTrio(Obsticles[1], 3 * i);
            Instantiate(Obsticles[0], new Vector3(-i*3.5f, 12), Quaternion.identity, RoadMap.transform);
            Instantiate(Obsticles[0], new Vector3(-i * 2.75f, 14), Quaternion.identity, RoadMap.transform);
            Instantiate(Obsticles[0], new Vector3(-i * 2f, 16), Quaternion.identity, RoadMap.transform);
            // coins
            Instantiate(Obsticles[7], new Vector3(-i * 3.25f, 17), Quaternion.identity, RoadMap.transform);
            placeTrio(Obsticles[3], i, 17);
        }
        if (k >= 48 && k < 52)
        {
            // a box in the middle, then ,edges of cones and bag of coins behind then, then a barrel in the middle.
            Instantiate(Obsticles[0], new Vector3(0f, 12), Quaternion.identity, RoadMap.transform);
            placeMirror(Obsticles[1], 4, 13);
            placeMirror(Obsticles[1], 3, 14);
            placeMirror(Obsticles[1], 2, 15);
            placeMirror(Obsticles[1], 2, 16);
            placeMirror(Obsticles[1], 2, 17);
            placeMirror(Obsticles[1], 2, 18);
            placeMirror(Obsticles[1], 2, 19);
            placeMirror(Obsticles[1], 4, 21);
            placeMirror(Obsticles[1], 3, 20);
            placeMirror(Obsticles[7], 3.5f, 17);
            Instantiate(Obsticles[6], new Vector3(0f, 22), Quaternion.identity, RoadMap.transform);

            //coins
            placeMirror(Obsticles[3], 3, 12);
            placeMirror(Obsticles[3], 2, 13);
            placeMirror(Obsticles[3], 1, 14);
            Instantiate(Obsticles[3], new Vector3(0f, 15), Quaternion.identity, RoadMap.transform);
            Instantiate(Obsticles[3], new Vector3(0f, 19), Quaternion.identity, RoadMap.transform);
            placeMirror(Obsticles[3], 1, 20);
            placeMirror(Obsticles[3], 2, 21);
            placeMirror(Obsticles[3], 3, 22);
            obsticleTimer = -5;
        }
        if (k >= 100 && k < 104)
        {
            // burning wheel left
            Vector3 p = new Vector3(Random.Range(12, 18), 18);
            GameObject obj = Instantiate(Obsticles[2], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            obj.GetComponent<Obsticle>().side_speed = -8;
            obj.SetActive(true);
        }
        if (k >= 104 && k < 108)
        {
            // burning wheel right
            Vector3 p = new Vector3(Random.Range(-18, -12), 18);
            GameObject obj = Instantiate(Obsticles[2], p, Quaternion.identity, RoadMap.GetComponent<Transform>());
            obj.GetComponent<Obsticle>().side_speed = 8;
            obj.GetComponent<SpriteRenderer>().flipX = true;
            obj.SetActive(true);
        }
        if (k >= 108 && k < 111)
        {
            // one Missle
            int i = Random.Range(-4, 4);
            GameObject obj = Instantiate(Obsticles[5], new Vector3(i, 7, 0) ,Quaternion.identity , GetComponent<Transform>());
            obj.GetComponent<IncomingMissleControl>().xPos = i;
            obj.GetComponent<IncomingMissleControl>().gm = this;
        }
        if (k >= 111 && k < 114)
        {
            // two Missle
            int i = Random.Range(-4, 4);
            GameObject obj = Instantiate(Obsticles[5], new Vector3(i, 7, 0), Quaternion.identity, GetComponent<Transform>());
            obj.GetComponent<IncomingMissleControl>().xPos = i;
            obj.GetComponent<IncomingMissleControl>().gm = this;
            int j;
            for (j = i; j == i; j = Random.Range(-4, 4));
            obj = Instantiate(Obsticles[5], new Vector3(j, 7, 0), Quaternion.identity, GetComponent<Transform>());
            obj.GetComponent<IncomingMissleControl>().xPos = j;
            obj.GetComponent<IncomingMissleControl>().gm = this;
            obj.GetComponent<IncomingMissleControl>().lifeTimer = -2f;
        }
    }
    public void pause(bool onlyPLay = false)
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
        if (onlyPLay) {
            foreach (Button b in pausePanel.GetComponentsInChildren<Button>())
                if (b.gameObject.name != "UnPauseButton") b.gameObject.SetActive(false);
         }
        else foreach (Button b in pausePanel.GetComponentsInChildren<Button>(true)) b.gameObject.SetActive(true);
    }
    public void unPause()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }
    public void confirmLoss()
    {
        Time.timeScale = 0;
        secondChancePanel.SetActive(true);
        foreach (Button c in secondChancePanel.GetComponentsInChildren<Button>())
        {
            if (c.gameObject.name == "SecondChanceButton" && chances < 1) c.gameObject.SetActive(false);
        }
        secondChancePanel.GetComponentInChildren<Text>().text = "Total:       " + PlayerPrefs.GetInt("coins");
    }
    public void LostInGame()
    {
        secondChancePanel.SetActive(false);
        lostPanel.SetActive(true);
        foreach ( Text T in lostPanel.GetComponentsInChildren<Text>())
        {
            if (T.gameObject.name == "EndScreenTitle")
            {
                if (PlayerPrefs.GetInt("highscore") < DistTimer)
                {
                    T.text = "New Highscore!";
                    T.fontSize = 17;
                }
                else
                    T.text = "Not Bad!";
            }
            if (T.gameObject.name == "EndDistance")
            {
                T.text = "" + (int)DistTimer;
            }
            if (T.gameObject.name == "EndBestDistance")
            {
                T.text = "Best: " + (PlayerPrefs.GetInt("highscore"));
            }
        }
        int y = 0;
        for (int i = 0; i < cardsUsed.Length; i++)
            if (cardsUsed[y] < cardsUsed[i]) y = i;
        int m = 0;
        for (int i = 0; i < obsticlesDamage.Length; i++)
            if (obsticlesDamage[m] < obsticlesDamage[i]) m = i;
        char[] charArray = AllCard[y].name.Remove(0,4).ToCharArray();
        System.Array.Reverse(charArray);
        char[] charArray2 = new string(charArray).Remove(0,7).ToCharArray();
        System.Array.Reverse(charArray2);
        string name = new string(charArray2);
        charArray = Obsticles[m].name.Remove(0, 9).ToCharArray();
        System.Array.Reverse(charArray);
        charArray2 = new string(charArray).Remove(0, 7).ToCharArray();
        System.Array.Reverse(charArray2);
        Loader.saveData(name, new string(charArray2));
        Debug.Log(name + ", " + new string(charArray2));
    }
    public void restart()
    {
        Advertisement.RemoveListener(this);
        Loader.LoadScene("SampleScene");
    }
    public void backToMainMenu()
    {
        GetComponentInChildren<WheelControl>().gameObject.SetActive(false);
        Advertisement.RemoveListener(this);
        targetedScene = "MainMenu";
        UI.GetComponent<Animator>().Play("Unload");
        Time.timeScale = 1;
    }
    public void loadScene()
    {
        Loader.LoadScene(targetedScene);
    }

    public void OnUnityAdsReady(string placementId)
    {
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.Log("Rewarded Ad Error: " + message);
    }

    public void OnUnityAdsDidStart(string placementId)
    {
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (showResult == ShowResult.Finished)
        {
            chances--;
            gameObject.GetComponentInChildren<WheelControl>().resetHealth();
           Obsticle obj = null;
            foreach ( Obsticle o in RoadMap.GetComponentsInChildren<Obsticle>())
            {
                if (obj != null) Destroy(obj.gameObject);
                obj = o;
            }
            Destroy(obj);
            secondChancePanel.SetActive(false);
            pause(true);
        }
        else if (showResult == ShowResult.Skipped)
        {
            LostInGame();
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }
    }
    public void ShowRewardedAd()
    {
        if (Advertisement.GetPlacementState(placementId) == PlacementState.Ready&& chances > 0)
        {
            Advertisement.Show(placementId);
        }
    }
    public void payForSecondChance()
    {
        if (PlayerPrefs.GetInt("coins") > 100&& chances > 0)
        {
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") - 100);
            chances--;
            secondChancePanel.GetComponentInChildren<Text>().text = "Total:       " + PlayerPrefs.GetInt("coins");
            gameObject.GetComponentInChildren<WheelControl>().resetHealth();
            Obsticle obj = null;
            foreach (Obsticle o in RoadMap.GetComponentsInChildren<Obsticle>())
            {
                if (obj != null) Destroy(obj.gameObject);
                obj = o;
            }
            Destroy(obj);
            secondChancePanel.SetActive(false);
            pause(true);
        }
    }
}