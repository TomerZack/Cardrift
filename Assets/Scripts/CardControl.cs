using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardControl : MonoBehaviour, IPointerClickHandler
{
   public float animateX = 0;
    public float animateY = 0;
    public float Xmodifier = 0;
    public float cardNum = 1;
    public float animateWidth = 0;
    public float animateHeight = 0;
    public bool animating = false;
    public bool destroy = false;
    float timer = 0;
    public float cardTime = 2;
    bool activated = false;
    Animator AnimControl;
    RectTransform Shape;
    DeckControl UI;
 
    public bool Pressed
    {
        get {
            return activated;
        }
        set {
            activated = value;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (UI.Pressed() < 2)
        {
            UI.activateCard(gameObject);
            AnimControl.SetTrigger("Pressed");
            AnimControl.Play("Pressed", -1, 0.125f);

        }

    }
    void Start()
    {
        UI = GetComponentInParent<RectTransform>().GetComponentInParent<DeckControl>();
        AnimControl = GetComponent<Animator>();
        Shape = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animating)
        {
            if (destroy) Destroy(gameObject);
            RectTransform r = GetComponentInParent<DeckControl>().gameObject.GetComponent<RectTransform>();
            Shape.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, (animateY * r.rect.height/39) , animateHeight);
            Shape.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (animateX * r.rect.width/154) + ((r.rect.width / 4.19f) * cardNum*Xmodifier), animateWidth);
        }
        if (activated)
        {
            timer = timer + Time.deltaTime;
            if (timer >= cardTime)
            {
                AnimControl.SetTrigger("Ended");
                UI.removePressed();
                timer = -10;
            }
        }
    }
}
