using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckControl : MonoBehaviour
{
    float CardelayTimer = 0;
    GameObject[] DeckCards = new GameObject[4];
    LinkedList<GameObject> PressedCards = new LinkedList<GameObject>();
    LinkedList<GameObject> pendingCards = new LinkedList<GameObject>();
    RectTransform shape;
    public GameControl father;
    
    public void Unload()
    {
        father.loadScene();
    }
    public void prepareCover()
    {
        transform.SetAsLastSibling();
        foreach (CardControl c in GetComponentsInChildren<CardControl>())
        {
             c.gameObject.SetActive(false);
        }
    }
    public void removePressed()
    {
            
        try
        {
            PressedCards.RemoveLast();
            RectTransform p = PressedCards.First.Value.GetComponent<RectTransform>();
            p.gameObject.GetComponent<CardControl>().Pressed = true;
            PressedCards.First.Value.GetComponent<CardControl>().cardNum = 0;
            p.GetComponent<Animator>().SetBool("FirstInLine", true);
            father.setCard(p.gameObject.name);
        }
        catch (System.Exception e) {}
    }
    void Start()
    {
        shape = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CardelayTimer > 0)
        {
            CardelayTimer = CardelayTimer - Time.deltaTime;
        }

    }
    public void addCard(GameObject card)
    {
        if (card.GetComponent<CardControl>() == null) return;
        int num;
        for (num = 0; DeckCards[num] != null; num++)
            if (num == 3) return;
        DeckCards[num] = Instantiate(card,new Vector3(0,-200,0), Quaternion.identity, shape);
        DeckCards[num].GetComponent<CardControl>().cardNum = num;
        DeckCards[num].GetComponent<Animator>().Play("CardSpawn");
       // DeckCards[num].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, (3 * shape.rect.height / 48), 181);
       // DeckCards[num].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (7 * shape.rect.width / 154) + ((shape.rect.width/4.19f)*num), 142);
    }
    public void activateCard(GameObject card)
    {
        int i = 0;
        CardelayTimer = 0.4f;
        father.addToCount(card);
        foreach (GameObject g in DeckCards)
        {
            if (g != null)
            {
                if (g.Equals(card))
                {
                    PressedCards.AddFirst(g);
                    DeckCards[i] = null;
                    if (PressedCards.Count > 1) g.GetComponent<CardControl>().cardTime = g.GetComponent<CardControl>().cardTime + 0.1f;
                    else
                    {
                        g.GetComponent<Animator>().SetBool("FirstInLine", true);
                        g.GetComponent<CardControl>().Pressed = true;
                        father.setCard(g.name);
                    }
                }
            }
            i++;
        }
    }
    public bool fullDeck
    {
        get
        {
            if (CardelayTimer > 0) return true;
            foreach(GameObject g in DeckCards)
            {
                if (g == null) return false;
            }
            return true;
        }
    }
    public int Pressed()
    {
        return PressedCards.Count;
    }

}

