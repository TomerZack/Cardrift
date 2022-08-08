using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckBar : MonoBehaviour
{
    public LinkedList<Image> cards = new LinkedList<Image>();

    public void addCard(Image card)
    {
        cards.AddLast(Instantiate<Image>(card, transform));
        cards.Last.Value.transform.localScale = new Vector3(0.4f,0.4f);
        cards.Last.Value.GetComponent<ChoosableCard>().darkened.enabled = false;
        cards.Last.Value.GetComponent<ChoosableCard>().enabled = false;
        cards.Last.Value.GetComponentInChildren<Button>().gameObject.SetActive(false);
        reArrange();
    }
    public void removeCard(Image card)
    {
        Image c = null;
        foreach (Image c2 in cards)
        {
            if (card.GetComponent<ChoosableCard>().id == c2.GetComponent<ChoosableCard>().id)
                c = c2;
        }
        cards.Remove(c);
        Destroy(c.gameObject);
        
        reArrange();
    }
    public void reArrange()
    {
        float Xoffset = (cards.Count * 14f)/(-2) ;
        int i = 0;
        foreach (Image c in cards)
        {
            c.transform.localPosition = new Vector3(Xoffset+(i*14f),20f);
            i++;
        }
    }
}
