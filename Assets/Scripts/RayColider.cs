using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayColider : MonoBehaviour
{
    public float lifetime = 5;
    public GameObject Blow;
    int mode = 0;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        lifetime = lifetime - Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void blow(int dir)
    {
        mode = 1;
            GameObject blow = Instantiate(Blow,transform.position, Quaternion.Euler(0, 0, dir),transform);
        blow.GetComponent<Rigidbody2D>().AddForce(GetComponentInChildren<SpriteRenderer>().GetComponent<Transform>().right* -7, ForceMode2D.Impulse);
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (mode == 1)
        {
            if (collision.attachedRigidbody.velocity.y > 0)
            {
                collision.GetComponent<Animator>().Play("fallback");
                if (collision.attachedRigidbody.velocity.x > collision.attachedRigidbody.velocity.y && collision.name == "Obsticle Cone Variant(Clone)")
                    collision.GetComponent<Obsticle>().rot = -45;
            }
            else
            {
                collision.GetComponent<Animator>().Play("fallforward");
                if (collision.attachedRigidbody.velocity.x < collision.attachedRigidbody.velocity.y && collision.name == "Obsticle Cone Variant(Clone)")
                    collision.GetComponent<Obsticle>().rot = -45;
            }
        }
    }


}
