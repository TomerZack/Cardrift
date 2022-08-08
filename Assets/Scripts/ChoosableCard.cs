using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChoosableCard : MonoBehaviour , IPointerClickHandler
{
    public int id;
    bool chosen = false;
    private bool owned = false;
    public DeckCraftControl deck;
    public Image darkened;
    public Image locked;
    public void OnPointerClick(PointerEventData eventData)
    { if (owned)
        {

        if (chosen)
        {
            deck.removeCard(id);
            chosen = false;
            darkened.enabled = false;
            GetComponent<RectTransform>().SetPositionAndRotation(new Vector3(GetComponent<RectTransform>().position.x, GetComponent<RectTransform>().position.y + 1), Quaternion.identity);
            deck.GetComponentInChildren<DeckBar>().removeCard(GetComponent<Image>());
        }
        else
            if (!deck.isFilled())
        {
            addCard();
        }
        else deck.CardsCountMismatch();
        eventData.Reset();
        }
    }
    public void addCard()
    {
        deck.addCard(id);
        chosen = true;
        darkened.enabled = true;
        GetComponent<RectTransform>().SetPositionAndRotation(new Vector3(GetComponent<RectTransform>().position.x, GetComponent<RectTransform>().position.y-1), Quaternion.identity);
        deck.GetComponentInChildren<DeckBar>().addCard(GetComponent<Image>());
    }
    public void setOwned()
    {
        owned = true;
        locked.gameObject.SetActive(false);
    }
    public bool Owned {
        get
        {
            return owned;
        }
    }
}
