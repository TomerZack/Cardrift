using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public bool delete = false;
    void Start()
    {
        
    }

    void Update()
    {
        if (delete) Destroy(gameObject);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Obsticle>() != null)
        {
            GameControl.ObsticlesBurned++;
            if (collision.tag == "Explodable") collision.GetComponent<Obsticle>().Explode();
            collision.gameObject.GetComponent<Animator>().Play("Burn");
            collision.attachedRigidbody.SetRotation(0);
            if (collision.gameObject.GetComponent<ParticleSystem>() != null)
                collision.gameObject.GetComponent<ParticleSystem>().Stop();
        }
    }
}
