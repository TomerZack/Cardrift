using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomingMissleControl : MonoBehaviour
{
    public int xPos;
    public float lifeTimer = -0.5f;
    public GameControl gm;
    void Start()
    {
        GetComponent<Animator>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (lifeTimer > -1&& lifeTimer < 0)
        {
            lifeTimer = 0;
            GetComponent<Animator>().enabled = true;
        }
        lifeTimer = lifeTimer + Time.deltaTime;
        if (lifeTimer >= 2) {
            gm.setMissle(xPos);
            Destroy(gameObject);
        }
    }
}
