using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public RectTransform mask;
    public void changeBar(float max, float health)
    {
        mask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, GetComponent<RectTransform>().rect.width*(health/max));
    }
}
